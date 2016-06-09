// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Configuration;
using Chronos.Presentation.Core.Widgets;

namespace Chronos.Presentation.Core.Configuration
{
    public interface IWidgetConfigurationService
    {
        IEnumerable<IWidget> GetWidgets();

        T GetWidgetConfigurationSection<T>(string sectionName)
            where T : ConfigurationSection;
    }
}
