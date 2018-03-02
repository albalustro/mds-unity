using UnityEngine;
using System.Collections;

namespace Playmove
{
    public class GameSceneManager : PYSceneManager
    {

        private static GameSceneManager _instance;
        public static GameSceneManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType<GameSceneManager>();
                return _instance;
            }
        }

        [Header("Buttons")]
        public PYButton ButtonRegister;

        void Awake()
        {
            if (_instance == null)
                _instance = this;
        }
    }
}