using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace MDS.Actions
{
    public class CarrousselChangeAction : BaseAction
    {

		[SerializeField]
		private GameObject _carrousselParent;

        [SerializeField]
        private CarrousselItem[] _carrousselItem;

        [SerializeField]
        private string _value;

        [SerializeField]
        private bool _freeze;

		[SerializeField]
		private bool _unfreeze;

        [SerializeField]
        private float _transitionTime = .1f;

		[SerializeField]
		private bool _ignoreFreezed;

		private int _count;

        public override IEnumerator Execute()
        {
            if(byPass)yield break;

            yield return base.Execute();

			if (_carrousselParent != null) {
				_carrousselItem = _carrousselParent.GetComponentsInChildren<CarrousselItem> ();
			}

			if (_ignoreFreezed) {
				_carrousselItem = _carrousselItem.Where (i => !i.isFreezed).ToArray();
			}

			_count = _carrousselItem.Length;

			for (int i = 0; i < _carrousselItem.Length; i++) {
				_corotineHolder.StartCoroutine (InternalExecute(i));
			}

			if(waitFinish)
				yield return new WaitWhile(() => _count > 0);
            

        }

		IEnumerator InternalExecute(int i)
		{
			if(_freeze)
				_carrousselItem[i].Freeze();

			if(_unfreeze)
				_carrousselItem[i].Unfreeze();

			while(_carrousselItem[i].GetCurrentValue().Equals(_value)==false)
			{
				_carrousselItem[i].MoveNext(_transitionTime);
				yield return new WaitForSeconds(_transitionTime);
			}

			--_count;
		}


    }
}