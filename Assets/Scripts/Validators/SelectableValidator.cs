
using System.Collections.Generic;
using System.Linq;
using MDS.Core;
using UnityEngine;

namespace MDS.Validators
{
    public class SelectableValidator : MDSBehaviour, IValidator
    {
        public SelectableGroup selectableGroup;
        public List<string> validLabelList;

        public bool ReadyToValidate()
        {
            return selectableGroup.GetSelectable() != null;
        }

        public ValidatorResult Validate()
        {
            if(!ReadyToValidate())
                return ValidatorResult.NotEnoughParameters;

            List<Selectable> selectables = selectableGroup.GetSelectables();

            foreach(var item in selectables)
            {
                if(item.Labels != null)
                {
                    if(item.Labels.Count() == 1)
                    {
                        if(!validLabelList.Contains(item.Labels[0]))
                            return ValidatorResult.Error;
                    }
                    else
                    {
                        // TODO: quando um item tiver mais de um label, o validador 
                        // devera saber como comparar... 
                        Debug.LogWarning("[SelectableValidator] Multiplos labels no item **NÃO IMPLEMENTADO**");
                    }
                }
            }

            return ValidatorResult.Victory;

        }
    }

}