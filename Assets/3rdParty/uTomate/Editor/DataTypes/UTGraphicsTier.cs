//
// Copyright (c) 2013-2016 Ancient Light Studios
// All Rights Reserved
// 
// http://www.ancientlightstudios.com
//

#if !(UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4) // VR: 5.5
namespace AncientLightStudios.uTomate
{
    using API;
    using System;
    using UnityEngine.Rendering;

    /// <summary>
    /// Enum type that wraps around <see cref="GraphicsTier"/>
    /// </summary>
    [Serializable]
    public class UTGraphicsTier : UTEnum<GraphicsTier>
    {
    }
}
#endif