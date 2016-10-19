using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace MDS
{
    public class FSMGroup : MDSBehaviour
    {

        public enum Multiplicity
        {
            Single,
            Multiple
        }

        public Multiplicity multiplicity;

        public FSM[] _fsm;
        

        void Start()
        {
            for(int i = 0; i < _fsm.Length; i++)
            {
                _fsm[i].SetGroup(this);
            }
        }

        //public Selectable GetSelectable()
        //{
        //    return _fsm.FirstOrDefault(s => s.Selected);
        //}

        public List<FSM> GetAllSelected()
        {
            return _fsm.Where(s => s.Selected).ToList();
        }

        public bool HasAnyoneSelected()
        {
            return _fsm.Any(s => s.Selected);
        }


        public void SelectItem(FSM s)
        {
            if(multiplicity == Multiplicity.Single)
                UnselectOthers(s);            
        }


        private void UnselectOthers(FSM s)
        {
            for (int i = 0; i < _fsm.Length; i++)
            {
                if (_fsm[i] != s)
                    _fsm[i].SetUnselected();
            }
        }
    }
}