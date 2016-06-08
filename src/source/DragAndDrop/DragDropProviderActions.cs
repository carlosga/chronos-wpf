// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Chronos.Presentation.DragAndDrop
{
    [Serializable]
    public enum DragDropProviderActions
    {
        None            = 0,
        Data            = 1,
        Visual          = 2,
        Feedback        = 4,
        ContinueDrag    = 8,
        Clone           = 16,
        MultiFormatData = 32,
        // 64, 128  left for decent operations 
        // unparent feels hacky 
        Unparent        = 256,
    } 
}
