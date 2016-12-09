using System.Collections;
using System.Collections.Generic;
using MDS.Gameplay.FSM.State;
using System.Linq;
using UnityEngine;
using System;

namespace MDS.Gameplay.FSM
{
    public class RandomFSM : MDSBehaviour
    {
        
        private RandomState[] _states;


        protected override void Awake()
        {
            base.Awake();
            InitializeStates();
        }

        private void InitializeStates()
        {
            int maxStates = transform.childCount;
            _states = new RandomState[maxStates];

            for(int i = 0; i < maxStates; i++)
            {
                _states[i] = transform.GetChild(i).GetComponent<RandomState>();
            }
        }

        public void SetInitialState()
        {
            foreach(var state in _states)
            {
                state.gameObject.SetActive(false);
            }
            _states[0].gameObject.SetActive(true);

        }


        public void ChangeState()
        {
            _states.Where(s => s.gameObject.activeInHierarchy).First().ChangeState();
        }



        public RandomState GetCurrentState()
        {
            return _states.Where(s => s.gameObject.activeInHierarchy).First();
        }


    }
}