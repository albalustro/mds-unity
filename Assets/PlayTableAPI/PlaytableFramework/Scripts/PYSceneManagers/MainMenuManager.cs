using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Playmove
{
    public class MainMenuManager : PYSceneManager
    {
        private static MainMenuManager _instance;
        public static MainMenuManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType<MainMenuManager>();
                return _instance;
            }
        }

        [Header("Buttons")]
        public PYButton MenuButton;
        public PYButton ScoreButton;
        public PYButton ClearNames;

        void Awake()
        {
            FaderManager.FadeOutCamera(1, 0, Color.white);
            if (_instance == null)
                _instance = this;

            ClearNames.onClick.AddListener(ClearAllNames);
        }

        private void ClearAllNames(PYButton btn)
        {
            //PYNamesManager.DeleteAll();
        }

        protected override void Start()
        {
            base.Start();

            //PYScoreData.Initialize();

            MenuButton.onClick.AddListener((sender) =>
            {
                ChangeScene(OptionsMenuManager.Instance);
            });

            ScoreButton.onClick.AddListener((sender) =>
            {
                ChangeScene(TagManager.Scenes.Score);
            });
        }

        public void CloseApp()
        {
            Close(() =>
            {
                if (!Application.isEditor) System.Diagnostics.Process.GetCurrentProcess().Kill();
            });
        }
    }
}