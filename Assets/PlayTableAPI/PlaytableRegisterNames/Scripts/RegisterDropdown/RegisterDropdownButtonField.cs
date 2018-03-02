using UnityEngine;
using System.Collections;
using System;

namespace Playmove
{
    public class RegisterDropdownButtonField : MonoBehaviour
    {
        [SerializeField]
        private AnimationCurve _animationCurve;

        [SerializeField]
        private string _text;
        public string Text
        {
            get { return _text; }
            set { _text = NameText.Text = value; }
        }

        private PYButton _button;
        public PYButton Button
        {
            get
            {
                if (!_button) _button = GetComponent<PYButton>();
                return _button;
            }
        }

        private PYTweenAnimation _feedbackAnimation;
        private PYTweenAnimation FeebackAnimation
        {
            get
            {
                if (!_feedbackAnimation)
                    _feedbackAnimation = PYTweenAnimation.AddNew(gameObject, 888)
                        .SetDuration(0.1f)
                        .SetEaseType(Ease.Type.InOutSine)
                        .SetScale(Vector3.one, Vector3.one * 0.95f);
                return _feedbackAnimation;
            }
        }
        private PYTweenAnimation _EnterAnimation;
        private PYTweenAnimation EnterAnimation
        {
            get
            {
                if (!_EnterAnimation)
                    _EnterAnimation = PYTweenAnimation.AddNew(gameObject, 999);
                return _EnterAnimation;
            }
        }

        private PYTextBox _nameText;
        private PYTextBox NameText
        {
            get
            {
                if (!_nameText) _nameText = GetComponentInChildren<PYTextBox>();
                return _nameText;
            }
        }

        #region Unity

        private void Awake()
        {
            Text = "";

            Button.onDown.AddListener(NameButtonDownAction);
            Button.onUp.AddListener(NameButtonUpAction);
        }

        #endregion

        public void PlayAnimation(Action callback)
        {
            gameObject.SetActive(true);

            EnterAnimation.Stop();
            EnterAnimation
                .SetScale(new Vector3(0, 1, 1), Vector3.one)
                .SetDuration(0.3f)
                .SetCurve(_animationCurve);
                //.SetEaseType(Ease.Type.OutElastic);
            EnterAnimation.Play(callback);
        }

        public void ReverseAnimation(Action callback)
        {
            EnterAnimation.Stop();
            EnterAnimation
                .SetScale(Vector3.one, new Vector3(0, 1, 1))
                .SetDuration(0.25f)
                .SetEaseType(Ease.Type.InBack);
            EnterAnimation.Play(callback);
        }

        private void NameButtonUpAction(PYButton btn)
        {
            FeebackAnimation.Stop();
            FeebackAnimation.Reverse();
        }

        private void NameButtonDownAction(PYButton btn)
        {
            FeebackAnimation.Stop();
            FeebackAnimation.Play();
        }
    }
}