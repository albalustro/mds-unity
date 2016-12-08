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
		private UnityEngine.Object _target;

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

			if (_target is GameObject)
			{
				(_target as GameObject).SetActive(en);
			}   
			else if (_target is Behaviour)
			{
				(_target as Behaviour).enabled = en;
			}
			else if (_target as Renderer){
				(_target as Renderer).enabled = en;
			}

		

		}

	  
	}


}