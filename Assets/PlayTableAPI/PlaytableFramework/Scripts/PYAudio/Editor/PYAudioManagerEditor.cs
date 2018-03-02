using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.Audio;
using System;
using System.Text.RegularExpressions;

namespace Playmove
{
    [CustomEditor(typeof(PYAudioManager))]
    public class PYAudioManagerEditor : Editor
    {
        public class TagData
        {
            public string Tag;
            public bool IsObsolete;

            public TagData(string tag, bool isObsolete)
            {
                Tag = tag;
                IsObsolete = isObsolete;
            }
        }

        private const string AUDIO_TAG_OBSOLETE = "[Obsolete(\"The audioFile for this tag dont exist!\")] ";
        private const string AUDIO_RESOURCE_FOLDER = "Audios";

        private PYAudioManager targetRef;

        void OnEnable()
        {
            targetRef = (PYAudioManager)target;
            if (targetRef == null) return;

            if (targetRef.Groups == null)
                targetRef.Groups = new Dictionary<PYGroupTag, PYAudioManager.AudioGroup>();

            UpdateAudioManagerGroups();
        }

        public override void OnInspectorGUI()
        {
            GUILayout.Space(5);
            GUILayout.BeginVertical(GUI.skin.box);
            PYAudioManager.GlobalVolume = EditorGUILayout.Slider("Global Volume: ", PYAudioManager.GlobalVolume, 0, 1);

            AudioMixer mixer = (AudioMixer)EditorGUILayout.ObjectField("Audio Mixer: ", targetRef.Mixer, typeof(AudioMixer), false);
            // Update mixer in AudioManager and update groups
            if (mixer != targetRef.Mixer)
            {
                targetRef.Mixer = mixer;
                UpdateAudioManagerGroups();
            }
            GUILayout.EndVertical();

            GUILayout.Space(10);
            GUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label("Groups" + (targetRef.Groups.Keys.Count == 0 ? ": You need to assing a AudioMixer to show groups" : ""));
            foreach (PYGroupTag groupName in targetRef.Groups.Keys)
            {
                GUILayout.BeginHorizontal();

                PYAudioManager.AudioGroup group = targetRef.Groups[groupName];
                bool groupMute = !EditorGUILayout.ToggleLeft(groupName.ToString(), !group.Mute);
                float volume = EditorGUILayout.Slider(group.Volume, 0, 1);

                if (GUI.changed)
                {
                    if (groupMute != group.Mute)
                        PYAudioManager.Instance.MuteGroup(group.Group, groupMute);
                    if (volume != group.Volume)
                        PYAudioManager.Instance.SetGroupVolume(group.Group, volume);
                }

                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();

            GUILayout.Space(10);
            if (GUILayout.Button("Build Tags"))
            {
                BuildTags();
            }

            if (GUILayout.Button("Remove Obsolete Tags"))
            {
                RemoveObsoleteTags();
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(targetRef);
                EditorUtility.SetDirty(targetRef.gameObject);
            }
        }

        void UpdateAudioManagerGroups()
        {
            if (targetRef.Mixer == null) return;

            AudioMixerGroup[] mixerGroups = targetRef.Mixer.FindMatchingGroups("");
            // If dont exist any group in audioManager or a new group was added in mixer we update it
            if (targetRef.Groups.Count == 0 || targetRef.Groups.Count != mixerGroups.Length)
            {
                foreach (AudioMixerGroup mixerGroup in mixerGroups)
                {
                    PYGroupTag groupTag = (PYGroupTag)Enum.Parse(typeof(PYGroupTag), mixerGroup.name);
                    if (!targetRef.Groups.ContainsKey(groupTag))
                        targetRef.Groups.Add(groupTag, new PYAudioManager.AudioGroup(groupTag));
                }
            }

            // Reorder the groups in audioManager to match the mixer order
            Dictionary<PYGroupTag, PYAudioManager.AudioGroup> tempGroups = new Dictionary<PYGroupTag, PYAudioManager.AudioGroup>(targetRef.Groups);
            targetRef.Groups.Clear();
            foreach (AudioMixerGroup mixerGroup in mixerGroups)
            {
                PYGroupTag groupTag = (PYGroupTag)Enum.Parse(typeof(PYGroupTag), mixerGroup.name);
                targetRef.Groups.Add(groupTag, tempGroups[groupTag]);
            }
        }

        void BuildTags()
        {
            targetRef.Audios.Clear();
            UpdateAudioManagerGroups();

            List<string> files = new List<string>();
            List<string> filesBundle = new List<string>(PYBundleManager.GetAssetsTag().Where(s => s.EndsWith("Audio")));
            List<string> filesResources = new List<string>();

            try
            {
                filesResources.AddRange(Directory.GetFiles(Application.dataPath, "*.mp3", SearchOption.AllDirectories));
                filesResources.AddRange(Directory.GetFiles(Application.dataPath, "*.wav", SearchOption.AllDirectories));
                filesResources = filesResources.Where(n => n.Contains(@"Resources\" + AUDIO_RESOURCE_FOLDER) && (n.EndsWith(".mp3") || n.EndsWith(".wav"))).ToList();
            }
            catch { }

            foreach (string dir in filesResources)
            {
                // Filter paths to just relative path to Unity Resource folder
                string filePath = dir.Replace(dir.Substring(0, dir.IndexOf(@"Resources\" + AUDIO_RESOURCE_FOLDER)), "")
                    .Replace(@"Resources\" + AUDIO_RESOURCE_FOLDER, "");

                filePath = filePath.Replace(".mp3", "").Replace(".wav", "");
                filePath = filePath.Replace(@"\", "_").Remove(0, 1);

                PYGroupTag groupTag = (PYGroupTag)System.Enum.Parse(typeof(PYGroupTag), filePath.Split('_')[0]);
                targetRef.Audios.Add(new PYAudioManager.AudioTrack(filePath, groupTag, AUDIO_RESOURCE_FOLDER + "/" + filePath.Replace("_", @"/")));

                files.Add(filePath);
            }
            filesBundle.ForEach((n) => files.Add("B_" + n.Replace(" : Audio", "")));

            // Read PYAudioTags script
            List<TagData> audioTags = GetAudioTagDataFromScript();

            // Find obsolete tags
            foreach (TagData tagData in audioTags)
            {
                tagData.IsObsolete = (!files.Contains(tagData.Tag) && tagData.Tag != "None");
            }

            // Add new tags
            foreach (string tag in files)
            {
                if (audioTags.Find(i => i.Tag == tag) == null && tag != "None")
                    audioTags.Add(new TagData(tag, false));
            }

            // If the None tag wasn't in list we add it
            if (audioTags.Find(i => i.Tag == "None") == null)
                audioTags.Add(new TagData("None", false));

            WriteAudioTagsOnScript(audioTags);
        }

        void RemoveObsoleteTags()
        {
            List<TagData> audioTags = GetAudioTagDataFromScript();

            foreach (TagData tagData in audioTags.GetRange(0, audioTags.Count))
            {
                if (tagData.IsObsolete)
                    audioTags.Remove(tagData);
            }

            WriteAudioTagsOnScript(audioTags);
        }

        List<TagData> GetAudioTagDataFromScript()
        {
            List<TagData> audioTags = new List<TagData>();
            string scriptAudioTagPath = Directory.GetFiles(Application.dataPath, "PYAudioTags.cs", SearchOption.AllDirectories)[0];
            bool foundStartEnum = false;

            foreach (string line in File.ReadAllLines(scriptAudioTagPath))
            {
                if (line == "{")
                {
                    foundStartEnum = true;
                    continue;
                }

                bool isObsolete = line.Contains("[Obsolete(");
                if (foundStartEnum && line != "}")
                    audioTags.Add(new TagData(line.Replace(AUDIO_TAG_OBSOLETE, "").Replace(",", "").Trim(), isObsolete));
            }

            return audioTags;
        }
        void WriteAudioTagsOnScript(List<TagData> audioTags)
        {
            string content = "using System;\n\npublic enum PYAudioTags \n{\n";
            foreach (TagData tagData in audioTags)
            {
                if (tagData.IsObsolete)
                    content += "\t" + AUDIO_TAG_OBSOLETE + tagData.Tag + ",\n";
                else
                    content += "\t" + tagData.Tag + ",\n";
            }
            content += "}";

            string scriptAudioTagPath = Directory.GetFiles(Application.dataPath, "PYAudioTags.cs", SearchOption.AllDirectories)[0];
            File.WriteAllText(scriptAudioTagPath, content);
            AssetDatabase.Refresh();
        }
    }
}