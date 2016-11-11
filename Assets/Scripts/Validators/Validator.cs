
using System.Collections.Generic;
using System.Linq;
using MDS.Validators.Enum;
using MDS.Validators.Interfaces;
using UnityEngine;

namespace MDS.Validators
{
    public class Validator:  MDSBehaviour, IValidator 
    {
        /// <summary>
        /// Operacao logica a ser executada sobre todos os elementos
        /// do vetor de regras Rules
        /// </summary>
        public OperationLogic OperationLogic;

        /// <summary>
        /// Vetor de regras que serão validadas
        /// </summary>
        public List<ValidationRule> Rules;

        /// <summary>
        /// Verifica se as condições mínimas para tentar uma validação
        /// estão satisfeitas
        /// </summary>
        /// <returns>True caso já tenha condições de tentar validar</returns>
        public bool ReadyToValidate()
        {
            bool ret = false;

            switch(OperationLogic)
            {
                case OperationLogic.AND:
                    ret = Rules.All(r => r.ValidatableObject.ReadyToValidate());
                    break;

			case OperationLogic.OR:
				ret = Rules.Any (r => r.ValidatableObject.ReadyToValidate ());
				//Debug.Log (ret);
                    break;
            }

            return ret;
        }

        /// <summary>
        /// Metodo que tenta validar cada uma das regras do vetor de Rules
        /// Uma chamada a esse método deve 'queimar' uma tentativa de acerto
        /// </summary>
        /// <returns>Victory, Error ou NotEnoughParameters</returns>
        public ValidatorResult Validate()
        {
            if(!ReadyToValidate())
                return ValidatorResult.NotEnoughParameters;


            switch(OperationLogic)
            {
                case OperationLogic.AND:
                    if(Rules.All(r => r.IsSatisfied()))
                        return ValidatorResult.Victory;
                    break;

                case OperationLogic.OR:
                    if(Rules.Any(r => r.IsSatisfied()))
                        return ValidatorResult.Victory;
                    break;
            }


            return ValidatorResult.Error;
        }

    }



}