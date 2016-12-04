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

    [UTPropertyRenderer(typeof(UTLayer))]
    public class UTLayerPropertyRenderer : UTIPropertyRenderer
    {
        public void Render(UTFieldWrapper fieldWrapper)
        {
            if (fieldWrapper.Label != null)
            {
                fieldWrapper.Value = EditorGUILayout.LayerField(fieldWrapper.Label, (int)fieldWrapper.Value);
            }
            else
            {
                fieldWrapper.Value = EditorGUILayout.LayerField((int)fieldWrapper.Value);
            }
        }
    }
}
