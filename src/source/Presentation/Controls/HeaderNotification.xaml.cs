using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Chronos.Presentation.Controls
{
    [TemplateVisualState(Name = HIDDEN_STATENAME, GroupName = NOTIFICATION_STATEGROUP)]
    [TemplateVisualState(Name = VISIBLE_STATENAME, GroupName = NOTIFICATION_STATEGROUP)]
    public partial class HeaderNotification
        : UserControl
    {
        private const string NOTIFICATION_STATEGROUP = "NotificationStateGroup";
        private const string HIDDEN_STATENAME = "HiddenState";
        private const string VISIBLE_STATENAME = "VisibleState";
        private readonly static TimeSpan s_TRANSITION_TIMEOUT = TimeSpan.FromMilliseconds(600);

        /// <summary>
        /// Identifies the MessageText dependency property.
        /// </summary>
        public static readonly DependencyProperty MessageTextProperty =
            DependencyProperty.Register("MessageText", typeof(String), typeof(HeaderNotification),
                new FrameworkPropertyMetadata(String.Empty, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(OnMessageTextChanged)));

        private static void OnMessageTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                if (e.NewValue != null)
                {
                    HeaderNotification element = d as HeaderNotification;

                    element.ShowNotification(e.NewValue as String);
                }
            }
        }

        private readonly Object _syncObject = new Object();
        private Queue<InteractiveMessage> _messages;
        private InteractiveMessage _currentMessage;
        private DispatcherTimer _stateTimer;

        public string MessageText
        {
            get { return (String)base.GetValue(HeaderNotification.MessageTextProperty); }
            set { base.SetValue(HeaderNotification.MessageTextProperty, value); }
        }

        public HeaderNotification()
        {
            InitializeComponent();

            // set up
            _messages = new Queue<InteractiveMessage>();
            _stateTimer = new DispatcherTimer();
            _stateTimer.Interval = s_TRANSITION_TIMEOUT;
            _stateTimer.Tick += new EventHandler(StateTimer_Tick);
        }

        public void ShowNotification(string notification)
        {
            // basic checks
            if (String.IsNullOrWhiteSpace(notification))
            {
                return;
            }

            // show or enque message
            lock (_syncObject)
            {
                // if no items are queued then show the message, else enque
                var message = new InteractiveMessage() { Message = notification };

                if (_messages.Count == 0 && _currentMessage == null)
                {
                    ShowMessage(message);
                }
                else
                {
                    _messages.Enqueue(message);
                }
            }
        }

        private void StateTimer_Tick(object sender, EventArgs e)
        {
            _stateTimer.Stop();
            this.ProcessQueue();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            // basic check
            if (_stateTimer.IsEnabled)
            {
                return;
            }

            // we stop the timers and start transitioning
            VisualStateManager.GoToState(this, HIDDEN_STATENAME, true);

            // and transition
            _stateTimer.Start();
        }

        private void Header_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // basic check
            if (_stateTimer.IsEnabled)
            {
                return;
            }

            // we stop the timers and start transitioning
            VisualStateManager.GoToState(this, HIDDEN_STATENAME, true);

            // and transition
            _stateTimer.Start();
        }

        private void ProcessQueue()
        {
            lock (_syncObject)
            {
                if (_messages.Count == 0)
                {
                    _currentMessage = null;
                }
                else
                {
                    ShowMessage(_messages.Dequeue());
                }
            }
        }

        private void ShowMessage(InteractiveMessage message)
        {
            this.HeaderText.Text = message.Message;
            VisualStateManager.GoToState(this, VISIBLE_STATENAME, true);
            _currentMessage = message;
        }

        private class InteractiveMessage
        {
            public string Message
            {
                get;
                set;
            }
        }
    }
}
