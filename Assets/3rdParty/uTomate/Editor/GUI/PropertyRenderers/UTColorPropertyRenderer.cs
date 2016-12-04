//
// Copyright (c) 2013-2016 Ancient Light Studios
// All Rights Reserved
// 
// http://www.ancientlightstudios.com
//

namespace AncientLightStudios.uTomate
{
    using API;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    ///  Renderer for color values.
    /// </summary>
    [UTPropertyRenderer(typeof(Color), typeof(UTColor))]
    public class UTColorPropertyRenderer : UTIPropertyRenderer
    {
        public void Render(UTFieldWrapper wrapper)
        {
            if (wrapper.Label != null)
            {
                wrapper.Value = EditorGUILayout.ColorField(wrapper.Label, (Color)wrapper.Value);
            }
            else
            {
                wrapper.Value = EditorGUILayout.ColorField((Color)wrapper.Value);
            }
        }
    }
}
