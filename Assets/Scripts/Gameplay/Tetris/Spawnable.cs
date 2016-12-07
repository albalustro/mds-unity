using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FullInspector;

namespace MDS.Gameplay.Tetris
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Spawnable : MDSBehaviour
    {

        public List<string> Labels;
        private SpriteRenderer _renderer;
        private Lane _currentLane;

        protected override void Awake()
        {
            base.Awake();
            _renderer = this.GetComponent<SpriteRenderer>();
        }

        void OnEnable()
        {

        }


        public void SetCurrentLane(Lane l)
        {

        }

        public Lane GetCurrentLane()
        {
            Lane l = null;
            return l;
        }


    }
}