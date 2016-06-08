// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


using System.ComponentModel;
using Chronos.Modules.Navigation;
using Chronos.Modules.Sample.Models;
using Chronos.Presentation.Core.Windows;
using Chronos.Presentation.ViewModel;
using Chronos.Presentation.Core.Navigation;
using nRoute.Components;
using System.Collections.ObjectModel;

namespace Chronos.Modules.Sample.ViewModels
{
    public sealed class EmpresaViewModel
        : WorkspaceViewModel<EmpresaEntity>
    {
        #region · PropertyChangedEventArgs Cached Instances ·

        private static readonly PropertyChangedEventArgs IdEmpresaChangedArgs       = CreateArgs<EmpresaViewModel>(x => x.IdEmpresa);
        private static readonly PropertyChangedEventArgs CifChangedArgs             = CreateArgs<EmpresaViewModel>(x => x.Cif);
        private static readonly PropertyChangedEventArgs IdEpigrafeIaeChangedArgs   = CreateArgs<EmpresaViewModel>(x => x.IdEpigrafeIae);
        private static readonly PropertyChangedEventArgs IdCnaeChangedArgs          = CreateArgs<EmpresaViewModel>(x => x.IdCnae);
        private static readonly PropertyChangedEventArgs NombreChangedArgs          = CreateArgs<EmpresaViewModel>(x => x.Nombre);
        private static readonly PropertyChangedEventArgs NombreComercialChangedArgs = CreateArgs<EmpresaViewModel>(x => x.NombreComercial);
        private static readonly PropertyChangedEventArgs IdTipoViaChangedArgs       = CreateArgs<EmpresaViewModel>(x => x.IdTipoVia);
        private static readonly PropertyChangedEventArgs DireccionChangedArgs       = CreateArgs<EmpresaViewModel>(x => x.Direccion);
        private static readonly PropertyChangedEventArgs NumeroBloqueChangedArgs    = CreateArgs<EmpresaViewModel>(x => x.NumeroBloque);
        private static readonly PropertyChangedEventArgs PisoChangedArgs            = CreateArgs<EmpresaViewModel>(x => x.Piso);
        private static readonly PropertyChangedEventArgs PuertaChangedArgs          = CreateArgs<EmpresaViewModel>(x => x.Puerta);
        private static readonly PropertyChangedEventArgs CodigoPostalChangedArgs    = CreateArgs<EmpresaViewModel>(x => x.CodigoPostal);
        private static readonly PropertyChangedEventArgs CiudadChangedArgs          = CreateArgs<EmpresaViewModel>(x => x.Ciudad);
        private static readonly PropertyChangedEventArgs ProvinciaChangedArgs       = CreateArgs<EmpresaViewModel>(x => x.Provincia);
        private static readonly PropertyChangedEventArgs PaisChangedArgs            = CreateArgs<EmpresaViewModel>(x => x.Pais);
        private static readonly PropertyChangedEventArgs Telefono1ChangedArgs       = CreateArgs<EmpresaViewModel>(x => x.Telefono1);
        private static readonly PropertyChangedEventArgs Telefono2ChangedArgs       = CreateArgs<EmpresaViewModel>(x => x.Telefono2);
        private static readonly PropertyChangedEventArgs Fax1ChangedArgs            = CreateArgs<EmpresaViewModel>(x => x.Fax1);
        private static readonly PropertyChangedEventArgs Fax2ChangedArgs            = CreateArgs<EmpresaViewModel>(x => x.Fax2);
        private static readonly PropertyChangedEventArgs WwwChangedArgs             = CreateArgs<EmpresaViewModel>(x => x.Www);
        private static readonly PropertyChangedEventArgs EmailChangedArgs           = CreateArgs<EmpresaViewModel>(x => x.Email);

        #endregion

        #region · Model Data ·

        public string IdEmpresa
        {
            get { return this.Entity.IdEmpresa; }
            set
            {
                if (this.Entity.IdEmpresa != value)
                {
                    this.Entity.IdEmpresa = value;
                    this.NotifyPropertyChanged(IdEmpresaChangedArgs);
                }
            }
        }

        public string Cif
        {
            get { return this.Entity.Cif; }
            set
            {
                if (this.Entity.Cif != value)
                {
                    this.Entity.Cif = value;
                    this.NotifyPropertyChanged(CifChangedArgs);
                }
            }
        }

        public string IdEpigrafeIae
        {
            get { return this.Entity.IdEpigrafeIae; }
            set
            {
                if (this.Entity.IdEpigrafeIae != value)
                {
                    this.Entity.IdEpigrafeIae = value;
                    this.NotifyPropertyChanged(IdEpigrafeIaeChangedArgs);
                }
            }
        }

        public string IdCnae
        {
            get { return this.Entity.IdCnae; }
            set
            {
                if (this.Entity.IdCnae != value)
                {
                    this.Entity.IdCnae = value;
                    this.NotifyPropertyChanged(IdCnaeChangedArgs);
                }
            }
        }

        public string Nombre
        {
            get { return this.Entity.Nombre; }
            set
            {
                if (this.Entity.Nombre != value)
                {
                    this.Entity.Nombre = value;
                    this.NotifyPropertyChanged(NombreChangedArgs);
                }
            }
        }

        public string NombreComercial
        {
            get { return this.Entity.NombreComercial; }
            set
            {
                if (this.Entity.NombreComercial != value)
                {
                    this.Entity.NombreComercial = value;
                    this.NotifyPropertyChanged(NombreComercialChangedArgs);
                }
            }
        }

        public string IdTipoVia
        {
            get { return this.Entity.IdTipoVia; }
            set
            {
                if (this.Entity.IdTipoVia != value)
                {
                    this.Entity.IdTipoVia = value;
                    this.NotifyPropertyChanged(IdTipoViaChangedArgs);
                }
            }
        }

        public string Direccion
        {
            get { return this.Entity.Direccion; }
            set
            {
                if (this.Entity.Direccion != value)
                {
                    this.Entity.Direccion = value;
                    this.NotifyPropertyChanged(DireccionChangedArgs);
                }
            }
        }

        public string NumeroBloque
        {
            get { return this.Entity.NumeroBloque; }
            set
            {
                if (this.Entity.NumeroBloque != value)
                {
                    this.Entity.NumeroBloque = value;
                    this.NotifyPropertyChanged(NumeroBloqueChangedArgs);
                }
            }
        }

        public string Piso
        {
            get { return this.Entity.Piso; }
            set
            {
                if (this.Entity.Piso != value)
                {
                    this.Entity.Piso = value;
                    this.NotifyPropertyChanged(PisoChangedArgs);
                }
            }
        }

        public string Puerta
        {
            get { return this.Entity.Puerta; }
            set
            {
                if (this.Entity.Puerta != value)
                {
                    this.Entity.Puerta = value;
                    this.NotifyPropertyChanged(PuertaChangedArgs);
                }
            }
        }

        public string CodigoPostal
        {
            get { return this.Entity.CodigoPostal; }
            set
            {
                if (this.Entity.CodigoPostal != value)
                {
                    this.Entity.CodigoPostal = value;
                    this.NotifyPropertyChanged(CodigoPostalChangedArgs);
                }
            }
        }

        public string Ciudad
        {
            get { return this.Entity.Ciudad; }
            set
            {
                if (this.Entity.Ciudad != value)
                {
                    this.Entity.Ciudad = value;
                    this.NotifyPropertyChanged(CiudadChangedArgs);
                }
            }
        }

        public string Provincia
        {
            get { return this.Entity.Provincia; }
            set
            {
                if (this.Entity.Provincia != value)
                {
                    this.Entity.Provincia = value;
                    this.NotifyPropertyChanged(ProvinciaChangedArgs);
                }
            }
        }

        public string Pais
        {
            get { return this.Entity.Pais; }
            set
            {
                if (this.Entity.Pais != value)
                {
                    this.Entity.Pais = value;
                    this.NotifyPropertyChanged(PaisChangedArgs);
                }
            }
        }

        public string Telefono1
        {
            get { return this.Entity.Telefono1; }
            set
            {
                if (this.Entity.Telefono1 != value)
                {
                    this.Entity.Telefono1 = value;
                    this.NotifyPropertyChanged(Telefono1ChangedArgs);
                }
            }
        }

        public string Telefono2
        {
            get { return this.Entity.Telefono2; }
            set
            {
                if (this.Entity.Telefono2 != value)
                {
                    this.Entity.Telefono2 = value;
                    this.NotifyPropertyChanged(Telefono2ChangedArgs);
                }
            }
        }

        public string Fax1
        {
            get { return this.Entity.Fax1; }
            set
            {
                if (this.Entity.Fax1 != value)
                {
                    this.Entity.Fax1 = value;
                    this.NotifyPropertyChanged(Fax1ChangedArgs);
                }
            }
        }

        public string Fax2
        {
            get { return this.Entity.Fax2; }
            set
            {
                if (this.Entity.Fax2 != value)
                {
                    this.Entity.Fax2 = value;
                    this.NotifyPropertyChanged(Fax2ChangedArgs);
                }
            }
        }

        public string Www
        {
            get { return this.Entity.Www; }
            set
            {
                if (this.Entity.Www != value)
                {
                    this.Entity.Www = value;
                    this.NotifyPropertyChanged(WwwChangedArgs);
                }
            }
        }

        public string Email
        {
            get { return this.Entity.Email; }
            set
            {
                if (this.Entity.Email != value)
                {
                    this.Entity.Email = value;
                    this.NotifyPropertyChanged(EmailChangedArgs);
                }
            }
        }

        #endregion

        #region · Properties ·

        /// <summary>
        /// Gets the navigation URL
        /// </summary>
        /// <value></value>
        public override string NavigationRoute
        {
            get { return NavigationRoutes.Companies; }
        }

        #endregion

        #region · Constructors ·

        public EmpresaViewModel()
            : base()
        {
        }

        #endregion

        #region · Overriden Methods ·

        protected override void OnInitialize(nRoute.Components.ParametersCollection requestParameters)
        {
            if (requestParameters.ContainsKey(NavigationParams.NavigationParamsKey))
            {
                object[] rparams = requestParameters.GetValueOrDefault<object[]>(NavigationParams.NavigationParamsKey, null);

                if (rparams != null && rparams[0] is int)
                {
                    int value = (int)rparams[0];
                }
            }
        }

        protected override void InitializePropertyStates()
        {
            this.PropertyStates.Add(e => e.IdEmpresa);
            this.PropertyStates.Add(e => e.Nombre);
            this.PropertyStates.Add(e => e.Cif);
            this.PropertyStates.Add(e => e.IdEpigrafeIae);
            this.PropertyStates.Add(e => e.IdCnae);
            this.PropertyStates.Add(e => e.NombreComercial);
            this.PropertyStates.Add(e => e.IdTipoVia);
            this.PropertyStates.Add(e => e.Direccion);
            this.PropertyStates.Add(e => e.NumeroBloque);
            this.PropertyStates.Add(e => e.Piso);
            this.PropertyStates.Add(e => e.Puerta);
            this.PropertyStates.Add(e => e.CodigoPostal);
            this.PropertyStates.Add(e => e.Ciudad);
            this.PropertyStates.Add(e => e.Provincia);
            this.PropertyStates.Add(e => e.Pais);
            this.PropertyStates.Add(e => e.Telefono1);
            this.PropertyStates.Add(e => e.Telefono2);
            this.PropertyStates.Add(e => e.Fax1);
            this.PropertyStates.Add(e => e.Fax2);
            this.PropertyStates.Add(e => e.Www);
            this.PropertyStates.Add(e => e.Email);
        }

        protected override void OnViewModeChanged()
        {
            base.OnViewModeChanged();

            if (this.PropertyStates.Count > 0)
            {
                bool editing = this.ViewMode == Chronos.Presentation.Core.Windows.ViewModeType.Add ||
                               this.ViewMode == Chronos.Presentation.Core.Windows.ViewModeType.Edit;

                this.PropertyStates[x => x.IdEmpresa].IsEditable                = this.ViewMode == ViewModeType.ViewOnly;
                this.PropertyStates[x => x.Nombre].IsEditable                   = editing;
                this.PropertyStates[x => x.Cif].IsEditable                      = editing;
                this.PropertyStates[x => x.NombreComercial].IsEditable          = editing;
                this.PropertyStates[x => x.IdEpigrafeIae].IsEditable            = editing;
                this.PropertyStates[x => x.IdCnae].IsEditable                   = editing;
                this.PropertyStates[x => x.NombreComercial].IsEditable          = editing;
                this.PropertyStates[x => x.IdTipoVia].IsEditable      = editing;
                this.PropertyStates[x => x.Direccion].IsEditable      = editing;
                this.PropertyStates[x => x.NumeroBloque].IsEditable   = editing;
                this.PropertyStates[x => x.Piso].IsEditable           = editing;
                this.PropertyStates[x => x.Puerta].IsEditable         = editing;
                this.PropertyStates[x => x.CodigoPostal].IsEditable   = editing;
                this.PropertyStates[x => x.Ciudad].IsEditable         = editing;
                this.PropertyStates[x => x.Provincia].IsEditable      = editing;
                this.PropertyStates[x => x.Pais].IsEditable           = editing;
                this.PropertyStates[x => x.Telefono1].IsEditable       = editing;
                this.PropertyStates[x => x.Telefono2].IsEditable       = editing;
                this.PropertyStates[x => x.Fax1].IsEditable            = editing;
                this.PropertyStates[x => x.Fax2].IsEditable            = editing;
                this.PropertyStates[x => x.Www].IsEditable             = editing;
                this.PropertyStates[x => x.Email].IsEditable           = editing;
            }
        }

        #endregion

        #region · Command Actions ·

        protected override void OnInquiryAction(InquiryActionResult<EmpresaEntity> result)
        {
        }

        #endregion
    }
}