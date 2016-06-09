// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel;
using Chronos.Presentation.ViewModel;

namespace Chronos.Model
{
    public sealed class UserLogin
        : ObservableObject, IDataErrorInfo
    {
        private string _userId;
        private string _password;
        private int _workYear;

        public string Error
        {
            get { return null; }
        }

        public string this[string columnName]
        {
            get { return null; }
        }

        public string UserId
        {
            get { return _userId; }
            set
            {
                if (_userId != value)
                {
                    _userId = value;
                    this.NotifyPropertyChanged(() => UserId);
                }
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                if (_password != value)
                {
                    _password = value;
                    this.NotifyPropertyChanged(() => Password);
                }
            }
        }

        public int WorkYear
        {
            get { return _workYear; }
            set
            {
                if (_workYear != value)
                {
                    _workYear = value;
                    this.NotifyPropertyChanged(() => WorkYear);
                }
            }
        }

        public UserLogin()
        {
        }
    }
}
