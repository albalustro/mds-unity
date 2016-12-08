using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MDS.Core.Interfaces;
using MDS.Validators.Interfaces;
using UnityEngine;

namespace MDS.Core.ProcessActivator
{
    public class ProcessAnswerCounter : BaseValidationActivator
    {

        [SerializeField]
        private int _amount;

        [SerializeField]
        private string _label;

        [SerializeField]
        private IValidatableGroup[] _group;

		[SerializeField]
		private int[] _groupCurrentAmount;

        void Start()
        {
            Enable();
			_groupCurrentAmount = new int[_group.Length];
        }

        public void Update()
        {

            if(_isEnabled ==  false)
                return;


			int count = 0;

			for (int i = 0; i < _group.Length; i++) {
				_groupCurrentAmount[i] = _group [i].Count (_label);
				count += _groupCurrentAmount [i];
			}

			if (count == _amount)
            {
                Disable();
                FireValidationEvent();
            }
        }

    }
}
