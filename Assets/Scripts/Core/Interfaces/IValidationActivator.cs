using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MDS.Core.Interfaces
{

    public interface IValidationActivator
    {
        MDSBehaviour Behaviour { get; }

        event ValidateAnswerDelegate OnValidateAnswer;

        void Enable();
        void Disable();
    }
}
