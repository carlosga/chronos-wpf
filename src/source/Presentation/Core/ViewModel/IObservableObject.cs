// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel;

namespace Chronos.Presentation.Core.ViewModel
{
    /// <summary>
    /// Interface for observable object implementations
    /// </summary>
    public interface IObservableObject
        : INotifyPropertyChanged
    {
    }
}
