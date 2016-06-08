using Chronos.Presentation.Windows.Controls;

namespace Chronos.Presentation.Widgets
{
    /// <summary>
    /// Navigator Widget View
    /// </summary>
    public partial class NavigatorWidgetView 
        : WidgetElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NavigatorWidgetView"/> class.
        /// </summary>
        public NavigatorWidgetView()
        {
            InitializeComponent();

            this.DataContext = new NavigatorWidgetViewModel();
        }
    }
}
