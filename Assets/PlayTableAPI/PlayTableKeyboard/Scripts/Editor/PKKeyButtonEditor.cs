using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;

namespace Playmove
{
    [CustomEditor(typeof(PKKeyButton))]
    [CanEditMultipleObjects]
    public class PKKeyButtonEditor : Editor
    {

        private string keyText;
        private PKKeyButton targetRef;

        void OnEnable()
        {
            targetRef = (PKKeyButton)target;
            targetRef.Graphic = targetRef.transform.Find("KeyGraphic").GetComponentInChildren<SpriteRenderer>();
            if (targetRef.Text != null)
                keyText = targetRef.Text;
        }

        public override void OnInspectorGUI()
        {
            keyText = EditorGUILayout.TextField("Letter", keyText);
            targetRef.Text = keyText;
            if (targetRef.TextComponent != null)
                targetRef.TextComponent.Text = keyText;
            targetRef.name = "Key - " + keyText;

            if (targetRef.gameObject.activeInHierarchy)
            {
                Vector3 graphicScale = targetRef.Graphic.transform.localScale;
                graphicScale = EditorGUILayout.Vector2Field("Key Scale", graphicScale);
                graphicScale.x = Mathf.Clamp(graphicScale.x, 0.1f, Mathf.Infinity);
                graphicScale.y = Mathf.Clamp(graphicScale.y, 0.1f, Mathf.Infinity);
                targetRef.Graphic.transform.localScale = graphicScale;
            }

            if (targetRef.CustomSkin == PlayTableKeyboard.Instance.Skin)
            {
                if (GUILayout.Button("Add Custom Skin"))
                {
                    var path = EditorUtility.SaveFilePanel(
                        "Save Custom Skin",
                        "",
                        targetRef.name + "Skin" + ".asset",
                        "asset");

                    if (path.Length > 0)
                    {
                        string[] temp = path.Split('/');
                        string assetName = "";
                        string assetPath = FilterPathToRelative(path, false);

                        assetName = temp[temp.Length - 1].Split('.')[0];

                        targetRef.CustomSkin = PlaytableKeyboardEditor.CreateSkin(assetName, assetPath);

                        Selection.activeObject = targetRef.CustomSkin;
                    }
                }
            }
            else
            {
                if (GUILayout.Button("Select Custom Skin"))
                {
                    var path = EditorUtility.OpenFilePanel(
                        "Select Custom Skin",
                        Application.dataPath + AssetDatabase.GetAssetPath(targetRef.CustomSkin).Replace("Assets/", ""),
                        "asset");

                    if (path.Length > 0)
                    {
                        targetRef.CustomSkin = (PKKeyboardSkin)AssetDatabase.LoadAssetAtPath(FilterPathToRelative(path, true), typeof(PKKeyboardSkin));
                    }
                }
            }

            GUILayout.Space(10);
            base.OnInspectorGUI();
            EditorUtility.SetDirty(targetRef);
            EditorUtility.SetDirty(targetRef.gameObject);
        }

        string FilterPathToRelative(string path, bool withAssetName)
        {
            string[] temp = path.Split('/');
            string relativePath = "";

            for (int i = 0; i < (withAssetName ? temp.Length : temp.Length - 1); i++)
            {
                if (temp[i] == "Assets" || relativePath.Length > 0)
                    relativePath += temp[i] + "/";
            }
            if (withAssetName && relativePath.EndsWith("/"))
                return relativePath.Remove(relativePath.Length - 1);

            return relativePath;
        }
    }
}