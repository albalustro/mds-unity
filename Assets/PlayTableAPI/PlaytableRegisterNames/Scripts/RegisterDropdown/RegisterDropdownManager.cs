using UnityEngine;
using System.Collections;
using System;

namespace Playmove
{
    public class RegisterDropdownManager : PYOpenable
    {
        [SerializeField]
        private Transform _nameLabel;
        [SerializeField]
        private bool _openListOnOpen = true;

        #region props
        private string _labelText = "Selecione";
        public string LabelText
        {
            get { return _labelText; }
            set
            {
                if (List != null)
                    _labelText = List.LabelText = NameButtonField.Text = value;
            }
        }

        private bool _disableList;
        public bool DisableList
        {
            get { return _disableList; }
            set
            {
                _disableList = value;
                NameButtonField.Button.enabled = !value;
            }
        }

        private RegisterDropdownButtonField _nameButtonField;
        private RegisterDropdownButtonField NameButtonField
        {
            get
            {
                if (!_nameButtonField)
                    _nameButtonField = gameObject.GetComponentInChildren<RegisterDropdownButtonField>(true);
                return _nameButtonField;
            }
        }

        private RegisterDropdownListGeneric _list;
        public RegisterDropdownListGeneric List
        {
            get
            {
                if (!_list) _list = GetComponentInChildren<RegisterDropdownListGeneric>(true);
                return _list;
            }
        }

        private PYTweenAnimation _labelEnterAnimation;
        private PYTweenAnimation LabelEnterAnimation
        {
            get
            {
                if (!_labelEnterAnimation)
                {
                    _labelEnterAnimation = PYTweenAnimation.AddNew(_nameLabel.gameObject, 999)
                        .SetDuration(0.3f)
                        .SetEaseType(Ease.Type.InExpo)
                        .SetAlpha(0f, 1f);
                    if (_nameLabel.position.x < 0)
                        _labelEnterAnimation
                            .SetPosition(_labelOriginalPosition - (_nameLabel.right * 5), _labelOriginalPosition, true);
                    else
                        _labelEnterAnimation
                            .SetPosition(_labelOriginalPosition + (_nameLabel.right * 5), _labelOriginalPosition, true);
                }
                return _labelEnterAnimation;
            }
        }

        private PYTweenAnimation _labelChangeAnimation;
        private PYTweenAnimation LabelChangeAnimation
        {
            get
            {
                if (!_labelChangeAnimation)
                    _labelChangeAnimation = PYTweenAnimation.AddNew(_nameLabel.gameObject, 888)
                                    .SetDuration(0.3f)
                                    .SetEaseType(Ease.Type.InExpo)
                                    .SetPosition(_labelOriginalPosition + (_nameLabel.up * 5), _labelOriginalPosition, true);
                return _labelChangeAnimation;
            }
        }

        #endregion

        public bool IsOpen
        {
            get
            {
                return State == OpenableState.Opened || State == OpenableState.Opening;
            }
        }

        private Vector3 _labelOriginalPosition;

        #region Unity

        private void Awake()
        {
            _labelOriginalPosition = _nameLabel.localPosition;
            _nameLabel.transform.localPosition = _labelOriginalPosition + (_nameLabel.up * 5);

            NameButtonField.Button.onClick.AddListener(OpenDropdownList);
        }

        #endregion

        #region PyOpenable

        /// <summary>
        /// Animação lateral, quando geralmente é usado quando troca de lado
        /// </summary>
        /// <param name="callback"></param>
        public void OpenHorizontal(Action callback = null)
        {
            if (State != OpenableState.Closed) return;
            Open();

            callback += Opened;

            LabelChangeAnimation.Stop();
            LabelEnterAnimation.Stop();
            LabelEnterAnimation.Play(() =>
            {
                NameButtonField.PlayAnimation(null);
            });
            StartCoroutine(OpenListView(callback)); // Feito para acelerar a animação e executar na metade da animação do NameButtonField
        }

        private IEnumerator OpenListView(Action callback, float waitTime = 0.35f)
        {
            yield return new WaitForSeconds(waitTime);
            if (_openListOnOpen)
                List.Open(callback);
            else
                Opened();
        }

        /// <summary>
        /// Vertical animation, when opening and closing the popup, 
        /// </summary>
        public void OpenVertical()
        {
            if (State != OpenableState.Closed) return;
            Open();

            LabelChangeAnimation.Stop();
            LabelEnterAnimation.Stop();

            LabelChangeAnimation.Play(() =>
            {
                NameButtonField.PlayAnimation(() =>
                {
                    if (_openListOnOpen)
                        List.Open(Opened);
                    else
                        Opened();
                });
            });
        }

        public void CloseHorizontal()
        {
            Close();

            if (List.IsOpen)
                List.Close(() =>
                {
                    NameButtonField.ReverseAnimation(() =>
                    {
                        LabelChangeAnimation.Stop();
                        LabelEnterAnimation.Stop();
                        LabelEnterAnimation.Reverse(Closed);
                    });
                });
            else
                NameButtonField.ReverseAnimation(() =>
                {
                    LabelChangeAnimation.Stop();
                    LabelEnterAnimation.Stop();
                    LabelEnterAnimation.Reverse(Closed);
                });

            ClearLabel();
        }

        public void CloseVertical()
        {
            if (State != OpenableState.Opened) return;
            Close();

            if (List.IsOpen)
                List.Close(() =>
                {
                    NameButtonField.ReverseAnimation(() =>
                    {
                        LabelChangeAnimation.Stop();
                        LabelEnterAnimation.Stop();
                        LabelChangeAnimation.Reverse(Closed);
                    });
                });
            else
                NameButtonField.ReverseAnimation(() =>
                {
                    LabelChangeAnimation.Stop();
                    LabelEnterAnimation.Stop();
                    LabelChangeAnimation.Reverse(Closed);
                });
        }

        #endregion

        public void ClearLabel()
        {
            LabelText = "";
        }

        private void OpenDropdownList(PYButton btn)
        {
            if (!DisableList)
                if (List.IsOpen)
                    List.Close();
                else
                    List.Open();
        }

        public void SetListItem(long index)
        {
            ((RegisterDropdownListClass)List).SetClass((int)index);
        }
    }
}