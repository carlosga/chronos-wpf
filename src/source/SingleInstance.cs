
namespace Chronos
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Remoting;
    using System.Runtime.Remoting.Channels;
    using System.Runtime.Remoting.Channels.Ipc;
    using System.Runtime.Serialization.Formatters;
    using System.Threading;
    using System.Windows;

    /// <summary>
    /// http://www.fishbowlclient.com/
    /// </summary>
    public static class SingleInstance
    {
        #region · Consts ·

        private const string RemoteServiceName = "SingleInstanceApplicationService";

        #endregion

        #region · Inner Types ·

        private class IpcRemoteService : MarshalByRefObject
        {
            #region · Methods ·

            /// <summary>Activate the first instance of the application.</summary>
            /// <param name="args">Command line arguemnts to proxy.</param>
            public void InvokeFirstInstance(IList<string> args)
            {
                if (Application.Current != null && !Application.Current.Dispatcher.HasShutdownStarted)
                {
                    Application.Current.Dispatcher.BeginInvoke((Action<object>)((arg) => SingleInstance.ActivateFirstInstance((IList<string>)arg)), args);
                }
            }

            /// <summary>Overrides the default lease lifetime of 5 minutes so that it never expires.</summary>
            public override object InitializeLifetimeService()
            {
                return null;
            }

            #endregion
        }

        #endregion

        #region · Events ·

        public static event EventHandler<SingleInstanceEventArgs> SingleInstanceActivated;

        #endregion

        #region · Fields ·

        private static Mutex s_singleInstanceMutex;
        private static IpcServerChannel s_channel;

        #endregion

        #region · Methods ·

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static void SafeDispose<T>(ref T disposable) where T : IDisposable
        {
            // Dispose can safely be called on an object multiple times.
            IDisposable t = disposable;
            disposable = default(T);

            if (null != t)
            {
                t.Dispose();
            }
        }

        public static bool InitializeAsFirstInstance(string applicationName)
        {
            IList<string> commandLineArgs = Environment.GetCommandLineArgs() ?? new string[0];

            // Build a repeatable machine unique name for the channel.
            string appId = applicationName + Environment.UserName;
            string channelName = String.Format("{0}:SingleInstanceIPCChannel", appId);

            bool isFirstInstance;

            s_singleInstanceMutex = new Mutex(true, appId, out isFirstInstance);

            if (isFirstInstance)
            {
                CreateRemoteService(channelName);
            }
            else
            {
                SignalFirstInstance(channelName, commandLineArgs);
            }

            return isFirstInstance;
        }

        public static void Cleanup()
        {
            SafeDispose(ref s_singleInstanceMutex);

            if (s_channel != null)
            {
                ChannelServices.UnregisterChannel(s_channel);
                s_channel = null;
            }
        }

        #endregion

        #region · Private Methods ·

        private static void CreateRemoteService(string channelName)
        {
            s_channel = new IpcServerChannel(
                new Dictionary<string, string>
                {
                    { "name", channelName },
                    { "portName", channelName },
                    { "exclusiveAddressUse", "false" },
                },
                new BinaryServerFormatterSinkProvider { TypeFilterLevel = TypeFilterLevel.Full });

            ChannelServices.RegisterChannel(s_channel, true);
            RemotingServices.Marshal(new IpcRemoteService(), RemoteServiceName);
        }

        private static void SignalFirstInstance(string channelName, IList<string> args)
        {
            var secondInstanceChannel = new IpcClientChannel();
            ChannelServices.RegisterChannel(secondInstanceChannel, true);

            string remotingServiceUrl = String.Format("ipc://{0}/{1}", channelName, RemoteServiceName);

            // Obtain a reference to the remoting service exposed by the first instance of the application
            var firstInstanceRemoteServiceReference = (IpcRemoteService)RemotingServices.Connect(typeof(IpcRemoteService), remotingServiceUrl);

            // Pass along the current arguments to the first instance if it's up and accepting requests.
            if (firstInstanceRemoteServiceReference != null)
            {
                firstInstanceRemoteServiceReference.InvokeFirstInstance(args);
            }
        }

        private static void ActivateFirstInstance(IList<string> args)
        {
            if (Application.Current != null && !Application.Current.Dispatcher.HasShutdownStarted)
            {
                var handler = SingleInstanceActivated;

                if (handler != null)
                {
                    handler(Application.Current, new SingleInstanceEventArgs { Args = args });
                }
            }
        }

        #endregion
    }
}