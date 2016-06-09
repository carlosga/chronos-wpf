using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Chronos.Presentation.Controls.Adorner;
using Chronos.Presentation.Controls.Events;

namespace Chronos.Presentation.Controls.Interactivity.Behaviors
{
    /// <summary>
    /// http://blindmeis.wordpress.com/2010/07/16/wpf-watermark-textbox-behavior/
    /// </summary>
    public sealed class PasswordBoxWatermarkBehavior
        : System.Windows.Interactivity.Behavior<PasswordBox>
    {
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.RegisterAttached("Label", typeof(string), typeof(PasswordBoxWatermarkBehavior));

        public static readonly DependencyProperty LabelStyleProperty =
            DependencyProperty.RegisterAttached("LabelStyle", typeof(Style), typeof(PasswordBoxWatermarkBehavior));

        private TextBlockAdorner _adorner;
        private WeakPropertyChangeNotifier _notifier;

        public string Label
        {
            get { return (string)base.GetValue(LabelProperty); }
            set { base.SetValue(LabelProperty, value); }
        }

        public Style LabelStyle
        {
            get { return (Style)base.GetValue(LabelStyleProperty); }
            set { base.SetValue(LabelStyleProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.Loaded += this.AssociatedObjectLoaded;
            this.AssociatedObject.PasswordChanged += this.AssociatedObjectPasswordChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.Loaded -= this.AssociatedObjectLoaded;
            this.AssociatedObject.PasswordChanged -= this.AssociatedObjectPasswordChanged;

            _notifier = null;
        }

        private void AssociatedObjectPasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            this.UpdateAdorner();
        }

        private void AssociatedObjectLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            _adorner = new TextBlockAdorner(this.AssociatedObject, this.Label, this.LabelStyle);

            this.UpdateAdorner();

            //AddValueChanged for IsFocused in a weak manner
            _notifier = new WeakPropertyChangeNotifier(this.AssociatedObject, UIElement.IsFocusedProperty);
            _notifier.ValueChanged += new EventHandler(this.UpdateAdorner);
        }

        private void UpdateAdorner(object sender, EventArgs e)
        {
            this.UpdateAdorner();
        }

        private void UpdateAdorner()
        {
            if (!String.IsNullOrEmpty(this.AssociatedObject.Password) || this.AssociatedObject.IsFocused)
            {
                // Hide the Watermark Label if the adorner layer is visible
                this.AssociatedObject.TryRemoveAdorners<TextBlockAdorner>();
            }
            else
            {
                // Show the Watermark Label if the adorner layer is visible
                this.AssociatedObject.TryAddAdorner<TextBlockAdorner>(_adorner);
            }
        }
    }
}
