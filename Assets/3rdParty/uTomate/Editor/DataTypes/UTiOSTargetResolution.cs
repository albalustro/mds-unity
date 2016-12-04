//
// Copyright (c) 2013-2016 Ancient Light Studios
// All Rights Reserved
// 
// http://www.ancientlightstudios.com
//

#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 // VR [5.0, 5.2]
namespace AncientLightStudios.uTomate
{
    using API;
    using System;
    using UnityEditor;

    /// <summary>
    /// Enum type that wraps around <see cref="iOSTargetResolution"/>
    /// </summary>
    [Serializable]
    public class UTiOSTargetResolution : UTEnum<iOSTargetResolution>
    {
    }
}
#endif
