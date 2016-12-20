using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using MDS.Validators.Interfaces;
using FullInspector;
using MDS.Validators.Enum;

namespace MDS.Gameplay.Selectable
{
    public class SelectableGroup : MDSBehaviour, IValidatableGroup, IValidatable
    {
        public enum GroupSelectionType
        {
            Single,
            Multiple
        }

        [SerializeField, InspectorTooltip("Propriedade para indicar quantos elementos podem estar selecionados simultaneamente")]
        private int? _maxSelected;
        private int _curSelectedAmount;

        #region IValidatableGroup

        [InspectorMargin(10)]

        public bool Overwritten { get; set; }

        [InspectorHideIf("Overwritten")]
        public OperationLogic OperationLogic { get; set; }

        [InspectorHideIf("Overwritten")]
        public bool AcceptEmptyAsCorrectAnswer { get; set; }

        [InspectorHideIf("Overwritten")]
        public int? SpecificAmount { get; set; }

        
        /// <summary>
        /// Retorna quantidade de elementos *selecionados* e que contem o label do parametro
        /// </summary>
        public int Count(string label)
        {
            if(string.IsNullOrEmpty(label))
                return _selectables.Count(s => s.Selected);

            return _selectables.Count(s => s.Selected && s.Labels.Contains(label));
        }

        #endregion

        public GroupSelectionType groupSelectionType;
        public Selectable[] _selectables;
        
        void Start()
        {
            SetupSelectables();
        }

        [InspectorButton, InspectorTooltip("Use esse recurso para atribuir os filhos no vetor de Selectables")]
        public void SetupSelectables()
        {
            _selectables = transform.GetComponentsInChildren<Selectable>(true);
            foreach(var s in _selectables)
            {
                s.SetGroup(this);
            }
        }

        public Selectable GetSelectedSingle()
        {
            return _selectables.FirstOrDefault(s => s.Selected);
        }

        public List<Selectable> GetSelectedMultiple()
        {
            return _selectables.Where(s => s.Selected).ToList();
        }

        public bool HasAnyoneSelected()
        {
            return _selectables.Any(s => s.Selected);
        }

        public void SelectItem(Selectable s)
        {
            if(groupSelectionType == GroupSelectionType.Single)
                UnselectOthers(s);     
            else
            {
                if (_maxSelected.HasValue)
                {

                }
            }       
        }

        private void UnselectOthers(Selectable s)
        {
            for (int i = 0; i < _selectables.Length; i++)
            {
                if(_selectables[i] != s)
                    _selectables[i].SetSelected(false, false);
            }
        }

        internal bool CanSelect()
        {
            if(groupSelectionType == GroupSelectionType.Multiple)
            {
                if(!_maxSelected.HasValue)
                    return true;

                int curSelAmount = GetSelectedMultiple().Count;

                return curSelAmount < _maxSelected.Value;
                
            }

            return true;
        }

        #region IValidatable

        public bool ReadyToValidate()
        {
            if(AcceptEmptyAsCorrectAnswer)
                return true;

            return HasAnyoneSelected();
        }

        public bool Validate(string acceptableAnswer)
        {
            bool ret = false;

            switch(groupSelectionType)
            {
                case GroupSelectionType.Single:
                    int selectedAmount = GetSelectedMultiple().Count;
                    if(selectedAmount == 0 && AcceptEmptyAsCorrectAnswer)
                    {
                        ret = true;
                    }
                    else if(selectedAmount > 1)
                    {
                        ret = false;
                    }
                    else
                    {
                        if(HasAnyoneSelected())
                            ret = GetSelectedSingle().Validate(acceptableAnswer);
                        else
                            ret = false;
                    }
                    break;

                case GroupSelectionType.Multiple:

                    List<Selectable> temp = GetSelectedMultiple();

                    switch(OperationLogic)
                    {
                        case OperationLogic.AND:
                            ret = temp.All(s => s.Validate(acceptableAnswer));
                            break;
                        case OperationLogic.OR:
                            ret = temp.Any(s => s.Validate(acceptableAnswer));
                            break;
                    }


                    if(ret && SpecificAmount.HasValue)
                    {
                        int qtde = temp.Where(s => s.Validate(acceptableAnswer)).ToList().Count;
                        ret = (qtde == SpecificAmount.Value);
                    }

                    break;
            }



            return ret;


        }

        public int? GetNumericValue()
        {
            bool hasResult = false;
            int result = 0;
            int? temp;
            foreach(var s in _selectables)
            {
                temp = s.GetNumericValue();
                if(temp.HasValue)
                {
                    result += temp.Value;
                    hasResult = true;
                }

            }
            if(hasResult)
                return result;
            return null;
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        #endregion

    }
}