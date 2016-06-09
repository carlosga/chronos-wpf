using System;
using System.Windows;
using Chronos.Presentation.Windows.Controls;

namespace Chronos.Presentation.Widgets
{
    /// <summary>
    /// Clock Widget View
    /// </summary>
    public partial class ClockWidgetView
        : WidgetElement
    {
        #region · Dependency Properties ·

        public static readonly DependencyProperty SecondsProperty = DependencyProperty.Register(
          "Seconds", typeof(string), typeof(ClockWidgetView), new PropertyMetadata("00"));

        public static readonly DependencyProperty MinutesProperty = DependencyProperty.Register(
          "Minutes", typeof(string), typeof(ClockWidgetView), new PropertyMetadata("00"));

        public static readonly DependencyProperty HoursProperty = DependencyProperty.Register(
          "Hours", typeof(string), typeof(ClockWidgetView), new PropertyMetadata("00"));

        public static readonly DependencyProperty PmAmProperty = DependencyProperty.Register(
          "PmAm", typeof(string), typeof(ClockWidgetView), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty Hours24Property = DependencyProperty.Register(
          "Hours24", typeof(bool), typeof(ClockWidgetView), new PropertyMetadata(true));

        public static readonly DependencyProperty DateProperty = DependencyProperty.Register(
          "Date", typeof(string), typeof(ClockWidgetView), new PropertyMetadata(String.Empty));

        public static readonly DependencyProperty DayOfWeekProperty = DependencyProperty.Register(
          "DayOfWeek", typeof(string), typeof(ClockWidgetView), new PropertyMetadata(String.Empty));

        public static readonly DependencyProperty AngleProperty = DependencyProperty.Register(
          "Angle", typeof(int), typeof(ClockWidgetView), new PropertyMetadata(0));

        #endregion

        #region · Properties ·

        public string Seconds
        {
            get { return (string)this.GetValue(SecondsProperty); }
            set { this.SetValue(SecondsProperty, value); }
        }

        public string Minutes
        {
            get { return (string)this.GetValue(MinutesProperty); }
            set { this.SetValue(MinutesProperty, value); }
        }

        public string Hours
        {
            get { return (string)this.GetValue(HoursProperty); }
            set { this.SetValue(HoursProperty, value); }
        }

        public string PmAm
        {
            get { return (string)this.GetValue(PmAmProperty); }
            set { this.SetValue(PmAmProperty, value); }
        }

        public bool Hours24
        {
            get { return (bool)this.GetValue(Hours24Property); }
            set { this.SetValue(Hours24Property, value); }
        }

        public string Date
        {
            get { return (string)this.GetValue(DateProperty); }
            set { this.SetValue(DateProperty, value); }
        }

        public string DayOfWeek
        {
            get { return (string)this.GetValue(DayOfWeekProperty); }
            set { this.SetValue(DayOfWeekProperty, value); }
        }

        public int Angle
        {
            get { return (int)this.GetValue(AngleProperty); }
            set { this.SetValue(AngleProperty, value); }
        }

        #endregion

        #region · Constuctors ·

        /// <summary>
        /// Initializes a new instance of the <see cref="ClockWidgetView"/> class.
        /// </summary>
        public ClockWidgetView()
        {
            InitializeComponent();

            this.DataContext = new ClockWidgetViewModel();
        }

        #endregion

        #region · Methods ·

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ((ClockWidgetViewModel)this.DataContext).Start();
        }

        #endregion 
    }
}
