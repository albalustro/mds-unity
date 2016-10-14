using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

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
        

        void Start()
        {
            for(int i = 0; i < _selectables.Length; i++)
            {
                _selectables[i].SetGroup(this);
            }
        }

        public Selectable GetSelectable()
        {
            return _selectables.FirstOrDefault(s => s.Selected);
        }

        public List<Selectable> GetSelectables()
        {
            return _selectables.Where(s => s.Selected).ToList();
        }

        public bool HasAnyoneSelected()
        {
            return _selectables.Any(s => s.Selected);
        }


        public void SelectItem(Selectable s)
        {
            if(selectableType == SelectableType.Single)
                UnselectOthers(s);            
        }


        private void UnselectOthers(Selectable s)
        {
            for (int i = 0; i < _selectables.Length; i++)
            {
                if (_selectables[i] != s)
                    _selectables[i].SetUnselected();
            }
        }
    }
}