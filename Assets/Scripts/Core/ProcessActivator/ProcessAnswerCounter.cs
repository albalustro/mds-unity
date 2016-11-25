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
        private IValidatableGroup _group;

        void Start()
        {
            Enable();
        }

        public void Update()
        {

            if(_isEnabled ==  false)
                return;

            if (_group.Count(_label) == _amount)
            {
                Disable();
                FireValidation();
            }
        }

    }
}
