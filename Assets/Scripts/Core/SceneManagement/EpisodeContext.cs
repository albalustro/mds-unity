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

        private ChallengeStatusInEpisode[] _challengeStatus = new ChallengeStatusInEpisode[5]; 

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);

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

            int index;
            index = int.Parse(curScene.name.Substring(7, 1))-1;

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
