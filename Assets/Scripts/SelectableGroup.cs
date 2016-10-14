using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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
        public List<string> _selectedList;

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

        public List<string> GetSelectables()
        {
            foreach (var item in _selectables)
            {
                if (item.chose)
                    _selectedList.Add(item.name);
            }
            if (_selectedList.Count > 0)
                return _selectedList;
            return null;
        }

        internal bool HasAnyoneSelected()
        {
            for (int i = 0; i < _selectables.Length; i++)
            {
                if (_selectables[i].chose)
                    return true;
            }
            return false;
        }


        public void SelectMe(Selectable s)
        {
            if(selectableType == SelectableType.Single)
                SelectMeSingle(s);

            if (selectableType == SelectableType.Multiple)
                SelectMultiple(s);
        }

        private void SelectMultiple(Selectable s)
        {
            s.chose = !s.chose;
        }

        private void SelectMeSingle(Selectable s)
        {
            for (int i = 0; i < _selectables.Length; i++)
            {
                if (_selectables[i] != s)
                    _selectables[i].SetUnselected();
                else
                {
                    if (_selectables[i].Selected)
                    {
                        _selectedIndex = i;
                    }
                    else
                    {
                        _selectedIndex = -1;
                    }
                }
                    
            }
        }
    }
}