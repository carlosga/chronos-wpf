// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Chronos.Configuration;
using Chronos.Presentation.Core.Configuration;
using Chronos.Presentation.Core.Widgets;
using NLog;
using nRoute.Components.Composition;
using nRoute.Services;
using System.Configuration;

namespace Chronos.Services
{
    [MapService(typeof(IWidgetConfigurationService),
        InitializationMode  = InitializationMode.OnDemand,
        Lifetime            = InstanceLifetime.Singleton)]
    public sealed class WidgetConfigurationService
        : IWidgetConfigurationService
    {
        #region · Logger ·

        private static Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region · SyncObject ·

        private static readonly object SyncObject = new object();

        #endregion

        #region · Constants ·

        private static readonly string RootConfigFile   = "Widgets.config";
        private static readonly string RootSection      = "chronoserp.widgets";

        #endregion

        #region · Fields ·

        private List<IWidget> widgets;

        #endregion

        #region · Constructors ·

        public WidgetConfigurationService()
        {
        }

        #endregion

        #region · Methods ·

        public IEnumerable<IWidget> GetWidgets()
        {
            lock (SyncObject)
            {
                if (this.widgets == null)
                {
                    WidgetsSectionHandler section = ConfigurationManager.GetSection(RootSection) as WidgetsSectionHandler;

                    if (section != null)
                    {
                        this.widgets = new List<IWidget>();

                        foreach (WidgetConfigurationElement widgetConfig in section.Widgets)
                        {
                            IWidget widget = this.CreateWidgetDefinition(widgetConfig);

                            if (widget == null)
                            {
                                Logger.Debug("Widget not found {0} - {1}", widgetConfig.Id, widgetConfig.Type);
                            }
                            else
                            {
                                this.widgets.Add(widget);
                            }
                        }
                    }
                }
            }

            return this.widgets;
        }

        public T GetWidgetConfigurationSection<T>(string sectionName)
            where T: ConfigurationSection
        {
            throw new NotImplementedException();
        }

        #endregion

        #region · Private Methods ·
        
        private IWidget CreateWidgetDefinition(WidgetConfigurationElement widgetConfig)
        {
            IWidget widget = null;

            try
            {
                widget = Activator.CreateInstance(Type.GetType(widgetConfig.Type)) as IWidget;
            }
            catch (Exception ex)
            {
                Logger.ErrorException
                (
                    String.Format("Widget not found {0} - {1}", widgetConfig.Id, widgetConfig.Type),
                    ex
                );
            }

            return widget;
        }

        #endregion
    }
}
