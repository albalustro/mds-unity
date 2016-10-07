using UnityEngine;
using System.Collections;
using System;

namespace MDS
{
    public class SelectableGroup : MDSBehaviour
    {

        public enum SelectableType
        {
            Single,
            Multiple
        }

        public SelectableType selectableType;

        public Selectable[] _selectables;
        private int _selectedIndex;

        void Start()
        {
            for(int i = 0; i < _selectables.Length; i++)
            {
                _selectables[i].SetGroup(this);
            }
            _selectedIndex = -1;
        }

        public Selectable GetSelectable()
        {
            if(_selectedIndex == -1)
                return null;

            return _selectables[_selectedIndex];
        }

        public void SelectMe(Selectable s)
        {
            if(selectableType == SelectableType.Single)
                SelectMeSingle(s);

        }

        private void SelectMeSingle(Selectable s)
        {
            for(int i = 0; i < _selectables.Length; i++)
            {
                if(_selectables[i] != s)
                    _selectables[i].SetUnselected();
                else
                    _selectedIndex = i;
            }
        }
    }
}