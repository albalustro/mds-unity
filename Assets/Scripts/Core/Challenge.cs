using System;
using System.Collections;
using MDS.Validators.Enum;
using MDS.Validators.Interfaces;
using UnityEngine;

namespace MDS.Core
{
    [Serializable]
    public class SolveChallenge : IAction
    {
        public IEnumerator Execute(Action callback)
        {
            throw new NotImplementedException();
        }
    }

    [Serializable]
    public class OpenDialogue : IAction
    {
        public Slug[] slugs { get; set; }
        public IEnumerator Execute(Action callback)
        {
            DialogueSystem.instance.ShowDialogueMessage(slugs);
            yield return new WaitWhile(DialogueSystem.instance.IsDialogueOpen);
        }
    }

    [Serializable]
    public class ErrorHandlerr : IAction
    {
        public IAction[] actions;
        public IEnumerator Execute(Action callback)
        {
            foreach (var item in actions)
            {
                yield return item.Execute();
            }
        }
    }

    public class Challenge : MDSBehaviour
    {
        // ações executadas automaticamente quando
        // entra no desafio (ANTES do desafio ser jogado)
        // ex: sequencias de dialogos, animacoes, etc
        public IAction[] preActionsList;

        // validadores do desafio
        public IValidator[] validatorsList;

        public IAction[] errorActionList;

        // ações executadas quando obtem sucesso na valicao
        public IAction[] posVictoryActionsList;

        // ações executadas quando obtem falhar no desafio
        //public IAction[] posVictoryActionsList;

        public Challenge nextChallenge;

        public ProcessAnswerButton btn;

        protected override void Awake()
        {
            base.Awake();
            btn.processAnswerEvent += ProcessResult;
        }

        public IEnumerator Start()
        {
            foreach (var item in preActionsList)
            {
                yield return item.Execute();
            }
        }

        public void Update()
        {
            if (validatorsList[0].ReadyToValidate())
                btn.Enable();
            else
                btn.Disable();
        }

        IEnumerator Victory()
        {
            foreach (var item in posVictoryActionsList)
            {
                yield return item.Execute();
            }
        }

        IEnumerator Lose()
        {
            foreach (var item in errorActionList)
            {
                yield return item.Execute();
            }
        }

        void ProcessResult()
        {
            ValidatorResult _validatorResult = validatorsList[0].Validate();
            if (_validatorResult == ValidatorResult.Victory)
            {
                StartCoroutine(Victory());
            }
            else
            {
                StartCoroutine(Lose());
            }
        }
    }
}