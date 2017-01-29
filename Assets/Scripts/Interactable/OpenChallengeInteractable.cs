using System;
using System.Collections;
using System.Collections.Generic;
using FullInspector;
using MDS.Core.SceneManagement;
using MDS.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MDS.Interactable
{
    public class OpenChallengeInteractable : MoveInteraction
    {
        
        private List<Transform> _interactionPositionReferences;
        private Vector2 _interationPosition;
        private int _challengeSceneIndex = 1;

        protected override void Awake()
        {
            base.Awake();
            LoadReferences();
            _challengeSceneIndex = GetComponent<ChallengeLinkInEpisode>().ChallengeIndex + 1;
        }

        private void LoadReferences()
        {
            _interactionPositionReferences = new List<Transform>();
            for(int i = 0; i < transform.childCount; i++)
            {
                if(transform.GetChild(i).CompareTag("Interaction Position"))
                    _interactionPositionReferences.Add(transform.GetChild(i));
            }

        }

        public override void Interact()
        {
            if(Vector2.Distance(_player.transform.position, _interationPosition) < 1f)
            {
                SceneLoader.Instance.LoadChallenge(_challengeSceneIndex, SceneManager.GetActiveScene().GetEpisodeIndex());
                return;
            }

            float dist = float.MaxValue;
            foreach(var item in _interactionPositionReferences)
            {
                var d = Vector2.SqrMagnitude(item.position - _player.transform.position);
                if(d < dist)
                {
                    dist = d;
                    _interationPosition = item.position;
                }
            }
           

            _player.MoveAndInteract(_interationPosition, this);
            
        }



#if UNITY_EDITOR
        [ShowInInspector]
        private bool debugging;

        public void OnDrawGizmos()
        {
            if(debugging)
                DrawGizmos();
        }
        public void OnDrawGizmosSelected()
        {
            DrawGizmos();
        }
        private void DrawGizmos()
        {
            if(Application.isPlaying) return;

            LoadReferences();
            Gizmos.color = Color.red;
            for(int i = 0; i < _interactionPositionReferences.Count; i++)
            {
                if (_interactionPositionReferences[i]!=null)
                {
                    Gizmos.DrawWireSphere(_interactionPositionReferences[i].position, 0.15f);
                }
            }

        }
#endif

    }

}