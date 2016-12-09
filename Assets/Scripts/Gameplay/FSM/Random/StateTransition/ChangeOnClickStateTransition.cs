using UnityEngine;
using System.Collections;
using UnityEngine.Events;

namespace MDS.Gameplay.FSM.State.Transition
{
    public class ChangeOnClickStateTransition : BaseStateTransition
    {

        

        public override void Initialize(RandomState state)
        {
            base.Initialize(state);
            state.ClickEvent += OnMouseUp;
        }

        public void OnMouseUp()
        {
            ExecuteTransition();
        }
    }
}
