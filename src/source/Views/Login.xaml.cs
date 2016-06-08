using Chronos.Modules.Navigation;
using Chronos.Presentation.Windows.Controls;
using Chronos.ViewModel;
using nRoute.Components.Composition;
using nRoute.Navigation.Mapping;
using nRoute.ViewModels;

namespace Chronos.Views
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    [MapNavigationContent(NavigationRoutes.Login)]
    [DefineViewViewModel(typeof(Login), typeof(LoginViewModel))] 
    public partial class Login : WindowElement
    {
        #region · Constructors ·

        [ResolveConstructor]
        public Login([ResolveViewModel(typeof(Login))]LoginViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }

        #endregion
    }
}
