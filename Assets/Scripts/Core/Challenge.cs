using System.Linq;
using System.Collections;
using MDS.Core.Interfaces;
using MDS.Core.ProcessActivator;
using MDS.Validators.Enum;
using MDS.Validators.Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;
using MDS.Actions;
using System;
using System.Collections.Generic;

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
            {
                nextChallenge.gameObject.SetActive(false);
            }

        }

        public void Start()
        {

            _answerProcessors = FindObjectsOfType<BaseValidationActivator>();
            if(_answerProcessors == null || _answerProcessors.Length == 0)
            {
                LogWarning("Nenhum 'CheckAnswer *' foi encontrado. Certifique-se de ter ao menos habilitado antes de habilitar o Challenge");
            }
            else
            {
                foreach(var item in _answerProcessors)
                {
                    item.OnValidateAnswer += ProcessResult;
                }
            }

            ExecuteActions(_actions.onStartActions);

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

        void Victory()
        {
            if(nextChallenge != null)
            {
                //nextChallenge.gameObject.SetActive(true);
                //gameObject.SetActive(false);
                EnableDisableAction ac1 = new EnableDisableAction(EnableDisableAction.EAction.Enable, new[] { nextChallenge.gameObject }) ;
                EnableDisableAction ac2 = new Actions.EnableDisableAction(EnableDisableAction.EAction.Disable, new[] { gameObject });
                List<IAction> tmp = new List<Interfaces.IAction>(_actions.onVictoryActions);
                tmp.Add(ac1);
                tmp.Add(ac2);
                _actions.onVictoryActions = tmp.ToArray();
            }
            ExecuteActions(_actions.onVictoryActions);
        }

        /// <summary>
        /// Esse metodo irá executar as actions definidas para um dado erro.
        /// </summary>
        void Lose(int index)
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
                    return;
                    break;
            }

            ExecuteActions(actions);
        }

        public void ProcessResult()
        {

            ValidatorResult _validatorResult = Validador.Validate();

            if(_validatorResult == ValidatorResult.Victory)
            {
                Log("Correct!!");
                Victory();
            }
            else
            {
                Log("Wrong!!");
                _errorCount++;
                Lose(_errorCount);
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
            return GetActiveInstance().Validador;
        }

        public static Challenge GetActiveInstance()
        {
            Challenge[] challenges = FindObjectsOfType<Challenge>();
            Challenge currentChallenge;
            if(challenges.Length == 1)
                currentChallenge = challenges[0];
            else
                currentChallenge = challenges.First(c => c.isActiveAndEnabled);
            return currentChallenge;
        }

        #endregion
    }
}