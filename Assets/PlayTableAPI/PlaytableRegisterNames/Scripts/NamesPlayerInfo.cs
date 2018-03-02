using UnityEngine;
using System.Collections.Generic;
using System;

//TODO Herdar de PyOpenable e fazer rescrita de Open
namespace Playmove
{
    public class NamesPlayerInfo : PYOpenable
    {
        [SerializeField]
        private GameObject _infoLabel;
        [SerializeField]
        private PYTextBox _playerName;
        [SerializeField]
        private SpriteRenderer _playerImage;
        [Header("Animation")]
        [SerializeField]
        private float _animateDistance = 7;
        [SerializeField]
        private float _animationDuration = 0.3f;
        [SerializeField]
        private Ease.Type _animationEase = Ease.Type.InSine;

        private PYTweenAnimation _labelAnimation;
        private PYTweenAnimation LabelAnimation
        {
            get
            {
                if (!_labelAnimation)
                    _labelAnimation = PYTweenAnimation.AddNew(_infoLabel, 1)
                        .SetDuration(_animationDuration)
                        .SetEaseType(_animationEase);
                return _labelAnimation;
            }
        }
        private PYTweenAnimation _playerImageAnimation;
        private PYTweenAnimation PlayerImageAnimation
        {
            get
            {
                if (!_playerImageAnimation)
                    _playerImageAnimation = PYTweenAnimation.AddNew(_playerImage.gameObject, 1)
                        .SetDuration(_animationDuration)
                        .SetDelay(0.25f)
                        .SetEaseType(_animationEase);
                return _playerImageAnimation;
            }
        }

        private Vector3 _labelPosition, _playerImagePosition;

        #region Unity

        private void Awake()
        {
            _labelPosition = _infoLabel.transform.localPosition;
            _playerImagePosition = _playerImage.transform.localPosition;
            _playerImage.transform.localPosition = _playerImagePosition + (_infoLabel.transform.right * -1) * _animateDistance;
            _infoLabel.transform.localPosition = _labelPosition + (_infoLabel.transform.right * -1) * _animateDistance;
        }

        #endregion

        #region PYOpenable

        public override void Open()
        {
            base.Open();
        }
        public void Open(string name, Sprite image)
        {
            Open();
            ShowInfo(name, image);
        }

        public override void Close()
        {
            base.Close();
            HideInfo();
        }

        #endregion

        public void SetQuadrant(Vector3 position)
        {
            transform.position = position;

            if (transform.position.x < 0)
            {
                _playerImage.transform.localPosition = _playerImagePosition + (_infoLabel.transform.right * -1) * _animateDistance;
                _infoLabel.transform.localPosition = _labelPosition + (_infoLabel.transform.right * -1) * _animateDistance;
            }
            else
            {
                _playerImage.transform.localPosition = _playerImagePosition + (_infoLabel.transform.right) * _animateDistance;
                _infoLabel.transform.localPosition = _labelPosition + (_infoLabel.transform.right) * _animateDistance;
            }
        }

        private void ShowInfo(string name, Sprite image)
        {
            SetName(name);
            SetImage(image);

            LabelAnimation.Stop();
            PlayerImageAnimation.Stop();
            LabelAnimation
                .SetPosition(_infoLabel.transform.localPosition, _labelPosition, true)
                .Play();
            PlayerImageAnimation
                .SetPosition(_playerImage.transform.localPosition, _playerImagePosition, true)
                .Play();
        }

        private void HideInfo()
        {
            LabelAnimation.Stop();
            PlayerImageAnimation.Stop();
            if (transform.position.x < 0)
            {
                LabelAnimation.SetPosition(_infoLabel.transform.localPosition, _infoLabel.transform.localPosition + (_infoLabel.transform.right * -1) * _animateDistance, true).Play();
                PlayerImageAnimation.SetPosition(_playerImage.transform.localPosition, _playerImage.transform.localPosition + (_infoLabel.transform.right * -1) * _animateDistance, true).Play();
            }
            else
            {
                LabelAnimation.SetPosition(_infoLabel.transform.localPosition, _infoLabel.transform.localPosition + (_infoLabel.transform.right) * _animateDistance, true).Play();
                PlayerImageAnimation.SetPosition(_playerImage.transform.localPosition, _playerImage.transform.localPosition + (_infoLabel.transform.right) * _animateDistance, true).Play();
            }
        }

        private void SetName(string name)
        {
            _playerName.Text = name;
        }

        private void SetImage(Sprite image = null)
        {
            if (image)
                _playerImage.sprite = image;
            else
                _playerImage.enabled = false;
        }
    }
}