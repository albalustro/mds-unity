using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MDS.Gameplay.FSM.State.Transition
{
    public interface IStateTransition
    {

        void Initialize(RandomState state);
        void ExecuteTransition();

    }
}
