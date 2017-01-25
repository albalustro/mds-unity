using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MDS.Player
{
    public class PlayerAutoAwayAnimBehaviour : StateMachineBehaviour
    {

        private const string paramAwayName = "away";
        private readonly int paramAwayHash = Animator.StringToHash(paramAwayName);

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool(paramAwayHash, true);
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool(paramAwayHash, false);
        }

    }
}