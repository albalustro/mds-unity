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
		private bool m_useMemorizedGoAsTarget;

		[SerializeField]
		private UnityEngine.Object[] _targets;

        [SerializeField]
        private bool _includeChidren;
        [SerializeField]
        private bool _onlyChildren;

		[SerializeField]
		private bool m_destroyOnDisable;

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
            if(byPass) yield break;
            yield return base.Execute();


				bool en = false;

				switch (_action) {
				case EAction.Enable:
					en = true;
					break;

				case EAction.Disable:
					en = false;
					break;

				}

				if (m_useMemorizedGoAsTarget) {
					GameObject tmpGo = MemorizeMe.MemorizedGameObject;
					tmpGo.SetActive (en);
				} else {
					foreach (var target in _targets) {
						if (target is GameObject) {
							var p = (target as GameObject).transform;
							if (_includeChidren || _onlyChildren) {
								for (int i = 0; i < p.childCount; i++) {
									p.GetChild (i).gameObject.SetActive (en);
								}
							}
							if (!_onlyChildren)
								p.gameObject.SetActive (en);

						} else if (target is Behaviour) {
							(target as Behaviour).enabled = en;
						} else if (target as Renderer) {
							(target as Renderer).enabled = en;
						}
					}
				}

			if (m_destroyOnDisable) {
				for (int i = 0; i < _targets.Length; i++) {
					LeanTween.Destroy (_targets [i]);
				}

			}

	  
		}
	}

}