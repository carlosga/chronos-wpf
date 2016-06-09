using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace Chronos.Presentation.Controls.Events
{
    /// <summary>
    /// http://agsmith.wordpress.com/2008/04/07/propertydescriptor-addvaluechanged-alternative/
    /// </summary>
    public sealed class WeakPropertyChangeNotifier
        : DependencyObject, IDisposable
    {
        private readonly WeakReference _propertySource;

        public WeakPropertyChangeNotifier(DependencyObject propertySource, string path)
            : this(propertySource, new PropertyPath(path))
        {
        }
        public WeakPropertyChangeNotifier(DependencyObject propertySource, DependencyProperty property)
            : this(propertySource, new PropertyPath(property))
        {
        }
        public WeakPropertyChangeNotifier(DependencyObject propertySource, PropertyPath property)
        {
            if (null == propertySource)
                throw new ArgumentNullException("propertySource");
            if (null == property)
                throw new ArgumentNullException("property");

            _propertySource = new WeakReference(propertySource);

            Binding binding = new Binding();
            binding.Path = property;
            binding.Mode = BindingMode.OneWay;
            binding.Source = propertySource;
            BindingOperations.SetBinding(this, ValueProperty, binding);
        }

        public DependencyObject PropertySource
        {
            get
            {
                try
                {
                    // note, it is possible that accessing the target property
                    // will result in an exception so i’ve wrapped this check
                    // in a try catch
                    return _propertySource.IsAlive
                    ? _propertySource.Target as DependencyObject
                    : null;
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Identifies the <see cref="Value"/> dependency property
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value",
        typeof(object), typeof(WeakPropertyChangeNotifier), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnPropertyChanged)));

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WeakPropertyChangeNotifier notifier = (WeakPropertyChangeNotifier)d;
            if (null != notifier.ValueChanged)
                notifier.ValueChanged(notifier, EventArgs.Empty);
        }

        /// <summary>
        /// Returns/sets the value of the property
        /// </summary>
        /// <seealso cref="ValueProperty"/>
        [Description("Returns/sets the value of the property")]
        [Category("Behavior")]
        [Bindable(true)]
        public object Value
        {
            get
            {
                return (object)this.GetValue(ValueProperty);
            }
            set
            {
                this.SetValue(ValueProperty, value);
            }
        }

        public event EventHandler ValueChanged;

        public void Dispose()
        {
            BindingOperations.ClearBinding(this, ValueProperty);
        }
    }
}
