using Playmove;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FactoryConfiguration))]
public class FactoryFilesEditor : Editor
{
    private FactoryConfiguration _ownTarget;

    private int _selectedFileWithoutGroup = 0;
    private List<string> _filesWithoutGroupOptions = new List<string>();

    private bool _filesWithoutGroupFoldout = false;
    private List<bool> _groupsFoldout = new List<bool>();
    private List<bool> _groupsFilesFoldout = new List<bool>();

    private void OnEnable()
    {
        _ownTarget = (FactoryConfiguration)target;
        for (int i = 0; i < _ownTarget.Groups.Count; i++)
        {
            _groupsFoldout.Add(false);
            _groupsFilesFoldout.Add(false);
        }

        foreach (var file in _ownTarget.FilesWithoutGroup)
            _filesWithoutGroupOptions.Add(CreateFileWithoutGroupOption(file));
    }

    private void OnDisable()
    {
        EditorUtility.SetDirty(_ownTarget);
        AssetDatabase.SaveAssets();
    }

    public override void OnInspectorGUI()
    {
        if (GUILayout.Button(ActiveEditorTracker.sharedTracker.isLocked ? "Unlock" : "Lock"))
        {
            // If we are locked we select us firts that allow the unlocking proccess
            if (ActiveEditorTracker.sharedTracker.isLocked)
                Selection.activeObject = _ownTarget;

            ActiveEditorTracker.sharedTracker.isLocked = !ActiveEditorTracker.sharedTracker.isLocked;
        }

        if (GUILayout.Button("Generate GUIDs for all Files"))
        {
            foreach (var group in _ownTarget.Groups)
                GenerateGUIDs(group.AppFiles);

            GenerateGUIDs(_ownTarget.FilesWithoutGroup);
        }

        GUILayout.Space(20);

        GUILayout.BeginVertical(GUI.skin.box);
        GUILayout.BeginHorizontal();
        GUILayout.Space(10);
        _filesWithoutGroupFoldout = EditorGUILayout.Foldout(_filesWithoutGroupFoldout, "Files without group: " + _ownTarget.FilesWithoutGroup.Count);
        if (GUILayout.Button("Remove all"))
        {
            _ownTarget.FilesWithoutGroup.Clear();
            EditorUtility.SetDirty(_ownTarget);
        }
        GUILayout.EndHorizontal();
        if (_filesWithoutGroupFoldout)
        {
            GUI.enabled = false;
            foreach (var file in _ownTarget.FilesWithoutGroup.GetRange(0, _ownTarget.FilesWithoutGroup.Count))
                DrawFileGUI(null, file);
        }
        GUILayout.EndVertical();

        GUI.enabled = true;
        GUILayout.Space(15);
        if (GUILayout.Button("Create New Group"))
        {
            PYGroupFilesManager.GameFilesGroup group = new PYGroupFilesManager.GameFilesGroup();
            group.FactoryConfiguration = true;
            group.Visible = true;
            group.Localization = "pt-BR";

            _ownTarget.Groups.Add(group);
            _groupsFoldout.Add(true);
            _groupsFilesFoldout.Add(false);
        }

        foreach (PYGroupFilesManager.GameFilesGroup group in _ownTarget.Groups.GetRange(0, _ownTarget.Groups.Count))
        {
            if (group == null) continue;
            if (group.Name == null) group.Name = "";

            GUILayout.BeginVertical(GUI.skin.box);

            int groupIndex = _ownTarget.Groups.IndexOf(group);

            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            _groupsFoldout[groupIndex] = EditorGUILayout.Foldout(_groupsFoldout[groupIndex], group.Name.ToUpper());

            if (GUILayout.Button("Delete"))
                DeleteGroup(group);
            GUILayout.EndHorizontal();

            if (_groupsFoldout[groupIndex])
            {
                if (string.IsNullOrEmpty(group.Guid))
                    group.Guid = Guid.NewGuid().ToString().ToUpper();
                GUILayout.Label("GUID: " + group.Guid);
                GUILayout.Space(10);
                group.Name = EditorGUILayout.TextField("Nome: ", group.Name);
                group.Visible = EditorGUILayout.Toggle("Visivel: ", group.Visible);
                group.Localization = EditorGUILayout.TextField("Localização: ", group.Localization);

                DropAreaGUI("Drop files here", (assets) =>
                {
                    foreach (string path in assets)
                    {
                        if (group.AppFiles.Find(i => i.File.FullPath == path) != null) continue;
                        PYFileApplicationManager.FileApplication appfile = new PYFileApplicationManager.FileApplication(new PYFileManager.File(path), PlaytableWin32.Instance.GameId);
                        appfile.File.Localization = group.Localization;
                        group.AppFiles.Add(appfile);
                    }
                });
                GUILayout.Space(5);

                GUILayout.BeginHorizontal();
                GUILayout.Space(15);
                GUILayout.BeginVertical();
                _groupsFilesFoldout[groupIndex] = EditorGUILayout.Foldout(_groupsFilesFoldout[groupIndex], string.Format("Files: {0}", group.AppFiles.Count));
                if (_groupsFilesFoldout[groupIndex])
                {
                    if (_ownTarget.FilesWithoutGroup.Count > 0)
                    {
                        GUILayout.BeginHorizontal();

                        _selectedFileWithoutGroup = EditorGUILayout.Popup("File to be added: ", _selectedFileWithoutGroup, _filesWithoutGroupOptions.ToArray());
                        if (GUILayout.Button("Add"))
                        {
                            group.AppFiles.Add(_ownTarget.FilesWithoutGroup[_selectedFileWithoutGroup]);
                            _filesWithoutGroupOptions.RemoveAt(_selectedFileWithoutGroup);
                            _ownTarget.FilesWithoutGroup.RemoveAt(_selectedFileWithoutGroup);
                            _selectedFileWithoutGroup = 0;

                            EditorUtility.SetDirty(_ownTarget);
                        }

                        GUILayout.EndHorizontal();
                    }

                    foreach (var file in group.AppFiles.GetRange(0, group.AppFiles.Count))
                        DrawFileGUI(group, file);
                }
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }
    }

    public void DropAreaGUI(string message, Action<string[]> dropCallback)
    {
        Event evt = Event.current;
        Rect drop_area = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
        GUI.Box(drop_area, message);

        switch (evt.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (!drop_area.Contains(evt.mousePosition))
                    return;

                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();

                    if (dropCallback != null)
                        dropCallback(DragAndDrop.paths);
                }
                break;
        }
    }

    private void DrawFileGUI(PYGroupFilesManager.GameFilesGroup group, PYFileApplicationManager.FileApplication file)
    {
        GUILayout.BeginVertical(GUI.skin.box);

        GUILayout.BeginHorizontal();
        GUILayout.Label("GUID: " + Path.GetFileNameWithoutExtension(file.File.FullPath));

        if (group != null)
        {
            if (GUILayout.Button("Remove"))
            {
                group.AppFiles.Remove(file);
                _ownTarget.FilesWithoutGroup.Add(file);
                _filesWithoutGroupOptions.Add(CreateFileWithoutGroupOption(file));
            }
        }
        GUILayout.EndHorizontal();

        file.File.Name = EditorGUILayout.TextField("Nome: ", file.File.Name);
        file.Grouping = EditorGUILayout.TextField("Grupo/Categoria: ", file.Grouping);
        file.File.Localization = EditorGUILayout.TextField("Localização: ", file.File.Localization);

        EditorGUILayout.PrefixLabel("Configuração: ");
        file.Data = EditorGUILayout.TextArea(file.Data, GUILayout.Height(40));

        EditorGUILayout.LabelField(file.File.FullPath);

        GUILayout.EndVertical();
    }

    private void DeleteGroup(PYGroupFilesManager.GameFilesGroup group)
    {
        _ownTarget.FilesWithoutGroup.AddRange(group.AppFiles);
        _ownTarget.FilesWithoutGroup = _ownTarget.FilesWithoutGroup.Distinct().ToList();

        // Sync options for files without group
        _filesWithoutGroupOptions.Clear();
        foreach (var file in _ownTarget.FilesWithoutGroup)
            _filesWithoutGroupOptions.Add(CreateFileWithoutGroupOption(file));

        _ownTarget.Groups.Remove(group);
        EditorUtility.SetDirty(_ownTarget);
    }

    private void GenerateGUIDs(List<PYFileApplicationManager.FileApplication> files)
    {
        foreach (PYFileApplicationManager.FileApplication file in files)
        {
            if (!IsValidGUID(Path.GetFileNameWithoutExtension(file.File.FullPath)))
            {
                string destPath = file.File.FullPath.Replace(file.File.Name, Guid.NewGuid().ToString().ToUpper());
                try
                {
                    File.Move(file.File.FullPath, destPath);
                    file.File.FullPath = destPath;
                }
                catch (Exception e)
                {
                    Debug.LogError(string.Format("{0} at src path {1} and dest path {2} \n {3}",
                        file.File.Name, file.File.FullPath, destPath, e.Message));
                }
            }
        }

        AssetDatabase.Refresh();
        EditorUtility.SetDirty(_ownTarget);
    }

    private bool IsValidGUID(string GUIDCheck)
    {
        if (!string.IsNullOrEmpty(GUIDCheck))
            return new Regex(@"^(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})$").IsMatch(GUIDCheck);
        return false;
    }

    private string CreateFileWithoutGroupOption(PYFileApplicationManager.FileApplication file)
    {
        return string.Format("Name: {0} | GUID: {1}", file.File.Name, Path.GetFileNameWithoutExtension(file.File.FullPath));
    }
}