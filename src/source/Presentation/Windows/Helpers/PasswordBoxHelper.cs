﻿using System.Windows;
using System.Windows.Controls;

namespace Chronos.Presentation.Windows.Helpers
{
    /// <summary>
    /// This class adds binding capabilities to the standard WPF PasswordBox.
    /// </summary>
    /// <remarks>
    /// http://www.codeproject.com/Articles/37167/Binding-Passwords.aspx
    /// </remarks>
    public static class PasswordBoxHelper
    {
        #region · Static Members ·

        private static bool s_isUpdating = false;

        #endregion

        #region · Attached Properties ·

        /// <summary>
        /// BoundPassword Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty BoundPasswordProperty =
            DependencyProperty.RegisterAttached("BoundPassword",
                typeof(string),
                typeof(PasswordBoxHelper),
                new FrameworkPropertyMetadata(string.Empty, OnBoundPasswordChanged));

        #endregion

        #region · Attached Property Get/Set Methods ·

        /// <summary>
        /// Gets the BoundPassword property.
        /// </summary>
        public static string GetBoundPassword(DependencyObject d)
        {
            return (string)d.GetValue(BoundPasswordProperty);
        }

        /// <summary>
        /// Sets the BoundPassword property.
        /// </summary>
        public static void SetBoundPassword(DependencyObject d, string value)
        {
            d.SetValue(BoundPasswordProperty, value);
        }

        #endregion

        #region · Attached Properties Callbacks ·

        /// <summary>
        /// Handles changes to the BoundPassword property.
        /// </summary>
        private static void OnBoundPasswordChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            PasswordBox password = d as PasswordBox;

            if (password != null)
            {
                // Disconnect the handler while we're updating.
                password.PasswordChanged -= PasswordChanged;
            }

            if (e.NewValue != null)
            {
                if (!s_isUpdating)
                {
                    password.Password = e.NewValue.ToString();
                }
            }
            else
            {
                password.Password = string.Empty;
            }

            // Now, reconnect the handler.
            password.PasswordChanged += new RoutedEventHandler(PasswordChanged);
        }

        /// <summary>
        /// Handles the password change event.
        /// </summary>
        private static void PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox password = sender as PasswordBox;
            s_isUpdating = true;
            SetBoundPassword(password, password.Password);
            s_isUpdating = false;
        }

        #endregion
    }
}
