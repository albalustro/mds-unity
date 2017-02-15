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
using FullInspector;
using MDS.Gameplay.DragDrop;

using MDS.Utilities;
using MDS.Core.SceneManagement;

namespace MDS.Core
{

    public delegate void ValidateAnswerDelegate();


    public class Challenge : MDSBehaviour
    {

#if UNITY_EDITOR

        void OnGUI()
        {
            GUILayout.BeginVertical();

            if(GUILayout.Button("Start", GUILayout.Width(100), GUILayout.Height(80)))
                ExecuteActionsStart();

            if(GUILayout.Button("Error 1", GUILayout.Width(100), GUILayout.Height(80)))
                ExecuteActions1();

            if(GUILayout.Button("Error 2", GUILayout.Width(100), GUILayout.Height(80)))
                ExecuteActions2();

            if(GUILayout.Button("Error 3", GUILayout.Width(100), GUILayout.Height(80)))
                ExecuteActions3();

            if(GUILayout.Button("Victory", GUILayout.Width(100), GUILayout.Height(80)))
                ExecuteActionsVic();

            if (GUILayout.Button("Random Draggable Populate", GUILayout.Width(100), GUILayout.Height(80)))
            {
                var initGroup = FindObjectOfType<InitialDropGroupArea>();

                List<BaseDropGroupArea> others = FindObjectsOfType<BaseDropGroupArea>().ToList();
                others.Remove(initGroup);

                foreach(var slot in initGroup.GetComponentsInChildren<DropGroupSlot>())
                {
                    if (slot.draggableReference!=null)
                    {

                        IAction[] xpto = new IAction[1];
                        xpto[0] = new SetInSlotAction(slot.draggableReference, others.GetRandom());
                        ExecuteActions(xpto);

                    }
                }
            }


            GUILayout.EndVertical();
        }

       
        private void ExecuteActionsStart()
        {
            ExecuteActions(_actions.onStartActions);
        }
        
        private void ExecuteActions1()
        {
            ExecuteActions(_actions.onErrorActions_1);
        }
       
        private void ExecuteActions2()
        {
            ExecuteActions(_actions.onErrorActions_2);
        }
        
        private void ExecuteActions3()
        {
            ExecuteActions(_actions.onErrorActions_3);
        }
      
        private void ExecuteActionsVic()
        {
            ExecuteActions(_actions.onVictoryActions);
        }

#endif


        // classe auxiliar (apenas para organizar o inspector)
        public class ChallengeActions
        {
            // ações executadas automaticamente quando
            // entra no desafio (ANTES do desafio ser jogado)
            // ex: sequencias de dialogos, animacoes, etc
            [InspectorCollectionShowItemDropdown(IsCollapsedByDefault = true)]
            public IAction[] onStartActions;

            // ações executadas quando responde errado na primeira vez
            [InspectorCollectionShowItemDropdown(IsCollapsedByDefault = true)]
            public IAction[] onErrorActions_1;

            // ações executadas quando responde errado na primeira vez
            [InspectorCollectionShowItemDropdown(IsCollapsedByDefault = true)]
            public IAction[] onErrorActions_2;

            // ações executadas quando responde errado na primeira vez
            [InspectorCollectionShowItemDropdown(IsCollapsedByDefault = true)]
            public IAction[] onErrorActions_3;

            // ações executadas quando obtem sucesso na valicao
            [InspectorCollectionShowItemDropdown(IsCollapsedByDefault = true)]
            public IAction[] onVictoryActions;
        }

        #region Fields & Properties

        // Propriedade usada para rastrear o conceito obtido pelo jogador
        // ao jogar um desafio.
        // É iniciado com GREEN (no SceneLoader), antes de abrir a cena do desafio e, ao sair,
        // será verificado o seu valor para atualizacao do mapa de conceitos 
        // do PlayerInfo. (tambémo no sceneLoader, no metodo GoBackAfterChallenge)
        public static ConceptTypes ChallengeConcept;

        public IValidator Validador;
        public Challenge nextChallenge;
        public ChallengeActions _actions;

        private IValidationActivator[] _answerProcessors;
        private int _errorCount = 0;

       
        private bool _isExecutingActions;
        private bool _isInVictoryCondition;

        public bool IsExecutingActions() { return _isExecutingActions; }
        public bool IsInVictoryCondition() { return _isInVictoryCondition; }

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
                    //item.Enable(); 
                }
            }
            
            ExecuteActions(_actions.onStartActions);

            _isInVictoryCondition = false;
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
            _isInVictoryCondition = true;

            List<IAction> tmp = new List<Interfaces.IAction>(_actions.onVictoryActions);

            // se tem um proximo challenge, inserir as actions para desabilitar/habilitar
            // os challenges relacionados
            if(nextChallenge != null)
            {
                EnableDisableAction ac1 = new EnableDisableAction(EnableDisableAction.EAction.Enable, new[] { nextChallenge.gameObject }) ;
                EnableDisableAction ac2 = new EnableDisableAction(EnableDisableAction.EAction.Disable, new[] { gameObject });
                tmp.Add(ac1);
                tmp.Add(ac2);                
            }
            // caso contrario, colocar a actions que termina a cena
            else
            {
                FinishChallenge ac = new FinishChallenge();
                tmp.Add(ac);
				if (AudioController.Instance != null && SFXController.instance != null) {
					PlaySFX (SFXController.instance._won);
				}
            }

            ExecuteActions(tmp.ToArray());
            
        }

        /// <summary>
        /// Esse metodo irá executar as actions definidas para um dado erro.
        /// </summary>
        void Lose(int index)
        {
            List<IAction> actions = new List<IAction>();

			if (AudioController.Instance != null && SFXController.instance != null) {
				PlaySFX (SFXController.instance._inputClick);
			}

            switch(index)
            {
                case 1:
                    actions.AddRange( _actions.onErrorActions_1);
                    break;
                case 2:
                    actions.AddRange( _actions.onErrorActions_2);
                    break;
                case 3:
                    actions.AddRange( _actions.onErrorActions_3);
                    break;
                default:
                    LogError("Error Index não definido");
                    return;
                    
            }

            // colocando conceito amarelo para qq erro será sobrescrito
            // pela action que coloca conceito vermelho.
            // dessa forma nao precisa se preocupar se o desafio tem 1, 2 ou 3 erros
            // ou ainda se há multiplos challenges
            Challenge.ChallengeConcept = ConceptTypes.CONCEPT_YELLOW;
                                   
            ExecuteActions(actions.ToArray());

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

        protected override IEnumerator exec(IAction[] a)
        {
            _isExecutingActions = true;
            yield return base.exec(a);
            _isExecutingActions = false;
        }

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