using UnityEngine;
using System.Collections;


namespace MDS.Gameplay.FSM
{
    [RequireComponent(typeof(BoxCollider2D), typeof(SpriteRenderer))]
    public class ChangeOnClickState : MDSBehaviour
    {

        private RandomFSM _fsm;

        public GameObject _nextState;

        void Start()
        {
            _fsm = GetComponentInParent<RandomFSM>();
            if(_fsm == null)
            {
                LogError("RandomFSMState nao está como filho de um RandomFSM");
            }
        }

        public void OnMouseUp()
        {
            _fsm.SetState(_nextState);
        }
    }
}
