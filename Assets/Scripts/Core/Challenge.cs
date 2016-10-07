using UnityEngine;
using System.Collections;

namespace MDS.Core
{
    public class Challenge : MDSBehaviour
    {

        // ações executadas automaticamente quando
        // entra no desafio (ANTES do desafio ser jogado)
        // ex: sequencias de dialogos, animacoes, etc
        public IAction[] preActionsList;

        // validadores do desafio
        public IValidator[] validatorsList;

        // ações executadas quando obtem sucesso na valicao
        public IAction[] posVictoryActionsList;


        // ações executadas quando obtem falhar no desafio
        //public IAction[] posVictoryActionsList;


        public Challenge nextChallenge;

        public ProcessAnswerButton btn;

        public void Update()
        {
            if(validatorsList[0].ReadyToValidate())
                btn.Enable();
            else
                btn.Disable();

        }
    }
}