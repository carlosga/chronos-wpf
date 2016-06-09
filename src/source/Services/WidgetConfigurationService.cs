// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Configuration;
using Chronos.Configuration;
using Chronos.Presentation.Core.Configuration;
using Chronos.Presentation.Core.Widgets;
using NLog;
using nRoute.Components.Composition;
using nRoute.Services;

namespace Chronos.Services
{
    [MapService(typeof(IWidgetConfigurationService), InitializationMode = InitializationMode.OnDemand, Lifetime = InstanceLifetime.Singleton)]
    public sealed class WidgetConfigurationService
        : IWidgetConfigurationService
    {
        private static Logger s_logger = LogManager.GetCurrentClassLogger();

        private static readonly object s_syncObject = new object();

        private static readonly string s_rootConfigFile = "Widgets.config";
        private static readonly string s_rootSection = "chronoserp.widgets";

        private List<IWidget> _widgets;

        public WidgetConfigurationService()
        {
        }

        public IEnumerable<IWidget> GetWidgets()
        {
            lock (s_syncObject)
            {
                if (_widgets == null)
                {
                    var section = ConfigurationManager.GetSection(s_rootSection) as WidgetsSectionHandler;

                    if (section != null)
                    {
                        _widgets = new List<IWidget>();

                        foreach (WidgetConfigurationElement widgetConfig in section.Widgets)
                        {
                            IWidget widget = this.CreateWidgetDefinition(widgetConfig);

                            if (widget == null)
                            {
                                s_logger.Debug("Widget not found {0} - {1}", widgetConfig.Id, widgetConfig.Type);
                            }
                            else
                            {
                                _widgets.Add(widget);
                            }
                        }
                    }
                }
            }

            return _widgets;
        }

        public T GetWidgetConfigurationSection<T>(string sectionName)
            where T : ConfigurationSection
        {
            throw new NotImplementedException();
        }

        private IWidget CreateWidgetDefinition(WidgetConfigurationElement widgetConfig)
        {
            IWidget widget = null;

            try
            {
                widget = Activator.CreateInstance(Type.GetType(widgetConfig.Type)) as IWidget;
            }
            catch (Exception ex)
            {
                s_logger.ErrorException
                (
                    String.Format("Widget not found {0} - {1}", widgetConfig.Id, widgetConfig.Type),
                    ex
                );
            }

            return widget;
        }
    }
}
