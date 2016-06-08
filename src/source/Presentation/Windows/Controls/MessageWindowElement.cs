// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Windows;
using System.Windows.Input;
using Chronos.Extensions.Windows;
using Chronos.Presentation.Core.VirtualDesktops;
using Chronos.Presentation.Core.Windows;
using nRoute.Services;

namespace Chronos.Presentation.Windows.Controls
{
    /// <summary>
    /// Provides the ability to create modal message windows (like messageboxes)
    /// </summary>
    public sealed class MessageWindowElement 
        : WindowElement
    {
        #region · Dependency Properties ·

        /// <summary>
        /// Identifies the Buttons dependency property.
        /// </summary>
        public static readonly DependencyProperty ButtonsProperty =
            DependencyProperty.Register("Buttons", typeof(DialogButton), typeof(MessageWindowElement),
                new FrameworkPropertyMetadata(DialogButton.Ok));

        #endregion

        #region · Sync Object ·

        static readonly object SyncObject = new object();

        #endregion

        #region · Static Commands ·

        /// <summary>
        /// Accept command
        /// </summary>
        public static RoutedCommand AcceptCommand;

        /// <summary>
        /// Cancel command
        /// </summary>
        public static RoutedCommand CancelCommand;

        #endregion

        #region · Static Constructors ·

        /// <summary>
        /// Initializes the <see cref="MessageWindowElement"/> class.
        /// </summary>
        static MessageWindowElement()
        {
            MessageWindowElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(MessageWindowElement),
                new FrameworkPropertyMetadata(typeof(MessageWindowElement)));

            KeyboardNavigation.IsTabStopProperty.OverrideMetadata(typeof(MessageWindowElement),
                new FrameworkPropertyMetadata(false));

            MessageWindowElement.AcceptCommand = new RoutedCommand("Accept", typeof(MessageWindowElement));
            MessageWindowElement.CancelCommand = new RoutedCommand("Cancel", typeof(MessageWindowElement));
        }

        #endregion
        
        #region · Show "Factory" Methods ·

        /// <summary>
        /// Shows a new <see cref="MessageWindowElement"/> with the given message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static DialogResult Show(string message)
        {
            return MessageWindowElement.Show("Message", message, DialogButton.OkCancel);
        }

        /// <summary>
        /// Shows a new <see cref="MessageWindowElement"/> with the given caption and message
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static DialogResult Show(string caption, string message)
        {
            return MessageWindowElement.Show(caption, message, DialogButton.OkCancel);
        }

        /// <summary>
        /// Shows a new <see cref="MessageWindowElement"/> with the given caption, message and buttons
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="message"></param>
        /// <param name="buttons"></param>
        /// <returns></returns>
        public static DialogResult Show(string caption, string message, DialogButton buttons) 
        {
            lock (SyncObject)
            {
                DialogResult result = DialogResult.None;

                System.Windows.Application.Current.Dispatcher.Invoke(
                    (Action)delegate
                    {
                        MessageWindowElement window = new MessageWindowElement
                        {
                            Title           = caption,
                            Content         = message,
                            Buttons         = buttons,
                            StartupLocation = StartupPosition.CenterParent
                        };

                        result = ServiceLocator.GetService<IVirtualDesktopManager>().ShowDialog(window);
                    });

                return result;
            }
        }

        #endregion

        #region · Properties ·

        /// <summary>
        /// Gets or sets the button combination to be shown
        /// </summary>
        public DialogButton Buttons
        {
            get { return (DialogButton)base.GetValue(MessageWindowElement.ButtonsProperty); }
            set { base.SetValue(MessageWindowElement.ButtonsProperty, value); }
        }

        #endregion

        #region · Constructors ·

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageWindowElement"/> class
        /// </summary>
        public MessageWindowElement()
            : base()
        {
            CommandBinding bindinAccept = new CommandBinding(MessageWindowElement.AcceptCommand, new ExecutedRoutedEventHandler(OnAccept));
            this.CommandBindings.Add(bindinAccept);

            CommandBinding bindingCancel = new CommandBinding(MessageWindowElement.CancelCommand, new ExecutedRoutedEventHandler(OnCancel));
            this.CommandBindings.Add(bindingCancel);
        }

        #endregion

        #region · Protected Methods ·

        /// <summary>
        /// Focuses the window
        /// </summary>
        protected override void GiveFocus()
        {
            this.SetFocus();
        }

        #endregion

        #region · Command Actions ·

        private void OnAccept(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.Buttons == DialogButton.Ok ||
                this.Buttons == DialogButton.OkCancel)
            {
                this.DialogResult = DialogResult.Ok;
            }
            else
            {
                this.DialogResult = DialogResult.Yes;
            }

            this.Hide();
        }

        private void OnCancel(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.Buttons == DialogButton.OkCancel)
            {
                this.DialogResult = DialogResult.Cancel;
            }
            else
            {
                this.DialogResult = DialogResult.No;
            }

            this.Hide();
        }

        #endregion
    }
}
