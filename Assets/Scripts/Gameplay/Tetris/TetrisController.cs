using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FullInspector;

namespace MDS.Gameplay.Tetris
{
    public class TetrisController : MDSBehaviour
    {

        public LaneGroup laneGroup;
        public SpawnableGroup spawnableGroup;
        public Counter counter;
        public int amount;

        private int _currentLaneIndex;
        private bool _didChangeLastFrame;
        private Spawnable _currentSpawnable;

        //Preciso de uma Action que inicie a mecânica quando o diálogo fechar

        private void Start()
        {
            //defino o counter com o amount passado
        }

        void StartChallenge()
        {
            _currentSpawnable = SortNewSpawnable();
            Lane la = SortNewLane(ref _currentLaneIndex);
            _currentSpawnable.transform.position = la.spawnPosition.position;
            _currentSpawnable.SetCurrentLane(la);
            _currentSpawnable.gameObject.SetActive(true);
        }


        #region Spawn New Item
        //Sorteia uma posição para criar o item
        Lane SortNewLane(ref int laneIndex)
        {
            laneIndex = RandomInt(laneGroup.lanes.Count);
            return laneGroup.lanes[laneIndex];
        }

        //Sorteia um item a ser lançado nas lanes
        Spawnable SortNewSpawnable()
        {
            return spawnableGroup.spawnables[RandomInt(spawnableGroup.spawnables.Count)];
        }

        int RandomInt(int max)
        {
            return Random.Range(0, max);
        }


        #endregion

        void Update()
        {
            float input = Input.GetAxis("Horizontal");
            if (Mathf.Abs(input) > 0.1f)
            {
                if (!_didChangeLastFrame)
                {
                    _didChangeLastFrame = true;
                    _currentLaneIndex += Mathf.RoundToInt(Mathf.Sign(input));
                    if (_currentLaneIndex < 0) _currentLaneIndex = 0;
                    else if (_currentLaneIndex >= laneGroup.lanes.Count) _currentLaneIndex = laneGroup.lanes.Count - 1;
                }
            }
            else
            {
                _didChangeLastFrame = false;
            }


            //Vector3 pos = transform.position;
            //pos.x = Mathf.Lerp(pos.x, firstLandXPos + laneDistance * laneNumber, Time.deltaTime * sideSpeed);
            //transform.position = pos;
        }

    }
}