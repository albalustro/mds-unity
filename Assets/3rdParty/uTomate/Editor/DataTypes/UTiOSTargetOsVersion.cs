//
// Copyright (c) 2013-2016 Ancient Light Studios
// All Rights Reserved
// 
// http://www.ancientlightstudios.com
//

#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4 // VR: [5.0,5.4]
namespace AncientLightStudios.uTomate
{
    using API;
    using System;
    using UnityEditor;

    /// <summary>
    /// Enum type wrapping around <see cref="iOSTargetOSVersion"/>
    /// </summary>
    [Serializable]
    public class UTiOSTargetOsVersion : UTEnum<iOSTargetOSVersion>
    {
    }
}
#endif