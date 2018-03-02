using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;

namespace Playmove
{
    public class PYSelectorWindow : EditorWindow
    {
        public class PYSelectorEventData
        {
            public int Index = -1;
            public string Value = "";
        }

        public class ItemData
        {
            public string Tag = "";
            public string Description = "";
        }

        private List<ItemData> _tags = new List<ItemData>();
        private List<ItemData> _searchTags = new List<ItemData>();

        private Vector2 _scrollPosition;

        private MethodInfo searchGUI;
        private string _searchTag = "";
        private ItemData _selectedTag = new ItemData();

        private GUIStyle _windowStyle;
        private GUIStyle _headerStyle;
        private GUIStyle _itemStyle;
        private GUIStyle _itemDescriptionLabelStyle;

        private Texture2D _tex;
        private bool _hasFocusedSearchBox = false;
        private bool _hasScrolledToSelectedItem = false;

        private double _clickTimer = 0;

        private Action<PYSelectorEventData> _onSelectedItem;

        private static Rect _windowRect;

        public static PYSelectorWindow Init(Rect windowRect, string[] tags, Action<PYSelectorEventData> onSelectedItem)
        {
            string[] tagsDescriptions = new string[tags.Length];
            for (int x = 0; x < tags.Length; x++)
                tagsDescriptions[x] = "";

            return Init(windowRect, tags, tagsDescriptions, onSelectedItem);
        }
        public static PYSelectorWindow Init(Rect windowRect, string selectedTag, string[] tags, Action<PYSelectorEventData> onSelectedItem)
        {
            string[] tagsDescriptions = new string[tags.Length];
            for (int x = 0; x < tags.Length; x++)
                tagsDescriptions[x] = "";

            return Init(windowRect, selectedTag, tags, tagsDescriptions, onSelectedItem);
        }
        public static PYSelectorWindow Init(Rect windowRect, string selectedTag, string[] tags, string[] tagsDescriptions, Action<PYSelectorEventData> onSelectedItem)
        {
            _windowRect = windowRect;
            _windowRect.height = 320;

            PYSelectorWindow window = Editor.CreateInstance<PYSelectorWindow>();
            window.ShowAsDropDown(GUIToScreenRect(_windowRect), new Vector2(_windowRect.width, _windowRect.height));
            window.Initialize(selectedTag, tags, tagsDescriptions, onSelectedItem);

            window.Focus();
            return window;
        }
        public static PYSelectorWindow Init(Rect windowRect, string[] tags, string[] tagsDescriptions, Action<PYSelectorEventData> onSelectedItem)
        {
            _windowRect = windowRect;
            _windowRect.height = 320;

            PYSelectorWindow window = Editor.CreateInstance<PYSelectorWindow>();
            window.ShowAsDropDown(GUIToScreenRect(_windowRect), new Vector2(_windowRect.width, _windowRect.height));
            window.Initialize("None", tags, tagsDescriptions, onSelectedItem);

            window.Focus();
            return window;
        }

        public void Initialize(string selectedTag, string[] tags, string[] tagsDescriptions, Action<PYSelectorEventData> onSelectedItem)
        {
            _onSelectedItem = onSelectedItem;

            for (int x = 0; x < tags.Length; x++)
            {
                _tags.Add(new ItemData() { Tag = tags[x], Description = tagsDescriptions[x] });
            }
            _searchTags = new List<ItemData>(_tags);
            _tags.Insert(0, new ItemData() { Tag = "None", Description = "" });

            if (!string.IsNullOrEmpty(selectedTag) &&
                selectedTag != "None")
            {
                foreach (ItemData item in _tags)
                {
                    if (item.Tag.StartsWith(selectedTag))
                    {
                        this._selectedTag = item;
                        break;
                    }
                }
            }
            else
                _hasScrolledToSelectedItem = true;

            _windowStyle = new GUIStyle();

            _headerStyle = new GUIStyle(GUI.skin.box);
            _headerStyle.margin = new RectOffset(0, 0, 0, 0);
            _headerStyle.normal.textColor = (EditorGUIUtility.isProSkin ? Color.white : Color.black) * 0.75f;
            _headerStyle.fontStyle = FontStyle.Bold;

            _itemStyle = new GUIStyle(GUI.skin.box);
            _itemStyle.fixedWidth = _windowRect.width - 15;
            _itemStyle.alignment = TextAnchor.MiddleLeft;
            _itemStyle.fontStyle = FontStyle.Bold;
            _itemStyle.normal.textColor = (EditorGUIUtility.isProSkin ? Color.white : Color.gray) * 0.75f;
            _itemStyle.hover.textColor = (EditorGUIUtility.isProSkin ? Color.white : Color.gray);

            _itemStyle.hover.background = _itemStyle.normal.background;
            _itemStyle.normal.background = null;

            _itemDescriptionLabelStyle = new GUIStyle(GUI.skin.label);
            _itemDescriptionLabelStyle.wordWrap = true;
            _itemDescriptionLabelStyle.normal.textColor = (EditorGUIUtility.isProSkin ? Color.white : Color.gray);

            searchGUI = typeof(EditorGUI).GetMethod("SearchField", BindingFlags.NonPublic | BindingFlags.Static);
        }

        void OnGUI()
        {
            GUILayout.BeginVertical(_windowStyle);

            GUI.SetNextControlName("SearchBox");
            string searchTagTemp = (string)searchGUI.Invoke(null, new object[] { new Rect(10, 5, _windowRect.width - 20, 30), _searchTag });

            if (searchTagTemp != _searchTag)
            {
                _searchTag = searchTagTemp;
                FilterTags(_searchTag);
            }

            GUILayout.Space(35);
            GUILayout.Box("Tags", _headerStyle, GUILayout.Width(_windowRect.width), GUILayout.Height(20));

            EditorGUILayout.Separator();
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, true);
            for (int x = 0; x < _tags.Count; x++)
            {
                if (_selectedTag == _tags[x])
                    GUI.color = Color.green;

                if (GUILayout.Button(_tags[x].Tag, _itemStyle))
                {
                    // Double click
                    if ((EditorApplication.timeSinceStartup - _clickTimer) < 0.3f)
                    {
                        CloseWindow();
                        _clickTimer = 0;
                    }
                    // Single click
                    else
                    {
                        SelectTag(_tags[x]);
                        _clickTimer = EditorApplication.timeSinceStartup;
                    }
                }

                if (_selectedTag.Tag == _tags[x].Tag &&
                    !string.IsNullOrEmpty(_selectedTag.Description))
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(15);
                    GUILayout.Label(_selectedTag.Description, _itemDescriptionLabelStyle);
                    GUILayout.EndHorizontal();
                }

                if (!_hasScrolledToSelectedItem &&
                    Event.current.type == EventType.Repaint && _selectedTag.Tag == _tags[x].Tag)
                {
                    _hasScrolledToSelectedItem = true;
                    Rect tempRect = GUILayoutUtility.GetLastRect();
                    _scrollPosition = new Vector2(0, tempRect.y - 20);
                }

                GUI.color = Color.white;
                Repaint();
            }
            GUILayout.EndScrollView();

            if (!_hasFocusedSearchBox)
            {
                _hasFocusedSearchBox = true;
                EditorGUI.FocusTextInControl("SearchBox");
            }

            GUILayout.EndVertical();
        }

        static Rect GUIToScreenRect(Rect guiRect)
        {
            Vector2 vector2 = GUIUtility.GUIToScreenPoint(new Vector2(guiRect.x, guiRect.y));
            guiRect.x = vector2.x;
            guiRect.y = vector2.y;
            return guiRect;
        }

        void FilterTags(string searchTag)
        {
            _tags.Clear();
            for (int x = 0; x < _searchTags.Count; x++)
            {
                if (_searchTags[x].Tag.ToLower().Contains(_searchTag.ToLower()))
                    _tags.Add(_searchTags[x]);
            }

            if ("None".Contains(searchTag))
                _tags.Insert(0, new ItemData() { Tag = "None", Description = "" });
        }

        void SelectTag(ItemData tag)
        {
            _selectedTag = tag;
            if (_onSelectedItem != null)
                _onSelectedItem(new PYSelectorEventData() { Index = _searchTags.IndexOf(_selectedTag), Value = _selectedTag.Tag });
        }

        void CloseWindow()
        {
            _onSelectedItem = null;
            Close();
        }
    }
}