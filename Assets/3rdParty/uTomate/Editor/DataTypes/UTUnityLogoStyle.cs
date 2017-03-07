//
// Copyright (c) 2013-2016 Ancient Light Studios
// All Rights Reserved
// 
// http://www.ancientlightstudios.com
//

#if !(UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4) // VR: 5.5
namespace AncientLightStudios.uTomate
{
    using System;
    using API;
    using UnityEditor;

    /// <summary>
    /// Enum wrapper around <see cref="PlayerSettings.SplashScreen.UnityLogoStyle"/>
    /// </summary>
    [Serializable]
    public class UTUnityLogoStyle : UTEnum<PlayerSettings.SplashScreen.UnityLogoStyle>
    {
    }
}
#endif
