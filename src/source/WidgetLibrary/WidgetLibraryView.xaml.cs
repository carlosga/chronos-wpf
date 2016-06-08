using Chronos.Presentation.Windows.Controls;

namespace Chronos.WidgetLibrary
{
    /// <summary>
    /// Widget Library Widget View
    /// </summary>
    public partial class WidgetLibraryView
        : WidgetElement
    {
        #region · Constructors ·

        /// <summary>
        /// Initializes a new instance of the <see cref="WidgetLibraryView"/> class.
        /// </summary>
        public WidgetLibraryView()
        {
            InitializeComponent();

            this.DataContext = new WidgetLibraryViewModel();
        }

        #endregion
    }
}
