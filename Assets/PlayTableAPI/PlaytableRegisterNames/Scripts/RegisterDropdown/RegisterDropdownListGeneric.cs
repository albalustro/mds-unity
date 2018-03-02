using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Playmove
{
    public class RegisterDropdownListGeneric : PYOpenable
    {
        [Header("RegisterDropdownListGeneric")]
        [SerializeField]
        private AnimationCurve _animationCurve;

        [SerializeField]
        protected PYTextBox _labelText;
        public string LabelText
        {
            get { return _labelText.Text; }
            set { _labelText.Text = value; }
        }
        [SerializeField]
        protected Renderer _mask;

        [SerializeField]
        protected Transform ItensHolder;
        [SerializeField]
        protected int _listQuantity = 50;//, _listMaxSize = 50;
        [SerializeField]
        protected float _elementsDistance = 0.6f;

        public bool IsOpen { get { return State == OpenableState.Opened || State == OpenableState.Opening; } }

        [Header("Item of the list")]
        [SerializeField]
        protected RegisterDropdownItem ListItemPrefab;

        #region props

        protected ContentDrag ContentDrag { get { return gameObject.GetComponent<ContentDrag>(); } }
        protected ContentControl ContentControl { get { return GetComponent<ContentControl>(); } }

        protected PYTweenAnimation _windowAnimation;
        protected PYTweenAnimation WindowAnimation
        {
            get
            {
                if (!_windowAnimation)
                    _windowAnimation = PYTweenAnimation.AddNew(gameObject, 999)
                        .SetDuration(0.4f)
                        .SetCurve(_animationCurve)
                        //.SetEaseType(Ease.Type.OutElastic)
                        .SetScale(new Vector3(1, 0.65f, 1), Vector3.one);
                return _windowAnimation;
            }
        }

        protected PYTweenAnimation _listEnterAnimation;
        protected PYTweenAnimation ListEnterAnimation
        {
            get
            {
                if (!_listEnterAnimation)
                    _listEnterAnimation = PYTweenAnimation.AddNew(ItensHolder.gameObject, 888)
                        .SetDuration(0.5f)
                        .SetEaseType(Ease.Type.Linear);
                return _listEnterAnimation;
            }
        }
        protected PYTweenAnimation _listExitAnimation;
        protected PYTweenAnimation ListExitAnimation
        {
            get
            {
                if (!_listExitAnimation)
                    _listExitAnimation = PYTweenAnimation.AddNew(ItensHolder.gameObject, 777)
                        .SetDuration(0.5f)
                        .SetEaseType(Ease.Type.Linear);
                return _listExitAnimation;
            }
        }

        protected Transform _maskTransform;
        protected Transform MaskTransform
        {
            get
            {
                if (!_maskTransform)
                    _maskTransform = transform.Find("Mask");
                return _maskTransform;
            }
        }
        protected Renderer MaskRenderer
        {
            get { return MaskTransform.GetComponent<Renderer>(); }
        }
        protected Vector3 _maskSize
        {
            get
            {
                return MaskRenderer.bounds.size;
            }
        }

        #endregion

        protected List<string> _filterList = new List<string>();

        #region Events
        [Serializable]
        public class RegisterEvent : UnityEvent<RegisterDropdownItem> { }
        [Header("Events")]
        [SerializeField]
        protected RegisterEvent _onItemClicked = new RegisterEvent();
        public RegisterEvent onItemClicked
        {
            get { return _onItemClicked; }
            set { _onItemClicked = value; }
        }
        #endregion

        protected List<RegisterDropdownItem> _createdItens = new List<RegisterDropdownItem>();
        protected Vector3 _itensHolderPosition;

        #region Unity
        protected virtual void Update()
        {
            if (Input.GetKey(KeyCode.N) && Input.GetKeyDown(KeyCode.U))
            {
                Reposition();
                CalculateDragLimit();
            }
        }
        #endregion

        public virtual void Initialize() { }

        public virtual void ClearLabel()
        {
            LabelText = "";
        }

        public virtual void AddFilter(string name)
        {
            if (!_filterList.Contains(name))
                _filterList.Add(name);
        }

        public virtual void RemoveFilter(string name)
        {
            _filterList.Remove(name);
        }

        public virtual void ClearFilter()
        {
            _filterList = new List<string>();
        }

        public string GetItemText(int index)
        {
            return _createdItens[index].Text;
        }

        protected void OpenWindow()
        {
            WindowAnimation.Stop();
            WindowAnimation
                .SetDuration(0.4f)
                .SetEaseType(Ease.Type.OutElastic);
            WindowAnimation.Play(CalculateDragLimit);
        }

        protected Vector3 GetPosition(int index)
        {
            return (ItensHolder.position + (Vector3.down * index * _elementsDistance));
        }

        protected void AnimateListEnter(Action callback = null)
        {
            ListEnterAnimation.Stop();
            ListExitAnimation.Stop();
            ListEnterAnimation
                .SetDuration(0.2f)
                .SetAlpha(0f, 1f)
                .SetPosition(_itensHolderPosition + Vector3.down * 5, _itensHolderPosition, true)
                .Play(callback);
        }

        protected void AnimateListExit(Action callback)
        {
            ListEnterAnimation.Stop();
            ListExitAnimation.Stop();
            ListExitAnimation
                .SetDuration(0.4f)
                .SetAlpha(1f, 0f)
                .SetPosition(ItensHolder.transform.localPosition, ItensHolder.transform.localPosition + Vector3.up * 15, true)
                .Play(callback);
        }

        protected void CalculateDragLimit()
        {
            ContentDrag.StartPosition = ItensHolder.transform.position.y;
            int activeObjCount = _createdItens.FindAll(i => i.gameObject.activeSelf).Count;

            if (activeObjCount < 7)
            {
                ContentDrag.MaxDistance = -0.001f;
                ContentControl.TimesBigger = 1;
            }
            else
            {
                ContentDrag.MaxDistance = -_elementsDistance * (activeObjCount - 7);
                ContentControl.TimesBigger = (activeObjCount * _elementsDistance) / (_elementsDistance * 7);
            }
        }

        protected void Reposition()
        {
            int i = 0;
            foreach (RegisterDropdownItem item in _createdItens)
            {
                item.transform.position = ItensHolder.position + (Vector3.down * i * _elementsDistance);
                i++;
            }
        }
    }
}