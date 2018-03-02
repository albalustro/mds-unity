using System;
using System.Collections.Generic;
using UnityEngine;

namespace Playmove
{
    public class RegisterNames : PYButton
    {
        public NamesManagerPopup.PlayerInfo RegisterInfo;
        [SerializeField]
        private PYText NameRegistred;

        protected override void ClickAction()
        {
            base.ClickAction();

            if (string.IsNullOrEmpty(NameRegistred.Text))
            {
                NamesManagerPopup.RegisterNames(KeyboardEntry, RegisterInfo);
            }
            else
            {
                NamesManagerPopup.Instance.RemoveFromFilter(NameRegistred.Text);
                NameRegistred.Text = string.Empty;
                RegisterInfo.Name = string.Empty;
                RegisterInfo.ClassId = 0;
            }
        }

        private void KeyboardEntry(NamesManagerPopup.PlayerInfo[] names)
        {
            if (names != null && names[0] != null)
            {
                RegisterInfo.Name = names[0].Name;
                RegisterInfo.ClassId = names[0].ClassId;
            }

            NameRegistred.Text = RegisterInfo.Name;
            //PlaytableWin32.Instance.Data.ScoreManager.RegisterStudent(RegisterInfo.Name, RegisterInfo.ClassId, 0);
            //PYNamesManager.SaveName(RegisterInfo.Name, RegisterInfo.ClassId);
            Debug.Log("ERRROR");
        }
    }
}