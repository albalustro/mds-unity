using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MDS.Core.Interfaces
{

    public interface IValidationActivator
    {

        event ValidateAnswerDelegate OnValidateAnswer;

        void Enable();
        void Disable();
    }
}
