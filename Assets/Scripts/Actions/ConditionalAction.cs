using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FullInspector;
using MDS.Actions.DialogConditions;
using MDS.Core.Interfaces;
using UnityEngine;

namespace MDS.Actions
{
    public class ConditionalAction : BaseAction
    {

        [SerializeField]
        private IActionCondition[] _conditionedActions;

        [SerializeField]
        private bool HasFallback;

        [SerializeField, InspectorShowIf("HasFallback"), InspectorComment("Actions que serão executadas se nenhuma das condições acima for satisfeita")]
        private IAction[] _fallbackActions;

        public override IEnumerator Execute()
        {
            yield return base.Execute();

            IActionCondition actionCondition = _conditionedActions.FirstOrDefault(c => c.IsConditionSatisfied());

            IAction[] actions = null;

            if(actionCondition != null)
                actions = actionCondition.Actions;

            if (actions == null)
            {
                if (HasFallback)
                { 
                    actions = _fallbackActions;
                }
            }

            if (actions == null)
            {
                _corotineHolder.GetComponent<MDSBehaviour>().LogError("Action condicional sem nenhuma condicao satisfeita e tambem sem fallback");
                yield break;
            }

            MDSBehaviour.ExternalExecuteActions(_corotineHolder.GetComponent<MDSBehaviour>(), actions);

        }

    }
}