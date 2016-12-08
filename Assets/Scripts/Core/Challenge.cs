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

        // classe auxiliar (apenas para organizar o inspector)
        public class ChallengeActions
        {
            // ações executadas automaticamente quando
            // entra no desafio (ANTES do desafio ser jogado)
            // ex: sequencias de dialogos, animacoes, etc
            public IAction[] onStartActions;

            // ações executadas quando responde errado na primeira vez
            public IAction[] onErrorActions_1;

            // ações executadas quando responde errado na primeira vez
            public IAction[] onErrorActions_2;

            // ações executadas quando responde errado na primeira vez
            public IAction[] onErrorActions_3;

            // ações executadas quando obtem sucesso na valicao
            public IAction[] onVictoryActions;
        }

        #region Fields & Properties

        private int _errorCount = 0;
        
        // validadores do desafio
        public IValidator Validador;

        public Challenge nextChallenge;

        private IValidationActivator[] _answerProcessors;

        public ChallengeActions _actions;

        #endregion

        #region Unity methods

        protected override void Awake()
        {
            base.Awake();

            // Desabilita o próximo challenge, se ele existir
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


            for(int i = 0; i < _actions.onStartActions.Length; i++)
            {
                if(_actions.onStartActions[i].waitFinish)
                    yield return StartCoroutine(_actions.onStartActions[i].Execute());
                else
                    StartCoroutine(_actions.onStartActions[i].Execute());
            }

        }

        public void OnDisable()
        {
            // quando o challenge é desabilidado, tb devemos
            // parar de ouvir o evento que processa o resultado
            if(_answerProcessors == null) return;
            foreach(var item in _answerProcessors)
            {
                item.OnValidateAnswer -= ProcessResult;
            }
        }

        #endregion

        #region Private Methods

        IEnumerator Victory()
        {
            for(int i = 0; i < _actions.onVictoryActions.Length; i++)
            {
                if(_actions.onVictoryActions[i].waitFinish)
                    yield return StartCoroutine(_actions.onVictoryActions[i].Execute());
                else
                    StartCoroutine(_actions.onVictoryActions[i].Execute());
            }
        }

        /// <summary>
        /// Esse metodo irá executar as actions definidas para um dado erro.
        /// </summary>
        IEnumerator Lose(int index)
        {
            IAction[] actions;
            switch(index)
            {
                case 1:
                    actions = _actions.onErrorActions_1;
                    break;
                case 2:
                    actions = _actions.onErrorActions_2;
                    break;
                case 3:
                    actions = _actions.onErrorActions_3;
                    break;
                default:
                    LogError("Error Index não definido");
                    yield break;
                    break;
            }

            for(int i = 0; i < actions.Length; i++)
            {
                if (actions[i]==null)
                {
                    LogError("Action não definida.");
                    continue;
                }

                if(actions[i].waitFinish)
                    yield return StartCoroutine(actions[i].Execute());
                else
                    StartCoroutine(actions[i].Execute());
            }
        }

        void ProcessResult()
        {
            ValidatorResult _validatorResult = Validador.Validate();

            if(_validatorResult == ValidatorResult.Victory)
            {
                Log("Correct!!");
                StartCoroutine(Victory());
                if(nextChallenge != null)
                {
                    nextChallenge.gameObject.SetActive(true);
                    gameObject.SetActive(false);
                }
            }
            else
            {
                Log("Wrong!!");
                _errorCount++;
                StartCoroutine(Lose(_errorCount));
            }
        }

        #endregion

        #region Static methods
        /// <summary>
        /// Metodo estatico que irá buscar o validador do challenge que estiver ativo na cena
        /// </summary>
        /// <returns>O Validador referente ao challenge ativo</returns>
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

        #endregion
    }
}