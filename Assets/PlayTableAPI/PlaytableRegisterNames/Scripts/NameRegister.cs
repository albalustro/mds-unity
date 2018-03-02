using UnityEngine;
using System.Collections;
using System;

namespace Playmove
{
    public class NameRegister : RegisterDropdownItem
    {
        private PYStudentManager.Student _nameData;
        public PYStudentManager.Student NameData
        {
            get
            {
                return _nameData;
            }
            set
            {
                _nameData = value;
                if (_nameData != null)
                {
                    MyText.Text = name = _nameData.Name;
                    //string.Format("{0} - {1}/{2}/{3} as {4} - {5}",
                    //_nameData.Name,
                    //_nameData.LastUpdated.Day,
                    //_nameData.LastUpdated.Month,
                    //_nameData.LastUpdated.Year,
                    //_nameData.LastUpdated.Hour + ":" + _nameData.LastUpdated.Minute,
                    //_nameData.ClassId);
                }
                else
                    MyText.Text = name = "";
            }
        }

        public NameRegister MyNameRegister { get; private set; }
        [HideInInspector]
        public Vector3 StartPosition, FinalPosition;

        private PYTweenAnimation _animation;
        private PYTweenAnimation Animation
        {
            get
            {
                if (!_animation)
                    _animation = PYTweenAnimation.Add(gameObject)
                        .SetDuration(0.3f)
                        .SetAlpha(0f, 1f);
                return _animation;
            }
        }

        public string Name { get { return NameData != null ? NameData.Name : ""; } }

        #region Unity
        private void OnEnable()
        {
            MyButton.onClick.AddListener(ActionName);
        }

        private void OnDisable()
        {
            MyButton.onClick.RemoveAllListeners();
        }

        #endregion

        /// <summary>
        /// Cria a instancia dos nomes em cena, irá retornar a referencia do objeto criado
        /// </summary>
        /// <param name="position"></param>
        /// <param name="parent"></param>
        /// <param name="popupReference"></param>
        /// <returns></returns>
        public NameRegister CreateName(Vector3 position, Transform parent, int index)
        {
            ListIndex = index;
            StartPosition = parent.transform.position;
            FinalPosition = position;
            MyNameRegister = Instantiate(this, position, Quaternion.identity) as NameRegister;
            MyNameRegister.transform.SetParent(parent);

            return MyNameRegister;
        }

        /// <summary>
        /// Usado para calback de clique
        /// </summary>
        /// <param name="btn"></param>
        public void ActionName(PYButton btn)
        {
            PlayTableKeyboard.Instance.SetText(Name);
            NamesManagerPopup.Instance.NameSelectedOnList(Name);
        }

        public void EnterName(PYStudentManager.Student name)
        {
            gameObject.SetActive(true);
            NameData = name;
            Animation.Play();
        }

        public void RemoveName()
        {
            NameData = null;
            gameObject.SetActive(false);
        }

        public void Reposition(Vector3 vector3)
        {
            gameObject.transform.position = vector3;
        }
    }
}