/*
The MIT License

Copyright (c) 2009-2010. Carlos Guzmán Álvarez. http://chronoswpf.codeplex.com/

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

using System;
using System.Windows;
using System.Windows.Shell;
using Chronos.Authentication;
using nRoute.ApplicationServices;
using nRoute.Components.Composition;
using nRoute.Components.Messaging;

namespace Chronos
{
    /// <summary>
    /// Dedicated observer for application state notifications
    /// </summary>
    [MapChannelObserver(typeof(ApplicationStateInfo), 
        InitializationMode=InitializationMode.WhenAvailable,
        Lifetime=InstanceLifetime.Singleton,
        ThreadOption=ThreadOption.UIThread)]
    public sealed class ApplicationStateObserver : IObserver<ApplicationStateInfo>
    {
        #region · Fields ·

        private SplashScreen splashScreen;

        #endregion

        #region · IObserver<ApplicationStateInfo> Members ·

        /// <summary>
        /// Notifies the observer that the provider has finished sending push-based notifications.
        /// </summary>
        public void OnCompleted()
        {
        }

        /// <summary>
        /// Notifies the observer that the provider has experienced an error condition.
        /// </summary>
        /// <param name="error">An object that provides additional information about the error.</param>
        public void OnError(Exception error)
        {
        }

        /// <summary>
        ///  Provides the observer with new data.
        /// </summary>
        /// <param name="value">The current notification information.</param>
        public void OnNext(ApplicationStateInfo value)
        {
            if (value.CurrentState == ApplicationState.Starting)
            {
                this.splashScreen = new SplashScreen("SplashScreen.png");
                this.splashScreen.Show(false);
            }
            else if (value.CurrentState == ApplicationState.Started)
            {
                // Close the splash screen
                if (this.splashScreen != null)
                {
                    this.splashScreen.Close(new TimeSpan(0, 0, 1));
                    this.splashScreen = null;
                }

                // Initialize the JumpList
                this.InitializeJumpList();

                // Publish LogOn Request 
                Channel<AuthenticationInfo>.Public.OnNext
                (
                    new AuthenticationInfo
                    {
                        Action = AuthenticationAction.LogOn
                    }, 
                    true
                );
            }
        }

        #endregion

        #region · Private Methods ·

        private void InitializeJumpList()
        {
            if (App.RunningOnWin7)
            {
                JumpList jl = JumpList.GetJumpList(nRoute.ApplicationServices.Application.Current);

                if (jl == null)
                {
                    jl = new JumpList();
                    JumpList.SetJumpList(nRoute.ApplicationServices.Application.Current, jl);
                }

                jl.Apply();
            }
        }

        #endregion
    }
}
