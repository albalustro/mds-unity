using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Linq;
using System;

namespace Playmove
{
    [CustomPropertyDrawer(typeof(PYBundleAssetTag))]
    public class PYBundleAssetTagDrawer : PropertyDrawer
    {
        private string _currentType;

        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // Calculate rects
            var tagButtonRect = new Rect(position.x, position.y, position.width, position.height);

            SerializedProperty unprocessedTag = property.FindPropertyRelative("UnprocessedTag");
            string tag = string.IsNullOrEmpty(unprocessedTag.stringValue) ?
                "None" : unprocessedTag.stringValue.Split(':')[0];

            if (GUI.Button(tagButtonRect, tag))
            {
                string[] tags = PYBundleManager.GetAssetsTag();

                _currentType = PYBundleManager.GetTypeByRealType(property.serializedObject.targetObject);
                if (!string.IsNullOrEmpty(_currentType))
                    tags = tags.Where(FilterTag).ToArray();

                // Create a description for each tag, if exist any
                string[] tagsDescriptions = new string[tags.Length];
                for (int x = 0; x < tags.Length; x++)
                {
                    string[] tagSplit = tags[x].Split(new string[] { "<||>" }, StringSplitOptions.None);

                    if (tagSplit.Length > 1)
                    {
                        tags[x] = tagSplit[0];
                        tagsDescriptions[x] = tagSplit[1];
                    }
                    else
                        tagsDescriptions[x] = "";
                }

                PYSelectorWindow.Init(tagButtonRect, tag, tags,
                    tagsDescriptions, (selectedItem) =>
                {
                    unprocessedTag.stringValue = selectedItem.Value;

                    property.serializedObject.ApplyModifiedProperties();
                    property.serializedObject.Update();
                });
            }

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }

        bool FilterTag(string tag)
        {
            if (tag.Contains(_currentType))
            {
                if (_currentType == "Text")
                {
                    if (!tag.EndsWith("TextAsset"))
                        return true;
                    else
                        return false;
                }
                return true;
            }
            else
                return false;
        }

        int FindTagId(string[] tags, string tag)
        {
            for (int x = 0; x < tags.Length; x++)
            {
                if (tags[x] == tag)
                    return x;
            }
            return 0;
        }
    }
}