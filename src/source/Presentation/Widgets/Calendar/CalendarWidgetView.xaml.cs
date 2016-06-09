using Chronos.Presentation.Windows.Controls;

namespace Chronos.Presentation.Widgets
{
    /// <summary>
    /// Calendar Widget View
    /// </summary>
    public partial class CalendarWidgetView
        : WidgetElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarWidgetView"/> class.
        /// </summary>
        public CalendarWidgetView()
        {
            InitializeComponent();

            this.DataContext = new CalendarWidgetViewModel();
        }
    }
}
