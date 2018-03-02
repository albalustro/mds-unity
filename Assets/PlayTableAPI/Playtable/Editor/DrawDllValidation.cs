using UnityEngine;
using UnityEditor;
using System;

namespace Playmove
{
    [CustomEditor(typeof(PlaytableWin32))]
    public class DrawDllValidation : Editor
    {
        static void OnSceneGUI()
        {
            if (!SopService.Authentication.TrueValidation) return;

            Handles.BeginGUI();

            var centeredStyle = new GUIStyle(GUI.skin.GetStyle("Label"));
            centeredStyle.alignment = TextAnchor.UpperCenter;
            centeredStyle.fontSize = 20;
            GUILayout.Label(SopService.Authentication.Message, centeredStyle);

            Handles.EndGUI();
        }
    }
}