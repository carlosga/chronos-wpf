// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Windows.Input;
using Chronos.Presentation.Core.VirtualDesktops;
using Chronos.Presentation.Core.Widgets;
using Chronos.Presentation.Core.Windows;
using Chronos.Presentation.ViewModel;
using nRoute.Components;

namespace Chronos.WidgetLibrary
{
    /// <summary>
    /// Widget Definition ViewModel
    /// </summary>
    public sealed class WidgetItemViewModel
        : ViewModelBase, IWidget
    {
        #region · Fields ·

        private IWidget     widgetDefinition;
        private ICommand    createWidgetCommand;

        #endregion

        #region · Commands ·

        /// <summary>
        /// Gets the create widget command.
        /// </summary>
        /// <value>The create widget command.</value>
        public ICommand CreateWidgetCommand
        {
            get
            {
                if (this.createWidgetCommand == null)
                {
                    this.createWidgetCommand = new ActionCommand(() => OnCreateWidget());
                }

                return this.createWidgetCommand;
            }
        }

        #endregion

        #region · Properties ·

        /// <summary>
        /// Gets the widget title
        /// </summary>
        /// <value></value>
        public string Title
        {
            get { return this.widgetDefinition.Title; }
        }

        /// <summary>
        /// Gets the widget description
        /// </summary>
        /// <value></value>
        public string Description
        {
            get { return this.widgetDefinition.Description; }
        }

        /// <summary>
        /// Gets the widget group
        /// </summary>
        /// <value></value>
        public string Group
        {
            get { return this.widgetDefinition.Group; }
        }

        /// <summary>
        /// Gets the widget icon style
        /// </summary>
        /// <value></value>
        public string IconStyle
        {
            get { return this.widgetDefinition.IconStyle; }
        }

        #endregion

        #region · Constructors ·

        /// <summary>
        /// Initializes a new instance of the <see cref="WidgetItemViewModel"/> class.
        /// </summary>
        /// <param name="widget">The widget.</param>
        public WidgetItemViewModel(IWidget widgetDefinition)
            : base()
        {
            this.widgetDefinition = widgetDefinition;
        }

        #endregion

        #region · IWidget Members ·

        /// <summary>
        /// Creates the widget view
        /// </summary>
        /// <returns></returns>
        System.Windows.FrameworkElement IWidget.CreateView()
        {
            return this.widgetDefinition.CreateView();
        }

        #endregion

        #region · Command Actions ·

        /// <summary>
        /// Handles the create widget command.
        /// </summary>
        private void OnCreateWidget()
        {
            this.GetService<IVirtualDesktopManager>()
                .Show(this.widgetDefinition.CreateView() as IDesktopElement);
        }

        #endregion
    }
}
