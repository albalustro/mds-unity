//
// Copyright (c) 2013-2016 Ancient Light Studios
// All Rights Reserved
// 
// http://www.ancientlightstudios.com
//

namespace AncientLightStudios.uTomate.API
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using UnityEditor;
    using UnityEditor.AnimatedValues;
    using UnityEngine;
    using UObject = UnityEngine.Object;

    /// <summary>
    ///  Inspector-Renderer for our objects. Moved to a separate class so it can be used by the default
    /// inspectors and the "inspector" in our editor.
    /// </summary>
    public class UTInspectorRenderer
    {
        private const string BaseHelpUrl = "http://www.ancientlightstudios.com/utomate/documentation/actiondocs";
        private static GUIContent emptyLabel = new GUIContent(" ");
        private readonly Dictionary<string, AnimBool>  flagCache = new Dictionary<string, AnimBool>();

        // these will be re-used in the code so we don't have to generate a ton of wrapper objects per draw call.
        private UTPropertyFieldWrapper propertyFieldWrapper = new UTPropertyFieldWrapper();
        private UTPlainFieldWrapper plainFieldWrapper = new UTPlainFieldWrapper();
        private UTPlainArrayMemberWrapper arrayMemberWrapper = new UTPlainArrayMemberWrapper();

        /// <summary>
        /// A renderer delegate which can be used to modify the way in which properties are rendered.
        /// </summary>
        public UTInspectorRendererDelegate rendererDelegate;

        /// <summary>
        /// The validity checker. Uses UTDefaultFieldValidityChecker by default but can be replaced.
        /// </summary>
        public UTFieldValidityChecker fieldValidityChecker = new UTDefaultFieldValidityChecker();

        /// <summary>
        /// The object that should be rendered.
        /// </summary>
        public UObject target;

        /// <summary>
        /// The editor that owns this renderer.
        /// </summary>
        private readonly Editor owningEditor;

        /// <summary>
        /// The editor window that owns this renderer.
        /// </summary>
        private readonly EditorWindow owningEditorWindow;

        /// <param name="owningEditor">The editor that owns this renderer. Used to issue repaint calls.</param>
        public UTInspectorRenderer(Editor owningEditor)
        {
            this.owningEditor = owningEditor;
        }

        /// <param name="owningEditorWindow">The editor window that owns this renderer. Used to issue repaint calls.</param>
        public UTInspectorRenderer(EditorWindow owningEditorWindow)
        {
            this.owningEditorWindow = owningEditorWindow;
        }


        private void Repaint()
        {
            if (owningEditor != null)
            {
                owningEditor.Repaint();
            }

            if (owningEditorWindow != null)
            {
                owningEditorWindow.Repaint();
            }
        }

        /// <summary>
        /// Does the actual rendering. Call from the OnInspectorGUI event in your Unity editor.
        /// </summary>
        public void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical();
            EditorGUI.BeginChangeCheck();
            Undo.RecordObject(target, "inspector change");
            DrawAll();
            if (EditorGUI.EndChangeCheck())
            {
                // no replacement for 4.3+
                EditorUtility.SetDirty(target);
            }
            EditorGUILayout.EndVertical();
        }

        protected virtual void DrawAll()
        {
            var type = target.GetType();
            var fields = type.GetFields();
            var inspectorGroups = UTInspectorGroups.GetFor(type);
            var groups = inspectorGroups.groups;
            Array.Sort(fields, (field1, field2) => SortFields(field1, field2, groups));

            var firstGroup = true;
            var currentGroup = "";

            RenderActionHead(type);
            foreach (var field in fields)
            {
                if (field.IsPublic && !field.IsStatic)
                {
                    if (!IsVisible(field))
                    {
                        continue; // skip it.
                    }

                    var utDoc = UTDoc.GetFor(field);
                    var utHint = UTInspectorHint.GetFor(field);
                    var label = new GUIContent(utDoc.title + (utHint.required || utHint.arrayNotEmpty ? " *" : ""), utDoc.description);

                    if (utHint.group != currentGroup || firstGroup)
                    {
                        firstGroup = false;
                        currentGroup = utHint.group;

                        EditorGUILayout.Space();
                        if (!string.IsNullOrEmpty(currentGroup))
                        {
                            GUILayout.Label(currentGroup, UTEditorResources.GroupStyle);
                            EditorGUILayout.Space();
                        }
                    }

                    if (!string.IsNullOrEmpty(utHint.subSectionHeader))
                    {
                        if (utHint.HasFixedLength)
                        {
                            var cacheKey = currentGroup + utHint.subSectionHeader;
                            EnsureFlagCacheExists(cacheKey);
                            flagCache[cacheKey].value = EditorGUILayout.Foldout(flagCache[cacheKey].value, utHint.subSectionHeader);
                            if (!flagCache[cacheKey].value) // foldout is closed, don't render the field
                            {
                                continue;
                            }

                        }
                        else
                        {
                            GUILayout.Label(utHint.subSectionHeader, EditorStyles.boldLabel);
                        }
                    }

                    string msg;
                    if (!UTRequiresLicenseAttribute.HasRequiredLicense(field, out msg))
                    {
                        // disable fields which require licenses that are not currently installed
                        GUI.enabled = false;
                    }
                    EditorGUI.indentLevel += utHint.indentLevel;
                    DrawProperty(label, field);
                    EditorGUI.indentLevel -= utHint.indentLevel;
                    GUI.enabled = true;
                }
            }
        }


        protected void RenderActionHead(Type type)
        {
            // new type
            var utDoc = UTDoc.GetFor(type);
            var utDefaultAction = UTDefaultAction.GetFor(type);

            var title = new GUIContent(utDoc.title);
            GUILayout.Label(title, UTEditorResources.TitleStyle);

            var url = utDoc.helpUrl;
            if (string.IsNullOrEmpty(url) && utDefaultAction != null)
            {
                var html = Regex.Replace(utDoc.title, "[^a-zA-Z0-9]", "_").ToLower() + ".html";
                url = "/" + html;
            }

            if (!string.IsNullOrEmpty(url))
            {
                var titleRect = GUILayoutUtility.GetLastRect();
                var helpRect = new Rect(titleRect.xMax - 20, titleRect.yMin, 20, titleRect.height);
                if (GUI.Button(helpRect, UTEditorResources.HelpIcon, UTEditorResources.HelpButtonStyle))
                {
                    OpenHelp(url);
                }
            }

            if (!string.IsNullOrEmpty(utDoc.description))
            {
                EditorGUILayout.HelpBox(utDoc.description, MessageType.None, true);
            }

            string msg;
            if (!UTRequiresLicenseAttribute.HasRequiredLicense(type, out msg))
            {
                EditorGUILayout.HelpBox("This action requires the following licenses that are not currently installed: " + msg, MessageType.Warning, true);
            }
        }

        private void OpenHelp(string helpUrl)
        {
            if (!helpUrl.StartsWith("https://") && !helpUrl.StartsWith("http://"))
            {
                helpUrl = BaseHelpUrl + helpUrl;
            }

            Help.BrowseURL(helpUrl);
        }

        /// <summary>
        /// Checks if the given field should be rendered or not.
        /// </summary>
        protected virtual bool IsVisible(FieldInfo field)
        {
            var delegateResult = rendererDelegate != null ? rendererDelegate.IsVisible(field) : UTVisibilityDecision.Undetermined;
            if (delegateResult == UTVisibilityDecision.Undetermined)
            {
                var hide = field.GetCustomAttributes(typeof(HideInInspector), true);
                return hide.Length == 0;
            }
            return delegateResult == UTVisibilityDecision.Visible;
        }

        protected virtual void DrawProperty(GUIContent label, FieldInfo field)
        {
            var fieldValue = field.GetValue(target);

            if (field.FieldType.IsArray)
            {
                var hint = UTInspectorHint.GetFor(field);


                var objArray = (object[])fieldValue;
                var elementType = field.FieldType.GetElementType();

                // initialize array fields with empty arrays.
                if (objArray == null)
                {
                    objArray = (object[])Array.CreateInstance(elementType, 0);
                    field.SetValue(target, objArray);
                    EditorUtility.SetDirty(target);
                }

                // array size has changed (maybe action was updated)
                if (hint.HasFixedLength && objArray.Length != hint.fixedLength)
                {
                    var clone = (object[]) Array.CreateInstance(elementType, hint.fixedLength);
                    // copy existing members over
                    Array.Copy(objArray, clone, Math.Min(hint.fixedLength, objArray.Length));
                    // fill remaining length with new items (if any)
                    for (var i = objArray.Length; i < hint.fixedLength; i++)
                    {
                        if (elementType.IsSubclassOf(typeof (UTPropertyBase)))
                        {
                            clone[i] = Activator.CreateInstance(elementType);
                        }
                    }
                    field.SetValue(target, clone);
                    EditorUtility.SetDirty(target);
                }

                // only draw modification buttons when array is not fixed 
                if (!hint.HasFixedLength)
                {
                    EditorGUILayout.BeginHorizontal();
                    var arrayInfo = new GUIContent(objArray.Length + (objArray.Length == 1 ? " item" : " items"));
                    EditorGUILayout.LabelField(label, arrayInfo, UTEditorResources.ArrayLabelStyle);
                    if (GUILayout.Button("Add", UTEditorResources.ExpressionButtonStyle))
                    {
                        var newArray = (object[]) Array.CreateInstance(elementType, objArray.Length + 1);
                        Array.Copy(objArray, newArray, objArray.Length);
                        if (elementType.IsSubclassOf(typeof (UTPropertyBase)))
                        {
                            newArray[newArray.Length - 1] = Activator.CreateInstance(elementType);
                        }
                        field.SetValue(target, newArray);
                        GUI.changed = true;
                        objArray = newArray;
                    }

                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.BeginVertical();

                // the index of the element that should be deleted.
                // since the user cannot click multiple buttons at once
                // a single int is enough here.
                var deleteIndex = -1;
                for (var i = 0; i < objArray.Length; i++)
                {
                    var arrayMemberLabel = emptyLabel;

                    if (hint.HasFixedLength && hint.captions != null && hint.captions.Length > i)
                    {
                        arrayMemberLabel = new GUIContent(hint.captions[i]);
                    }

                    if (elementType.IsSubclassOf(typeof(UTPropertyBase)))
                    {
                        propertyFieldWrapper.SetUp(arrayMemberLabel, field, (UTPropertyBase)objArray[i], rendererDelegate);
                        if (DrawPropertyArrayMember(propertyFieldWrapper))
                        {
                            deleteIndex = i;
                        }
                    }
                    else
                    {
                        arrayMemberWrapper.SetUp(arrayMemberLabel, field, objArray, i, rendererDelegate);
                        if (DrawPropertyArrayMember(arrayMemberWrapper))
                        {
                            deleteIndex = i;
                        }
                    }
                }


                if (!hint.HasFixedLength && deleteIndex != -1)
                {
                    var newArray = (object[])Array.CreateInstance(elementType, objArray.Length - 1);
                    // no copy before index if the index was first element
                    if (deleteIndex > 0)
                    {
                        Array.Copy(objArray, 0, newArray, 0, deleteIndex);
                    }
                    // no copy after index if the index was last element
                    if (deleteIndex + 1 < objArray.Length)
                    {
                        Array.Copy(objArray, deleteIndex + 1, newArray, deleteIndex, objArray.Length - deleteIndex - 1);
                    }
                    field.SetValue(target, newArray);
                    GUI.changed = true;
                }

                // add space only if array isn't empty, looks weird otherwise.
                if (objArray.Length > 0)
                {
                    EditorGUILayout.Space();
                }
                else
                {
                    if (hint.arrayNotEmpty)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PrefixLabel(" ");
                        EditorGUILayout.HelpBox("At least one element is required.", MessageType.Error);
                        EditorGUILayout.EndHorizontal();
                    }
                }

                EditorGUILayout.EndVertical();
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                if (field.FieldType.IsSubclassOf(typeof(UTPropertyBase)))
                {
                    if (fieldValue == null)
                    {
                        fieldValue = Activator.CreateInstance(field.FieldType); // make an empty instance
                        field.SetValue(target, fieldValue);
                        EditorUtility.SetDirty(target);
                    }
                    propertyFieldWrapper.SetUp(label, field, (UTPropertyBase)fieldValue, rendererDelegate);
                    DrawProperty(propertyFieldWrapper, false);
                }
                else
                {
                    plainFieldWrapper.SetUp(label, field, target, rendererDelegate);
                    DrawProperty(plainFieldWrapper, false);
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        protected virtual bool DrawPropertyArrayMember(UTFieldWrapper fieldWrapper)
        {
            EditorGUILayout.BeginHorizontal();
            var delete = DrawProperty(fieldWrapper, !fieldWrapper.InspectorHint.HasFixedLength);
            EditorGUILayout.EndHorizontal();
            return delete;
        }

        protected virtual bool DrawProperty(UTFieldWrapper wrapper, bool removeable)
        {
            var errorMessage = "";
            var propertyIsValid = fieldValidityChecker == null ? UTFieldValidity.Valid :
                fieldValidityChecker.CheckValidity(wrapper, out errorMessage);
            if (propertyIsValid != UTFieldValidity.Valid)
            {
                EditorGUILayout.BeginVertical();
                EditorGUILayout.BeginHorizontal();
            }

            var removed = false;
            if (wrapper.SupportsExpressions)
            {
                if (wrapper.UseExpression)
                {
                    if (wrapper.Label != null)
                    {
                        wrapper.Expression = EditorGUILayout.TextField(wrapper.Label, wrapper.Expression);
                    }
                    else
                    {
                        wrapper.Expression = EditorGUILayout.TextField(wrapper.Expression);
                    }
                }
            }

            if (!wrapper.SupportsExpressions || !wrapper.UseExpression)
            {
                var fieldType = wrapper.FieldType;
                if (fieldType.IsArray)
                {
                    fieldType = fieldType.GetElementType();
                }
                var renderer = UTPropertyRendererRegistry.GetRendererForType(fieldType);
                if (renderer != null)
                {
                    renderer.Render(wrapper);
                }
                else
                {
                    EditorGUILayout.HelpBox("Missing renderer for property " + wrapper.FieldName + " of type " + fieldType.FullName, MessageType.Warning);
                }
            }

            if (removeable)
            {
                if (GUILayout.Button(UTEditorResources.DeleteIcon, UTEditorResources.DeleteButtonStyle))
                {
                    removed = true;
                }
            }

            if (wrapper.SupportsExpressions)
            {
                var oldExpression = wrapper.UseExpression;
                wrapper.UseExpression = GUILayout.Toggle(oldExpression, UTEditorResources.ExpressionButton, UTEditorResources.ExpressionButtonStyle);
                if (oldExpression != wrapper.UseExpression)
                {
                    GUIUtility.keyboardControl = 0; // unfocus textfield so it can update.
                }
            }

            if (propertyIsValid != UTFieldValidity.Valid)
            {
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(" ");
                EditorGUILayout.HelpBox(errorMessage, propertyIsValid == UTFieldValidity.ValidWithWarning ? MessageType.Warning : MessageType.Error);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }

            return removed;
        }

        private void EnsureFlagCacheExists(string currentGroup)
        {
            if (!flagCache.ContainsKey(currentGroup))
            {
                flagCache.Add(currentGroup, new AnimBool(false, Repaint));
            }
        }


        private static int SortFields(FieldInfo field1, FieldInfo field2, string[] groups)
        {
            var hint1 = UTInspectorHint.GetFor(field1);
            var hint2 = UTInspectorHint.GetFor(field2);

            var group1Idx = Array.IndexOf(groups, hint1.group);
            var group2Idx = Array.IndexOf(groups, hint2.group);

            if (group1Idx != -1 && group2Idx != -1)
            {
                if (group1Idx == group2Idx)
                {
                    return hint1.order - hint2.order;
                }
                else
                {
                    return group1Idx - group2Idx;
                }
            }

            if (group1Idx == -1 && group2Idx == -1)
            {
                var groupOrder = string.Compare(hint1.group, hint2.group, StringComparison.InvariantCultureIgnoreCase);
                if (groupOrder == 0)
                {
                    // same group or no group at all
                    return hint1.order - hint2.order;
                }
                // different groups
                // change priority of empty group
                if (string.IsNullOrEmpty(hint1.group))
                {
                    return 1;
                }
                else if (string.IsNullOrEmpty(hint2.group))
                {
                    return -1;
                }
                else
                {
                    return groupOrder;
                }
            }
            else if (group1Idx == -1)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }
    }
}
