using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MDS.Utilities;
using UnityEngine;

namespace MDS.Gameplay.FSM.State.Transition
{
    public class RandomTransition : IStateTransition
    {
        protected RndState _myState;

        public GameObject[] _states;


        public class ProbabilityDistribution<T>
        {
            public class ProbabilityElement
            {
                public T Element { get; set; }
                public float Probability { get; set; }

                [FullInspector.InspectorDisabled]
                public float NormalizedProbability { get; set; }
                [FullInspector.InspectorDisabled]
                public float MinPercent { get; set; }
                [FullInspector.InspectorDisabled]
                public float MaxPercent { get; set; }
            }

            [NonSerialized, FullInspector.InspectorDisabled]
            public bool isNormalized = false;

            public List<ProbabilityElement> distribution { get; set; }
            
            [FullInspector.InspectorButton]
            public void Normalize()
            {
                float sum = distribution.Sum(s => s.Probability);
                distribution.ForEach(d => d.NormalizedProbability = d.Probability / sum);

                distribution[0].MinPercent = 0f;
                distribution[0].MaxPercent = distribution[0].NormalizedProbability;

                if(distribution.Count == 1)
                    return;


                for(int i = 1; i < distribution.Count-1; i++)
                {
                    distribution[i].MinPercent = distribution[i - 1].MaxPercent;
                    distribution[i].MaxPercent = distribution[i].MinPercent + distribution[i].NormalizedProbability;
                }

                int lastIndex = distribution.Count - 1;
                distribution[lastIndex].MinPercent = distribution[lastIndex - 1].MaxPercent;
                distribution[lastIndex].MaxPercent = 1f;

                isNormalized = true;

            }

            public T PickRandom()
            {
                float percent = UnityEngine.Random.Range(0f, 1f);
                Debug.Log(percent);
                T element = distribution.Single(d => percent>= d.MinPercent && percent< d.MaxPercent).Element;
                return element;
            }
        }

        public ProbabilityDistribution<GameObject> _rndStates;

        public void ExecuteTransition()
        {
            if(!_rndStates.isNormalized)
                _rndStates.Normalize();

            _rndStates.PickRandom().SetActive(true);
            //_states.GetRandom().SetActive(true);
            _myState.gameObject.SetActive(false);
        }

        public void Initialize(RndState state)
        {
            _myState = state;
        }
    }
}