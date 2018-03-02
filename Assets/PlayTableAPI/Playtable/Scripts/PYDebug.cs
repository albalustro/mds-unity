using UnityEngine;
using System.Collections.Generic;

namespace Playmove
{
    public enum PYDebugLevel
    {
        Normal,
        Warning,
        Error
    }

    public class PYDebugData
    {
        public string Message = string.Empty;
        public PYDebugLevel Level = PYDebugLevel.Normal;

        public PYDebugData(string message, PYDebugLevel level)
        {
            Message = message;
            Level = level;
        }
    }

    public class PYDebug : MonoBehaviour
    {
        private bool _showDebugDialog = false;
        private Vector2 _logConsoleScrollPos = Vector2.zero;

        private static List<PYDebugData> _logs = new List<PYDebugData>();

        // Use this for initialization
        void Start()
        {

        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.BackQuote))
                _showDebugDialog = !_showDebugDialog;

            if (_showDebugDialog)
                FpsCounter.UpdateFPS();
        }

        void OnGUI()
        {
            if (!_showDebugDialog) return;

            GUILayout.BeginVertical(GUI.skin.box);
            GUILayout.BeginVertical(GUI.skin.box);

            GUI.color = FpsCounter.FPSColor();
            GUILayout.Label("FPS: " + FpsCounter.FPS.ToString("f4"));

            GUILayout.Space(5);
            GUI.color = Color.white;
            GUILayout.Label("Versão do jogo: " + PlaytableWin32.GameVersion.ToString());
            GUILayout.Label("Versão do Bundle: " + PlaytableWin32.BundleVersion.ToString());

            if (_logs.Count > 0)
            {
                GUILayout.BeginVertical(GUI.skin.box);
                GUILayout.Label("Logs:");

                if (_logs.Count >= 4)
                    _logConsoleScrollPos = GUILayout.BeginScrollView(_logConsoleScrollPos);
                
                int index = 0;
                foreach (PYDebugData log in _logs.GetRange(0, _logs.Count))
                {
                    switch (log.Level)
                    {
                        case PYDebugLevel.Normal: GUI.color = Color.white; break;
                        case PYDebugLevel.Warning: GUI.color = Color.yellow; break;
                        case PYDebugLevel.Error: GUI.color = Color.red; break;
                    }
                    GUILayout.Label(index + ": " + log.Message);
                    index++;
                }

                if (_logs.Count >= 4)
                    GUILayout.EndScrollView();

                GUILayout.EndVertical();
                GUI.color = Color.white;
            }

            GUILayout.EndVertical();
            GUILayout.EndVertical();
        }

        public static PYDebugData Log(string message)
        {
            PYDebugData data = new PYDebugData(message, PYDebugLevel.Normal);
            _logs.Add(data);
            return data;
        }
        public static PYDebugData Log(string message, PYDebugLevel level)
        {
            PYDebugData data = new PYDebugData(message, level);
            _logs.Add(data);
            return data;
        }

        public static void RemoveLog(PYDebugData logData)
        {
            _logs.Remove(logData);
        }
        public static void RemoveAllLogs()
        {
            _logs.Clear();
        }
    }
}
