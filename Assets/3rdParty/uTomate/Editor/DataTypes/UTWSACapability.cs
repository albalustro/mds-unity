//
// Copyright (c) 2013-2016 Ancient Light Studios
// All Rights Reserved
// 
// http://www.ancientlightstudios.com
//

#if !UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2 // VR: 5.3

namespace AncientLightStudios.uTomate
{
    using System;
    using API;

    /// <summary>
    /// Enum type that wraps around <see cref="UTSetPlayerSettingsWindowsStoreAction.WSACapability"/>
    /// </summary>
    [Serializable]
    public class UTWSACapability : UTEnum<UTSetPlayerSettingsWindowsStoreAction.WSACapability>
    {
    }
}

#endif
