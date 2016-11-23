using System;
using System.Collections;
using MDS.Core.Interfaces;
using MDS.Validators.Enum;
using MDS.Validators.Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MDS.Core
{

    public delegate void ProcessAnswerDelegate();


    public class Challenge : MDSBehaviour
    {

        public class ChallengeActions
        {
            // ações executadas automaticamente quando
            // entra no desafio (ANTES do desafio ser jogado)
            // ex: sequencias de dialogos, animacoes, etc
            public IAction[] onStartActions;

            // ações executadas quando responde errado
            public IAction[] onErrorActions;

            // ações executadas quando obtem sucesso na valicao
            public IAction[] onVictoryActions;
        }


        // validadores do desafio
        public IValidator Validador;

        public Challenge nextChallenge;

        private IAnswerProcessor answerProcessor;

        public ChallengeActions _actions;


        protected override void Awake()
        {
            base.Awake();

            if(nextChallenge != null)
                nextChallenge.gameObject.SetActive(false);

        }

        public IEnumerator Start()
        {
            answerProcessor = GameObject.FindWithTag("IAnswerProcessor").GetComponent<IAnswerProcessor>();
            if(answerProcessor == null)
            {
                Debug.LogError("IAnswerProcessor não encontrado na cena " + SceneManager.GetActiveScene().name);
            }
            answerProcessor.OnProcessAnswer += ProcessResult;

            foreach(var item in _actions.onStartActions)
            {
                yield return item.Execute();
            }
        }

        public void Update()
        {
            if(Validador.ReadyToValidate())
                answerProcessor.Enable();
            else
                answerProcessor.Disable();
        }


        IEnumerator Victory()
        {
            foreach(var item in _actions.onVictoryActions)
            {
                yield return item.Execute();
            }
        }

        IEnumerator Lose()
        {
            foreach(var item in _actions.onErrorActions)
            {
                yield return item.Execute();
            }
        }

        void ProcessResult()
        {
            ValidatorResult _validatorResult = Validador.Validate();
            if(_validatorResult == ValidatorResult.Victory)
            {
                Debug.Log("Correct!!");
                StartCoroutine(Victory());
                if(nextChallenge != null)
                {
                    nextChallenge.gameObject.SetActive(true);
                    gameObject.SetActive(false);
                }
            }
            else
            {
                Debug.Log("Wrong!!");
                StartCoroutine(Lose());
            }
        }
    }
}