using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MDS.Utilities;
using UnityEngine;


namespace MDS.Gameplay.FSM
{

    public class RandomFSMManager : MDSBehaviour
    {

        [SerializeField]
        private int _gameDuration = 40;
        private float _currentGameTime = 0;

        [SerializeField]
        private bool _spawnTwoAtOnce;
        [SerializeField]
        private bool _spawnTreeAtOnce;


        [SerializeField]
        private int _minSpawnInterval;
        [SerializeField]
        private int _maxSpawnInterval;


        private RandomFSM[] _fsmList;

        void Start()
        {
            _fsmList = transform.GetComponentsInChildren<RandomFSM>();

            StartGame();
        }


        public void StartGame()
        {
            _currentGameTime = 0;
            StartCoroutine(UpdateGame());
        }

        private IEnumerator UpdateGame()
        {
            while(true)
            {

                float time = UnityEngine.Random.Range(_minSpawnInterval, _maxSpawnInterval);
                
                if(_currentGameTime + time >= _gameDuration)
                    time = _gameDuration - _currentGameTime;

                Log("Proximo spawn em: " + time.ToString());

                yield return new WaitForSeconds(time);

                _currentGameTime+=time;
                if(_currentGameTime >= _gameDuration)
                    break;

                StartRandomFSM();

                if (_spawnTwoAtOnce)
                {
                    if (_currentGameTime/_gameDuration>.6)
                    {
                        StartRandomFSM();
                    }

                    if (_spawnTreeAtOnce)
                    {
                        if (_currentGameTime/_gameDuration>.8)
                        {
                            StartRandomFSM();
                        }
                    }
                }
            }

        }

        

        private void StartRandomFSM()
        {
            
            var availableList = _fsmList.Where(f => f.GetCurrentState().IsInitialState).ToList();

            if (availableList!=null && availableList.Count>=1)
                availableList.GetRandom().ChangeState();
        }
    }
}