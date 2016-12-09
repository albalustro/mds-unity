using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MDS.Core;
using MDS.Core.Interfaces;
using UnityEngine;

namespace MDS.Core.ProcessActivator
{
    public abstract class BaseValidationActivator : MDSBehaviour, IValidationActivator
    {

        public event ValidateAnswerDelegate OnValidateAnswer;

        protected bool _isEnabled;

        public virtual void Disable()
        {
            _isEnabled = false;
           
        }

        public virtual void Enable()
        {
            _isEnabled = true;
           
        }

        protected void FireValidationEvent()
        {
            if(OnValidateAnswer != null)
            {
                OnValidateAnswer();
            }
        }
    }
}
