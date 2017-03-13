//
// Copyright (c) 2013-2016 Ancient Light Studios
// All Rights Reserved
// 
// http://www.ancientlightstudios.com
//
namespace AncientLightStudios.uTomate
{
    using API;
    using System;
#if !(UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4) // VR: 5.5
    using UnityEditor;
#endif

    /// <summary>
    /// Enum wrapper around <see cref="StereoRenderingPath"/>
    /// </summary>
    [Serializable]
    public class UTStereoRenderingPath : UTEnum<StereoRenderingPath>
    {
    }


#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4 // VR: [5.0,5.4]
    public enum StereoRenderingPath
    {
        MultiPass,
        SinglePass
    }
#endif
}
