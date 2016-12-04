//
// Copyright (c) 2013-2016 Ancient Light Studios
// All Rights Reserved
// 
// http://www.ancientlightstudios.com
//

namespace AncientLightStudios.uTomate
{
    public interface UTConfigurableProperty
    {
        string Name { get; set; }

        string Value { get; set; }

        string Expression { get; set; }

        bool UseExpression { get; set; }

        bool IsMachineSpecific { get; set; }

        bool IsPrivate { get; set; }

        bool SupportsPrivate { get; }
    }
}
