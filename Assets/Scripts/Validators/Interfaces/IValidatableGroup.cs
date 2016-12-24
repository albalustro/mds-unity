using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MDS.Validators.Enum;

namespace MDS.Validators.Interfaces
{
    public interface IValidatableGroup : IValidatable
    {

        OperationLogic OperationLogic { get; set; }
        bool AcceptEmptyAsCorrectAnswer { get; set; }
        int? SpecificAmount { get; set; }

        bool Overwritten { get; set; }

		bool isGeneric { get; set; }

        int Count(string label);

    }
}
