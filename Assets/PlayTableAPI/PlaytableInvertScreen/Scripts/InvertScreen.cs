using UnityEngine;
using System.Collections;

namespace Playmove
{
    public class InvertScreen : PYButton
    {
        #region Singleton
        private static InvertScreen _instance;
        public static InvertScreen Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<InvertScreen>();
                }
                return _instance;
            }
        }
        #endregion

        public static bool Inverted = false;
        public static int Rotation = 1;

        [SerializeField]
        private Vector3 StartScale = Vector3.one;
        [SerializeField]
        private GameObject InvertMaskPrefab;

        #region Properties
        private BoxCollider2D Collider { get { return GetComponent<BoxCollider2D>(); } }
        #endregion

        #region Animations
        private PYTweenAnimation _buttonClickAnimation;
        private PYTweenAnimation ButtonClickAnimation
        {
            get
            {
                if (!_buttonClickAnimation)
                {
                    _buttonClickAnimation = PYTweenAnimation.AddNew(gameObject, 12653123)
                        .SetSpeed(270)
                        .SetEaseType(Ease.Type.OutBack);
                    _buttonClickAnimation.Name = "ClickAnimation";
                }
                return _buttonClickAnimation;
            }
        }

        private PYTweenAnimation _discreteAnimation;
        private PYTweenAnimation DiscreteAnimation
        {
            get
            {
                if (!_discreteAnimation)
                {
                    _discreteAnimation = PYTweenAnimation.AddNew(gameObject, 13123)
                        .SetAlpha(1f, 0.3f)
                        .SetDuration(0.2f);
                    _discreteAnimation.Name = "DiscreteAnimation";
                }
                return _discreteAnimation;
            }
        }

        private PYTweenAnimation _hideAnimation;
        private PYTweenAnimation HideAnimation
        {
            get
            {
                if (!_hideAnimation)
                {
                    _hideAnimation = PYTweenAnimation.AddNew(gameObject, 456435)
                          .SetAlpha(1f, 0f)
                          .SetDuration(0.2f)
                          .SetEaseType(Ease.Type.OutSine);
                    _hideAnimation.Name = "HideAnimation";
                }
                return _hideAnimation;
            }
        }
        #endregion

        private bool _buttonHidden, _discrete;
        private Vector3 _pressedScale = new Vector3(0.8f, 0.8f, 1);
        private Camera[] _cameras;
        private InvertMask _mask;

        #region Unity

        private void Update()
        {
            if (SoundControlButton.Instance == null) return;

            if (SoundControlButton.Instance.IsOpen)
                DiscreteButton();
            else
                RemoveDiscreteButton();
        }

        private void OnLevelWasLoaded()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            }

            UpdateCameraRotation();
        }

        #endregion

        public void ShowButton()
        {
            if (!_buttonHidden) return;
            _buttonHidden = false;

            Collider.enabled = true;

            HideAnimation.Stop();
            HideAnimation.Reverse();
        }

        public void HideButton()
        {
            if (_buttonHidden) return;
            _buttonHidden = true;

            Collider.enabled = false;

            HideAnimation.Stop();
            HideAnimation.Play();
        }

        public void RemoveDiscreteButton()
        {
            if (_buttonHidden) return;
            if (!_discrete) return;
            _discrete = false;

            DiscreteAnimation.Stop();
            DiscreteAnimation.Reverse();
        }

        public void DiscreteButton()
        {
            if (_buttonHidden) return;
            if (_discrete) return;
            _discrete = true;

            DiscreteAnimation.Stop();
            DiscreteAnimation.Play();
        }

        public void Rotate(bool animate = true)
        {
            Rotation *= -1;
            Inverted = Rotation == -1;

            if (animate)
                AnimateRotation();
            else
                UpdateCameraRotation();
        }

        #region PYButton
        protected override void DownAction()
        {
            if (SoundControlButton.Instance != null && SoundControlButton.Instance.IsOpen) return;
            AnimateButton(true);
        }

        protected override void UpAction()
        {
            AnimateButton(false);
        }

        protected override void ClickAction()
        {
            if (SoundControlButton.Instance != null && SoundControlButton.Instance.IsOpen) return;
            GetComponent<AudioSource>().Play();
            Rotate();
        }
        #endregion

        private void AnimateButton(bool isDown)
        {
            ButtonClickAnimation.Stop();
            ButtonClickAnimation.SetScale(transform.localScale, (isDown ? _pressedScale : StartScale));
            ButtonClickAnimation.SetRotation(transform.eulerAngles, Vector3.forward * (isDown ? 180 : 0));
            ButtonClickAnimation.Play();
        }

        private void UpdateCameraRotation()
        {
            _cameras = FindObjectsOfType<Camera>();

            for (int i = 0; i < _cameras.Length; i++)
            {
                Vector3 newRot = _cameras[i].gameObject.transform.eulerAngles;
                newRot.z = (Rotation == -1) ? 180 : 0;
                _cameras[i].gameObject.transform.eulerAngles = newRot;
            }
        }

        private void AnimateRotation()
        {
            _mask = FindObjectOfType<InvertMask>();
            if (_mask == null)
            {
                GameObject obj = (GameObject)Instantiate(InvertMaskPrefab);
                if (Camera.main != null)
                    obj.transform.SetParent(Camera.main.transform);
                obj.transform.localPosition = Vector3.forward;
                _mask = obj.GetComponent<InvertMask>();
            }

            _mask.Animate(CallbackMaskClosed);

            FaderManager.FadeInCamera(1, 0);
        }

        private void CallbackMaskClosed()
        {
            UpdateCameraRotation();
            FaderManager.FadeOutCamera(1, 0);
        }
    }
}