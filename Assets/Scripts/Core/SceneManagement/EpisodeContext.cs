using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MDS.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MDS.Core.SceneManagement
{
    public class EpisodeContext : MDSBehaviour
    {
        private static EpisodeContext _instance;
        public static EpisodeContext Instance
        {
            get
            {
                if(_instance == null)
                    _instance = FindObjectOfType<EpisodeContext>();
                return _instance;
            }
        }

        public static bool HasEpisodeContext()
        {
            return _instance != null;
        }

        

        [SerializeField]
        private ChallengeStatusInEpisode[] _challengeStatus = new ChallengeStatusInEpisode[5];

#if UNITY_EDITOR
        [SerializeField]
        private bool _debugMode = false;
#endif

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);

#if UNITY_EDITOR
            if(_debugMode) return;
#endif

            _challengeStatus[0] = ChallengeStatusInEpisode.Available;
            for(int i = 1; i < 5; i++)
            {
                _challengeStatus[i] = ChallengeStatusInEpisode.Unavailable;
            }

        }

        

        public void SetCurrentChallengeDone()
        {
            Scene curScene = SceneManager.GetActiveScene();
            if(curScene.IsChallenge() == false)
                return;

            int index = curScene.GetChallengeIndex() - 1;

            _challengeStatus[index] = ChallengeStatusInEpisode.Done;
            if(index < 4)
                _challengeStatus[++index] = ChallengeStatusInEpisode.Available;
        }

        public ChallengeStatusInEpisode GetChallengeStatus(int index)
        {
            return _challengeStatus[index];
        }

        public void SetChallengeStatus(int index, ChallengeStatusInEpisode status)
        {
            _challengeStatus[index] = status;
        }
    }


    public enum ChallengeStatusInEpisode
    {
        Unavailable,
        Available,
        Done
    }

}
