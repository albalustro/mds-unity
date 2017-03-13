
namespace AncientLightStudios.uTomate
{
    using System;
    using API;
#if !(UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4) // VR: 5.5
    using UnityEditor;
#endif
    /// <summary>
    /// Enum type that wraps around <see cref="WebGLExceptionSupport"/>
    /// </summary>
    [Serializable]
    public class UTWebGLExceptionSupport : UTEnum<WebGLExceptionSupport>
    {
    }

#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4 // VR: [5.0,5.4]
    public enum WebGLExceptionSupport
    {
        None,
        ExplicitlyThrownExceptionsOnly,
        Full,
    }
#endif
}
