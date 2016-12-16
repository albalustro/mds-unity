using UnityEngine;
using System.Collections;
using System;
using MDS.Core.Interfaces;

namespace MDS.Actions
{
	public class EnableDisableAction : BaseAction
	{

		public enum EAction
		{
			Enable,
			Disable
		}

		[SerializeField]
		private EAction _action;

		[SerializeField]
		private UnityEngine.Object[] _targets;

        public EnableDisableAction()
        {

        }

        public EnableDisableAction(EAction actionDesired, UnityEngine.Object[] targets)
        {
            _action = actionDesired;
            _targets = targets;
        }

		public override IEnumerator Execute()
		{
            yield return base.Execute();


            bool en = false;

			switch(_action)
			{
				case EAction.Enable:
					en = true;
					break;

				case EAction.Disable:
					en = false;
					break;

			}

            foreach(var target in _targets)
            {
                if(target is GameObject)
                {
                    (target as GameObject).SetActive(en);
                }
                else if(target is Behaviour)
                {
                    (target as Behaviour).enabled = en;
                }
                else if(target as Renderer)
                {
                    (target as Renderer).enabled = en;
                }

            }

		}

	  
	}


}