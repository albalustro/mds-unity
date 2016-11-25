using System.Linq;
using System.Collections;
using MDS.Core.Interfaces;
using MDS.Core.ProcessActivator;
using MDS.Validators.Enum;
using MDS.Validators.Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MDS.Core
{

    public delegate void ValidateAnswerDelegate();


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

        private IValidationActivator[] _answerProcessors;

        public ChallengeActions _actions;


        public static IValidator GetCurrentValidador()
        {
            Challenge[] challenges = FindObjectsOfType<Challenge>();
            Challenge currentChallenge;
            if(challenges.Length == 1)
                currentChallenge = challenges[0];
            else
                currentChallenge = challenges.First(c => c.isActiveAndEnabled);

            return currentChallenge.Validador;
        }


        protected override void Awake()
        {
            base.Awake();

            if(nextChallenge != null)
                nextChallenge.gameObject.SetActive(false);

        }

        public IEnumerator Start()
        {
            
            _answerProcessors = FindObjectsOfType<BaseValidationActivator>();
            if(_answerProcessors == null)
            {
                LogError("ValidationActivator não encontrado");
            }
            foreach(var item in _answerProcessors)
            {
                item.OnValidateAnswer += ProcessResult;
            }
            
            foreach(var item in _actions.onStartActions)
            {
                yield return item.Execute();
            }
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