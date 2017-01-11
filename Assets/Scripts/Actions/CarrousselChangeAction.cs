using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MDS.Actions
{
    public class CarrousselChangeAction : BaseAction
    {
        [SerializeField]
        private CarrousselItem _carrousselItem;

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

            if(_freeze)
                _carrousselItem.Freeze();

            while(_carrousselItem.GetCurrentValue().Equals(_value)==false)
            {
                _carrousselItem.MoveNext(_transitionTime);
                yield return new WaitForSeconds(_transitionTime);
            }

        }

    }
}