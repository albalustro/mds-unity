//
// Copyright (c) 2013-2016 Ancient Light Studios
// All Rights Reserved
// 
// http://www.ancientlightstudios.com
//

#if !(UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3) // VR: 5.4
namespace AncientLightStudios.uTomate
{
    using System;
    using API;
    using UnityEditor;

    /// <summary>
    /// Enum wrapper around <see cref="SplashScreenStyle"/>
    /// </summary>
    [Serializable]
    public class UTSplashScreenStyle : UTEnum<SplashScreenStyle>
    {
    }
}
#endif
