using System;
using FullInspector;
using UnityEngine;
using System.Collections;
using MDS.Core.Interfaces;
using UnityEngine.Events;

namespace MDS.Core
{
    public class ProcessAnswerTimer : MDSBehaviour, IAnswerProcessor
    {

        public event ProcessAnswerDelegate OnProcessAnswer;

        [SerializeField]
        private int _seconds;

        private int _currentCounter;
        private bool _enabledCounter;

        [SerializeField]
        private bool _countDown;

        public UnityEvent<int> OnTick;


        protected override void Awake()
        {
            base.Awake();
            gameObject.tag = "IAnswerProcessor";
        }

        public void Start()
        {
            StartCoroutine(CountOneSecond());

            _currentCounter = 0;
            if(_countDown)
                _currentCounter = _seconds;


            OnTick.Invoke(_currentCounter);

            Enable();
        }

        public void Disable()
        {
            _enabledCounter = false;
        }

        public void Enable()
        {
            _enabledCounter = true;
        }

        public IEnumerator CountOneSecond()
        {
            while(true)
            {
                yield return new WaitForSeconds(1);
                if(_enabledCounter)
                {
                    if(_countDown)
                    {
                        _currentCounter--;
                        if(_currentCounter == 0)
                            break;
                    }
                    else
                    {
                        _currentCounter++;
                        if(_currentCounter == _seconds)
                            break;
                    }

                    OnTick.Invoke(_currentCounter);

                }
            }

            // esse tick extra é para enviar o ultimo valor da contagem
            OnTick.Invoke(_currentCounter);

            Disable();
            if(OnProcessAnswer != null)
                OnProcessAnswer();

        }

    }

}