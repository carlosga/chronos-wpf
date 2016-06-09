// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ComponentModel;

namespace Chronos.Modules.Sample.Models
{
    public partial class EmpresaEntity
        : INotifyPropertyChanged, IDataErrorInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _idEmpresa;
        private string _cif;
        private string _idEpigrafeIae;
        private string _idCnae;
        private string _nombre;
        private string _nombreComercial;
        private string _idTipoVia;
        private string _direccion;
        private string _numeroBloque;
        private string _piso;
        private string _puerta;
        private string _codigoPostal;
        private string _ciudad;
        private string _provincia;
        private string _pais;
        private string _telefono1;
        private string _telefono2;
        private string _fax1;
        private string _fax2;
        private string _www;
        private string _email;

        public string IdEmpresa
        {
            get { return _idEmpresa; }
            set
            {
                if (_idEmpresa != value)
                {
                    _idEmpresa = value;
                    OnPropertyChanged("IdEmpresa");
                }
            }
        }

        public string Cif
        {
            get { return _cif; }
            set
            {
                if (_cif != value)
                {
                    _cif = value;
                    OnPropertyChanged("Cif");
                }
            }
        }

        public string IdEpigrafeIae
        {
            get { return _idEpigrafeIae; }
            set
            {
                if (_idEpigrafeIae != value)
                {
                    _idEpigrafeIae = value;
                    OnPropertyChanged("IdEpigrafeIae");
                }
            }
        }

        public string IdCnae
        {
            get { return _idCnae; }
            set
            {
                if (_idCnae != value)
                {
                    _idCnae = value;
                    OnPropertyChanged("IdCnae");
                }
            }
        }

        public string Nombre
        {
            get { return _nombre; }
            set
            {
                if (_nombre != value)
                {
                    _nombre = value;
                    OnPropertyChanged("Nombre");
                }
            }
        }

        public string NombreComercial
        {
            get { return _nombreComercial; }
            set
            {
                if (_nombreComercial != value)
                {
                    _nombreComercial = value;
                    OnPropertyChanged("NombreComercial");
                }
            }
        }

        public string IdTipoVia
        {
            get { return _idTipoVia; }
            set
            {
                if (_idTipoVia != value)
                {
                    _idTipoVia = value;
                    OnPropertyChanged("IdTipoVia");
                }
            }
        }

        public string Direccion
        {
            get { return _direccion; }
            set
            {
                if (_direccion != value)
                {
                    _direccion = value;
                    OnPropertyChanged("Direccion");
                }
            }
        }

        public string NumeroBloque
        {
            get { return _numeroBloque; }
            set
            {
                if (_numeroBloque != value)
                {
                    _numeroBloque = value;
                    OnPropertyChanged("NumeroBloque");
                }
            }
        }

        public string Piso
        {
            get { return _piso; }
            set
            {
                if (_piso != value)
                {
                    _piso = value;
                    OnPropertyChanged("Piso");
                }
            }
        }

        public string Puerta
        {
            get { return _puerta; }
            set
            {
                if (_puerta != value)
                {
                    _puerta = value;
                    OnPropertyChanged("Puerta");
                }
            }
        }

        public string CodigoPostal
        {
            get { return _codigoPostal; }
            set
            {
                if (_codigoPostal != value)
                {
                    _codigoPostal = value;
                    OnPropertyChanged("CodigoPostal");
                }
            }
        }

        public string Ciudad
        {
            get { return _ciudad; }
            set
            {
                if (_ciudad != value)
                {
                    _ciudad = value;
                    OnPropertyChanged("Ciudad");
                }
            }
        }

        public string Provincia
        {
            get { return _provincia; }
            set
            {
                if (_provincia != value)
                {
                    _provincia = value;
                    OnPropertyChanged("Provincia");
                }
            }
        }

        public string Pais
        {
            get { return _pais; }
            set
            {
                if (_pais != value)
                {
                    _pais = value;
                    OnPropertyChanged("Pais");
                }
            }
        }

        public string Telefono1
        {
            get { return _telefono1; }
            set
            {
                if (_telefono1 != value)
                {
                    _telefono1 = value;
                    OnPropertyChanged("Telefono1");
                }
            }
        }

        public string Telefono2
        {
            get { return _telefono2; }
            set
            {
                if (_telefono2 != value)
                {
                    _telefono2 = value;
                    OnPropertyChanged("Telefono2");
                }
            }
        }

        public string Fax1
        {
            get { return _fax1; }
            set
            {
                if (_fax1 != value)
                {
                    _fax1 = value;
                    OnPropertyChanged("Fax1");
                }
            }
        }

        public string Fax2
        {
            get { return _fax2; }
            set
            {
                if (_fax2 != value)
                {
                    _fax2 = value;
                    OnPropertyChanged("Fax2");
                }
            }
        }

        public string Www
        {
            get { return _www; }
            set
            {
                if (_www != value)
                {
                    _www = value;
                    OnPropertyChanged("Www");
                }
            }
        }

        public string Email
        {
            get { return _email; }
            set
            {
                if (_email != value)
                {
                    _email = value;
                    OnPropertyChanged("Email");
                }
            }
        }

        public string Error
        {
            get { return null; }
        }

        public string this[string columnName]
        {
            get { return null; }
        }

        protected virtual void OnPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
