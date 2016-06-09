// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Chronos.Extensions;
using Chronos.Presentation.Core.Navigation;
using Chronos.Presentation.ViewModel;
using Chronos.Presentation.Windows;
using nRoute.Components;
using nRoute.Services;
using nRoute.SiteMaps;

namespace Chronos.Presentation.Widgets
{
    /// <summary>
    /// Navigator Widget ViewModel
    /// </summary>
    /// <remarks>
    /// http://www.codeproject.com/KB/WPF/TreeViewWithViewModel.aspx
    /// </remarks>
    public sealed class NavigatorWidgetViewModel
        : WidgetViewModel
    {
        #region · Fields ·

        private readonly ReadOnlyCollection<NavigationNode> _functions;
        private readonly SiteMapNode _rootOption;
        private List<NavigationNode> _filteredFunctions;
        private string _filterText;
        private ICommand _navigateToCommand;

        #endregion

        #region · Commands ·

        /// <summary>
        /// Gets the navigation command
        /// </summary>
        public ICommand NavigateToCommand
        {
            get
            {
                if (_navigateToCommand == null)
                {
                    _navigateToCommand = new ActionCommand<string>
                    (
                        x => OnNavigateTo(x),
                        x => CanNavigate()
                    );
                }

                return _navigateToCommand;
            }
        }

        #endregion

        #region · Properties ·

        #region · FirstGeneration ·

        /// <summary>
        /// Returns a read-only collection containing the first application option
        /// in the tree, to which the TreeView can bind.
        /// </summary>
        public ReadOnlyCollection<NavigationNode> Functions
        {
            get
            {
                if (String.IsNullOrEmpty(_filterText))
                {
                    return _functions;
                }
                else
                {
                    return new ReadOnlyCollection<NavigationNode>(_filteredFunctions);
                }
            }
        }

        #endregion

        #region · FilterText ·

        /// <summary>
        /// Gets/sets a fragment of the name to filter for.
        /// </summary>
        public string FilterText
        {
            get { return _filterText; }
            set
            {
                if (value != _filterText)
                {
                    _filterText = value;
                    this.PerformFilter();
                }
            }
        }

        #endregion

        #endregion

        #region · Constructor ·

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigatorWidget"/> class.
        /// </summary>
        public NavigatorWidgetViewModel()
            : base()
        {
            if (!DesignMode.IsInDesignMode)
            {
                _rootOption = SiteMapService.SiteMap.RootNode;
                _filteredFunctions = new List<NavigationNode>();
                _functions = new ReadOnlyCollection<NavigationNode>(_rootOption.ChildNodes.OfType<NavigationNode>().ToArray());
            }
        }

        #endregion

        #region · Command Actions ·

        /// <summary>
        /// Returns a value indcating whether the navigation command can be executed
        /// </summary>
        /// <returns></returns>
        private bool CanNavigate()
        {
            return _filteredFunctions != null && _filteredFunctions.Count == 1;
        }

        /// <summary>
        /// Handles the navigation command
        /// </summary>
        private void OnNavigateTo(string url)
        {
            ServiceLocator.GetService<INavigationService>().Navigate(url);
        }

        #endregion

        #region · Search Logic ·

        private void PerformFilter()
        {
            this.VerifyMatchingOptionEnumerator();
        }

        private void VerifyMatchingOptionEnumerator()
        {
            // Clear current matching options
            _filteredFunctions.Clear();

            // Perform filter if needed
            if (!String.IsNullOrEmpty(_filterText))
            {
                _filteredFunctions.AddRange(this.FindMatches(_filterText));
            }

            // Notify changes
            this.NotifyPropertyChanged(() => Functions);
        }

        private List<NavigationNode> FindMatches(string filterText)
        {
            var nodes = from node in _rootOption
                            .ChildNodes
                            .OfType<NavigationNode>()
                            .Traverse<NavigationNode>(node => ((node.ChildNodes) != null ? node.ChildNodes.OfType<NavigationNode>() : null))
                        where node.Title.StartsWith(filterText, StringComparison.OrdinalIgnoreCase) && (node.ChildNodes == null || node.ChildNodes.Count == 0)
                        select node;

            return nodes.ToList();
        }

        #endregion
    }
}
