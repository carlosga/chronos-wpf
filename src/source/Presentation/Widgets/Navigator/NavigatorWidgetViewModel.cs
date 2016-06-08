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

        private readonly ReadOnlyCollection<NavigationNode> functions;
        private readonly SiteMapNode                        rootOption;
        private List<NavigationNode>                        filteredFunctions;
        private string                                      filterText;
        private ICommand                                    navigateToCommand;

        #endregion

        #region · Commands ·

        /// <summary>
        /// Gets the navigation command
        /// </summary>
        public ICommand NavigateToCommand
        {
            get
            {
                if (this.navigateToCommand == null)
                {
                    this.navigateToCommand = new ActionCommand<string>
                    (
                        x => OnNavigateTo(x),
                        x => CanNavigate()
                    );
                }

                return this.navigateToCommand;
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
                if (String.IsNullOrEmpty(this.filterText))
                {
                    return this.functions;
                }
                else
                {
                    return new ReadOnlyCollection<NavigationNode>(this.filteredFunctions);
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
            get { return this.filterText; }
            set
            {
                if (value != filterText)
                {
                    this.filterText = value;
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
                this.rootOption         = SiteMapService.SiteMap.RootNode;
                this.filteredFunctions  = new List<NavigationNode>();
                this.functions          = new ReadOnlyCollection<NavigationNode>(this.rootOption.ChildNodes.OfType<NavigationNode>().ToArray());
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
            return this.filteredFunctions != null && this.filteredFunctions.Count == 1;
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
            this.filteredFunctions.Clear();

            // Perform filter if needed
            if (!String.IsNullOrEmpty(this.filterText))
            {
                this.filteredFunctions.AddRange(this.FindMatches(this.filterText));
            }

            // Notify changes
            this.NotifyPropertyChanged(() => Functions);
        }

        private List<NavigationNode> FindMatches(string filterText)
        {
            var nodes = from node in this.rootOption
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
