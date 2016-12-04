//
// Copyright (c) 2013-2016 Ancient Light Studios
// All Rights Reserved
// 
// http://www.ancientlightstudios.com
//

#if !UNITY_5_0 && !UNITY_5_1 // VR: 5.2
namespace AncientLightStudios.uTomate
{
    using API;
    using System;
    using UnityEditor;

    /// <summary>
    /// Enum type that wraps around <see cref="VertexChannelCompressionFlags"/>
    /// </summary>
    [Serializable]
    public class UTVertexChannelCompressionFlags : UTEnum<VertexChannelCompressionFlags>
    {
    }
}
#endif