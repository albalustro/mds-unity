using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MDS.Actions
{
    public class CarrousselChangeAction : BaseAction
    {
        [SerializeField]
        private CarrousselItem[] _carrousselItem;

        [SerializeField]
        private string _value;

        [SerializeField]
        private bool _freeze;

        [SerializeField]
        private float _transitionTime = .2f;

        public override IEnumerator Execute()
        {
            if(byPass)yield break;

            yield return base.Execute();

			for (int i = 0; i < _carrousselItem.Length; i++) {
				if(_freeze)
					_carrousselItem[i].Freeze();

				while(_carrousselItem[i].GetCurrentValue().Equals(_value)==false)
				{
					_carrousselItem[i].MoveNext(_transitionTime);
					yield return new WaitForSeconds(_transitionTime);
				}
			}

            

        }

    }
}