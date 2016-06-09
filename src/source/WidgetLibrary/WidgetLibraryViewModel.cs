// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Chronos.Presentation.Core.Configuration;
using Chronos.Presentation.Core.VirtualDesktops;
using Chronos.Presentation.Core.Widgets;
using Chronos.Presentation.Core.Windows;
using Chronos.Presentation.ViewModel;
using NLog;
using nRoute.Components;

namespace Chronos.WidgetLibrary
{
    /// <summary>
    /// Widget library ViewModel
    /// </summary>
    public sealed class WidgetLibraryViewModel
        : WidgetViewModel
    {
        #region · Constants ·

        private static readonly string s_allWidgetsKey = "All";

        #endregion

        #region · PropertyChangedEventArgs Cached Instances ·

        private static readonly PropertyChangedEventArgs s_filterChangedArgs = CreateArgs<WidgetLibraryViewModel>(x => x.Filter);
        private static readonly PropertyChangedEventArgs s_widgetsChangedArgs = CreateArgs<WidgetLibraryViewModel>(x => x.Widgets);
        private static readonly PropertyChangedEventArgs s_selectedWidgetChangedArgs = CreateArgs<WidgetLibraryViewModel>(x => x.SelectedWidget);

        #endregion

        #region · Fields ·

        private string _filter;
        private List<string> _groups;
        private List<WidgetItemViewModel> _widgets;
        private IWidget _selectedWidget;
        private ActionCommand _createWidgetCommand;

        #endregion

        #region · Commands ·

        /// <summary>
        /// Gets the create widget command.
        /// </summary>
        /// <value>The create widget command.</value>
        public ActionCommand CreateWidgetCommand
        {
            get
            {
                if (_createWidgetCommand == null)
                {
                    _createWidgetCommand = new ActionCommand
                    (
                        () => OnCreateWidget(),
                        () => CanCreateWidget()
                    );
                }

                return _createWidgetCommand;
            }
        }

        #endregion

        #region · Properties ·

        /// <summary>
        /// Gets/sets a fragment of the name to filter for.
        /// </summary>
        public string Filter
        {
            get { return _filter; }
            set
            {
                if (value != _filter)
                {
                    _filter = value;
                    this.NotifyPropertyChanged(s_filterChangedArgs);
                    this.NotifyPropertyChanged(s_widgetsChangedArgs);

                    this.SelectedWidget = null;
                }
            }
        }

        /// <summary>
        /// Gets the list of widget groups.
        /// </summary>
        /// <value>The groups.</value>
        public IEnumerable<string> Groups
        {
            get { return _groups; }
        }

        /// <summary>
        /// Gets the list of widgets
        /// </summary>
        public IList<WidgetItemViewModel> Widgets
        {
            get
            {
                if (String.IsNullOrEmpty(_filter)
                    || _filter == s_allWidgetsKey)
                {
                    return _widgets;
                }
                else
                {
                    return new ReadOnlyCollection<WidgetItemViewModel>(_widgets.Where(x => x.Group == _filter).ToList());
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected widget.
        /// </summary>
        /// <value>The selected widget.</value>
        public IWidget SelectedWidget
        {
            get { return _selectedWidget; }
            set
            {
                if (_selectedWidget != value)
                {
                    _selectedWidget = value;
                    this.NotifyPropertyChanged(s_selectedWidgetChangedArgs);
                    this.CreateWidgetCommand.RequeryCanExecute();
                }
            }
        }

        #endregion

        #region · Constructors ·

        /// <summary>
        /// Initializes a new instance of the <see cref="WidgetLibrary"/> class.
        /// </summary>
        public WidgetLibraryViewModel()
            : base()
        {
            _widgets = this.DiscoverWidgets();
            _groups = this.DiscoverWidgetGroups();
        }

        #endregion

        #region · Overriden Methods ·

        /// <summary>
        /// Called when the related view is being closed.
        /// </summary>
        public override void Close()
        {
            base.Close();

            _selectedWidget = null;
            _createWidgetCommand = null;

            if (_groups != null)
            {
                _groups.Clear();
                _groups = null;
            }

            if (_widgets != null)
            {
                _widgets.Clear();
                _widgets = null;
            }
        }

        #endregion

        #region · Command Actions ·

        private bool CanCreateWidget()
        {
            return (_selectedWidget != null);
        }

        /// <summary>
        /// Handles the create widget command.
        /// </summary>
        private void OnCreateWidget()
        {
            this.GetService<IVirtualDesktopManager>()
                .Show(_selectedWidget.CreateView() as IDesktopElement);
        }

        #endregion

        #region · Private Methods ·

        private List<string> DiscoverWidgetGroups()
        {
            List<string> groups = new List<string>();
            var q = from widgetdef in _widgets
                    select widgetdef.Group;

            groups.Add(s_allWidgetsKey);
            groups.AddRange(q.Distinct().ToList());

            return groups;
        }

        private List<WidgetItemViewModel> DiscoverWidgets()
        {
            IEnumerable<IWidget> widgetDefs = this.GetService<IWidgetConfigurationService>()
                                                  .GetWidgets();

            if (widgetDefs != null)
            {
                var query = from widgetdef in widgetDefs
                            select new WidgetItemViewModel(widgetdef);

                return query.ToList();
            }

            return null;
        }

        #endregion
    }
}