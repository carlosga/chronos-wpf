using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Chronos.Modules.Navigation;
using Chronos.Modules.Sample.ViewModels;
using Chronos.Presentation.Core.ViewModel;
using Chronos.Presentation.Windows.Controls;
using nRoute.Components.Composition;
using nRoute.Navigation.Mapping;
using nRoute.ViewModels;

namespace Chronos.Modules.Sample.Views
{
    /// <summary>
    /// Interaction logic for EmpresaView.xaml
    /// </summary>
    [MapNavigationContent(NavigationRoutes.Companies)]
    [DefineViewViewModel(typeof(EmpresaView), typeof(EmpresaViewModel))]
    public partial class EmpresaView
        : WindowElement
    {
        #region · Constructors ·

        [ResolveConstructor]
        public EmpresaView([ResolveViewModel(typeof(EmpresaView))]INavigationViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }

        #endregion
    }
}
