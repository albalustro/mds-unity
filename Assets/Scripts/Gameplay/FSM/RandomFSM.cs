using UnityEngine;


namespace MDS.Gameplay.FSM
{

    public class RandomFSM : MDSBehaviour
    {

        private GameObject _currentState;

        #region Unity 

        void Start()
        {
            for(int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
            _currentState = transform.GetChild(0).gameObject;
            _currentState.SetActive(true);
        }



        #endregion

        #region Methods

        public void SetState(GameObject _nextState)
        {
            _currentState.SetActive(false);
            _currentState = _nextState;
            _currentState.SetActive(true);
        }

        #endregion


    }
}