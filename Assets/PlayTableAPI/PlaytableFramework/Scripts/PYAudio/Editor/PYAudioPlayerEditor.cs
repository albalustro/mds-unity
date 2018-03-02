using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Playmove
{
    [CustomEditor(typeof(PYAudioPlayer))]
    public class PYAudioPlayerEditor : Editor
    {
        private const string AUDIO_ITEM_NAME = "{0}: {1}";

        private PYAudioPlayer _target;
        private Rect _rectButtonTag;

        void OnEnable()
        {
            _target = (PYAudioPlayer)target;

            foreach (PYPlayer player in _target.Audios)
                player.InitializeInspector();
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Separator();

            _target.PlayOnEnable = EditorGUILayout.Toggle("Play On Enable", _target.PlayOnEnable);
            _target.PlayOnStart = EditorGUILayout.Toggle("Play On Start", _target.PlayOnStart);
            _target.DelayStartEnable = EditorGUILayout.FloatField("Delay Start Enable", _target.DelayStartEnable);

            #region Add AudioTag or AudioClip
            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add AudioTag"))
            {
                Undo.RecordObject(target, "Add AudioTag");
                PYPlayerTag playerTag = new PYPlayerTag();
                playerTag.Tag = (PYAudioTags)Enum.Parse(typeof(PYAudioTags), "None");
                _target.Audios.Add(playerTag);

                EditorUtility.SetDirty(_target);
            }
            if (GUILayout.Button("Add AudioClip"))
            {
                Undo.RecordObject(target, "Add AudioClip");
                _target.Audios.Add(new PYPlayerClip());

                EditorUtility.SetDirty(_target);
            }
            EditorGUILayout.EndHorizontal();
            #endregion

            #region Show all and Hide all
            GUI.enabled = _target.Audios.Count > 0;
            GUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Show All"))
                ShowElementsProperties(true);
            if (GUILayout.Button("Hide All"))
                ShowElementsProperties(false);
            EditorGUILayout.EndHorizontal();
            GUI.enabled = true;
            #endregion

            GUILayout.Space(10);
            foreach (PYPlayer player in _target.Audios.GetRange(0, _target.Audios.Count))
                ShowPYPlayer(player);

            if (GUI.changed)
                EditorUtility.SetDirty(target);
        }

        void ShowPYPlayer(PYPlayer elem)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            elem.IsShowingElement = EditorGUILayout.Foldout(elem.IsShowingElement, string.Format(AUDIO_ITEM_NAME, elem.Name, elem.GetType().ToString()));
            if (GUILayout.Button("-", GUILayout.Width(40)))
            {
                Undo.RecordObject(target, "Removed Elem");
                _target.Audios.Remove(elem);

                EditorUtility.SetDirty(target);
                return;
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            GUILayout.BeginVertical();
            if (elem.IsShowingElement)
            {
                if (elem is PYPlayerTag)
                {
                    PYPlayerTag playerTag = (PYPlayerTag)elem;
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("PYAudio Tag");
                    if (GUILayout.Button(playerTag.Tag.ToString()))
                    {
                        PYSelectorWindow.Init(_rectButtonTag, playerTag.Tag.ToString(),
                            Enum.GetNames(typeof(PYAudioTags)), (selectedItem) =>
                            {
                                playerTag.Tag = (PYAudioTags)Enum.Parse(typeof(PYAudioTags), selectedItem.Value);
                            });
                    }

                    GUILayout.EndHorizontal();
                }
                else if (elem is PYPlayerClip)
                {
                    PYPlayerClip playerClip = (PYPlayerClip)elem;
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Asset Tag");
                    string tag = string.IsNullOrEmpty(playerClip.AssetTag.UnprocessedTag) ?
                        "None" : playerClip.AssetTag.UnprocessedTag.Split(':')[0];
                    if (GUILayout.Button(tag))
                    {
                        string[] tags = PYBundleManager.GetAssetsTag();
                        tags = tags.Where(t => t.Contains(": Audio")).ToArray();

                        PYSelectorWindow.Init(_rectButtonTag, tag, tags, (selectedItem) =>
                            {
                                playerClip.AssetTag.UnprocessedTag = selectedItem.Value;
                            });
                    }

                    GUILayout.EndHorizontal();
                }

                if (Event.current.type == EventType.Repaint)
                    _rectButtonTag = GUILayoutUtility.GetLastRect();

                elem.DrawInspector();
                GUILayout.Space(5);
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        void ShowElementsProperties(bool isShowing)
        {
            foreach (PYPlayer player in _target.Audios)
                player.IsShowingElement = isShowing;
        }
    }
}