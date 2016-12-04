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

    [UTPropertyRenderer(typeof(UTTag))]
    public class UTTagPropertyRenderer : UTIPropertyRenderer
    {
        public void Render(UTFieldWrapper fieldWrapper)
        {
            if (fieldWrapper.Label != null)
            {
                fieldWrapper.Value = EditorGUILayout.TagField(fieldWrapper.Label, (string)fieldWrapper.Value);
            }
            else
            {
                fieldWrapper.Value = EditorGUILayout.TagField((string)fieldWrapper.Value);
            }
        }
    }
}
