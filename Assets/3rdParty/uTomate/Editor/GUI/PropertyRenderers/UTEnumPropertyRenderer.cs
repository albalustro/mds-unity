//
// Copyright (c) 2013-2016 Ancient Light Studios
// All Rights Reserved
// 
// http://www.ancientlightstudios.com
//

namespace AncientLightStudios.uTomate
{
    using System;
    using System.Linq;
    using API;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    ///  Renderer for enum values.
    /// </summary>
    [UTPropertyRenderer(typeof(Enum), typeof(UTEnum<>))]
    public class UTEnumPropertyRenderer : UTIPropertyRenderer
    {
        public void Render(UTFieldWrapper wrapper)
        {
            if (!wrapper.InspectorHint.multiSelect)
            {
                wrapper.Value = EnumPopup(wrapper.Label, (Enum)wrapper.Value, GetAllowedValues(wrapper), wrapper.InspectorHint.captions);
            }
            else
            {
                wrapper.Value = EnumMask(wrapper.Label, (Enum) wrapper.Value, GetAllowedValues(wrapper), wrapper.InspectorHint.captions);
            }
        }

        private string[] GetAllowedValues(UTFieldWrapper wrapper)
        {
            if (wrapper.RendererDelegate != null)
            {
                var allowedValues = wrapper.RendererDelegate.AllowedValues(wrapper.FieldName);
                if (allowedValues != null)
                {
                    return allowedValues;
                }
            }
            return wrapper.InspectorHint.allowedValues;
        }

        private static Enum EnumPopup(GUIContent label, Enum selected, string[] allowedValues, string[] replacementCaptions)
        {
            return RenderEnum(label, selected, allowedValues, replacementCaptions, false, (myLabel, mySelectedIndex, captions) =>
                EditorGUILayout.Popup(myLabel ?? GUIContent.none, mySelectedIndex, captions.Select(x => new GUIContent(x)).ToArray())
                );
        }

        private static Enum EnumMask(GUIContent label, Enum selected, string[] allowedValues, string[] replacementCaptions)
        {
            return EditorGUILayout.EnumMaskField(label ?? GUIContent.none, selected); // no custom captions for multiselect fields for now..
        }

        private static Enum RenderEnum(GUIContent label, Enum selected, string[] allowedValues, string[] replacementCaptions, bool multiSelect, Func<GUIContent,int,string[],int> renderAction)
        {
            var type = selected.GetType();
            string[] names;
            if (allowedValues == null || allowedValues.Length == 0)
            {
                names = Enum.GetNames(type);
            }
            else
            {
                names = allowedValues;
            }

            var captions = new string[names.Length];
            for (var i = 0; i < names.Length; i++)
            {
                if (replacementCaptions != null && replacementCaptions.Length > i)
                {
                    captions[i] = replacementCaptions[i];
                }
                else
                {
                    captions[i] = ObjectNames.NicifyVariableName(names[i]);
                }
            }


            var selectedIndex = Array.IndexOf<string>(names, Enum.GetName(type, (object)selected));
            if (selectedIndex == -1)
            {
                selectedIndex = 0;
            }

            var index = renderAction(label, selectedIndex, captions);
            
            if (index < 0 || index >= names.Length)
            {
                return selected;
            }
            return (Enum)Enum.Parse(type, names[index]);
        }
    }
}

