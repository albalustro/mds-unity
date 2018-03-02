using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Playmove
{
    [CustomEditor(typeof(PYBundleManager))]
    public class PYBundleManagerEditor : Editor
    {
        private PYBundleManager _target;
        private Rect _rectSelectorWindow;

        void OnEnable()
        {
            _target = (PYBundleManager)target;
        }

        void OnDisable()
        {
            if (PYBundleManager.Instance == null)
                return;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Separator();

            GUI.enabled = true;

            GUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Space(2.5f);

            // Var ExpansionName
            GUILayout.BeginHorizontal();
            GUILayout.Label("Expansion Name");

            if (GUILayout.Button(_target.ExpansionName))
            {
                string[] expansionsName = null;
                try
                {
                    expansionsName = Directory.GetDirectories("Assets/Bundles/Expansions/");
                    expansionsName = PYBundleBuildWindow.GetLastNameFromPaths(new List<string>(expansionsName));
                }
                catch { }
                PYSelectorWindow.Init(_rectSelectorWindow, _target.ExpansionName, expansionsName, (selectedItem) =>
                {
                    _target.ExpansionName = selectedItem.Value;
                });
            }

            if (Event.current.type == EventType.Repaint)
                _rectSelectorWindow = GUILayoutUtility.GetLastRect();
            GUILayout.EndHorizontal();

            // Var Language
            GUILayout.BeginHorizontal();
            GUILayout.Label("Language");

            Rect rectButton = GUILayoutUtility.GetRect(new GUIContent(_target.Language), GUI.skin.button);
            if (GUI.Button(rectButton, _target.Language))
            {
                string[] languagesName = null;
                try
                {
                    languagesName = Directory.GetDirectories("Assets/Bundles/Global/Localization/");
                    languagesName = PYBundleBuildWindow.GetLastNameFromPaths(new List<string>(languagesName));
                }
                catch { }
                PYSelectorWindow.Init(rectButton, _target.Language, languagesName, (selectedItem) =>
                {
                    _target.Language = selectedItem.Value;
                });
            }

            GUILayout.EndHorizontal();

            GUI.enabled = false;
            // Var IsReady
            _target.State = (PYBundleManagerState)EditorGUILayout.EnumPopup("State", _target.State);

            GUILayout.Space(2.5f);
            GUILayout.EndVertical();

            EditorGUILayout.Separator();
            GUI.enabled = true;

            if (GUILayout.Button("Build Bundle Assets Tags"))
                BuildAssetTags();

            EditorUtility.SetDirty(_target);
            EditorUtility.SetDirty(_target.gameObject);

            // Set PlaytableWin32 to dirty too because we change the language variable there
            if (PlaytableWin32.Instance != null)
            {
                EditorUtility.SetDirty(PlaytableWin32.Instance);
                EditorUtility.SetDirty(PlaytableWin32.Instance.gameObject);
            }
        }

        private void BuildAssetTags()
        {
            List<string> filesTag = new List<string>(PYBundleManager.GetAssetsTag());
            List<string> files = new List<string>();

            for (int i = 0; i < filesTag.Count; i++)
            {
                string tag = filesTag[i].Split(':')[0].TrimEnd(' ');
                string type = filesTag[i].Split(':')[1].TrimStart(' ');
                type = type.Split(new string[] { "<||>" }, StringSplitOptions.None)[0];
                files.Add(type + "_" + tag);
            }

            files.Sort();
            files = files.Distinct().ToList();

            string content = "public enum PYBundleTags {\n";
            for (int i = 0; i < files.Count; i++)
                content += "\t" + files[i] + ",\n";
            content += "}\n\n";

            content += "public static class PYBundlesTagsExtension\n{\n";
            content += "\tpublic static string ToStringTag(this PYBundleTags assetTag)\n\t{\n";
            content += "\t\tstring tag = assetTag.ToString();\n\t\ttag = tag.Remove(0, tag.IndexOf('_') + 1);\n";
            content += "\t\treturn tag;\n\t}\n}";

            string assetTag = Directory.GetFiles(Application.dataPath, "PYBundleTags.cs", SearchOption.AllDirectories)[0];
            File.WriteAllText(assetTag, content);
            AssetDatabase.Refresh();
        }
    }
}