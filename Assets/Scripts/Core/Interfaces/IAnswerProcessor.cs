using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MDS.Core.Interfaces
{

    public interface IAnswerProcessor
    {

        event ProcessAnswerDelegate OnProcessAnswer;

        void Enable();
        void Disable();
    }
}
