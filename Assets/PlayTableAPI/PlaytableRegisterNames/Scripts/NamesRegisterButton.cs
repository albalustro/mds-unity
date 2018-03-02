using UnityEngine;

namespace Playmove
{
    public class NamesRegisterButton : PYButton
    {
        [SerializeField]
        private PYText _nameHolder;

        [SerializeField]
        private NamesManagerPopup.PlayerInfo[] _players;

        public NamesManagerPopup.PlayerInfo[] Players
        {
            get { return _players; }
            private set { _players = value; }
        }

        protected override void Start()
        {
            base.Start();
            if (NamesManagerPopup.Instance != null)
            {
                NamesManagerPopup.Instance.ClearFilter();
            }
        }

        protected override void ClickAction()
        {
            base.ClickAction();

            if (string.IsNullOrEmpty(_nameHolder.Text))
            {
                if (Players.Length <= 1)
                    NamesManagerPopup.RegisterNames(NamesRegistred, Players[0]);
                else
                    NamesManagerPopup.RegisterNames(NamesRegistred, Players);
            }
            else
            {
                for (int i = 0; i < Players.Length; i++)
                {
                    NamesManagerPopup.Instance.RemoveFromFilter(Players[i].Name);
                    Players[i].Name = "";
                    Players[i].ClassId = 0;
                }
                _nameHolder.Text = string.Empty;
            }
        }

        private void NamesRegistred(NamesManagerPopup.PlayerInfo[] names)
        {
            if (names != null && names.Length > 0)
            {
                for (int i = 0; i < names.Length; i++)
                {
                    if (names[i] != null)
                    {
                        Players[i].Name = names[i].Name;
                        Players[i].ClassId = names[i].ClassId;
                        PlaytableWin32.Instance.Data.ScoreManager.RegisterStudent(Players[i].Name, Players[i].ClassId, 0, TagManager.GameDifficulty.Easy);
                        //Debug.LogError("Nome nao registrado");
                    }
                    else
                    {
                        Players[i].Name = "Anônimo";
                        Players[i].ClassId = 0;
                    }
                    _nameHolder.Text += Players[i].ToString() + "\n";
                }
            }
        }
    }
}