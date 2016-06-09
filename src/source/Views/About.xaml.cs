using Chronos.Modules.Navigation;
using Chronos.Presentation.Core.ViewModel;
using Chronos.Presentation.Windows.Controls;
using Chronos.ViewModel;
using nRoute.Components.Composition;
using nRoute.Navigation.Mapping;
using nRoute.ViewModels;

namespace Chronos.Views
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    [MapNavigationContent(NavigationRoutes.About)]
    [DefineViewViewModel(typeof(About), typeof(AboutViewModel))]
    public partial class About : WindowElement
    {
        [ResolveConstructor]
        public About([ResolveViewModel(typeof(About))]INavigationViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }
    }
}
