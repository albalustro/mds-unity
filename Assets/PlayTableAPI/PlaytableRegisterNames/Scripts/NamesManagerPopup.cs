using System;
using System.Collections.Generic;
using UnityEngine;

namespace Playmove
{
    public class NamesManagerPopup : PYOpenable
    {
        /// <summary>
        /// Essa classe se tornou singleton apenas para controle de instancia em tela,
        /// no caso irá buscar para ver se já foi instanciada na cena, caso contrário
        /// irá automaticamente ser criada
        /// </summary>

        #region Singleton

        private static NamesManagerPopup _instance;

        public static NamesManagerPopup Instance
        {
            get
            {
                if (!_instance) _instance = FindObjectOfType<NamesManagerPopup>();
                return _instance;
            }
        }

        #endregion Singleton

        [Serializable]
        public class PlayerInfo
        {
            public PlayerInfo()
            {
            }

            public PlayerInfo(string name, Sprite image, KeyboardPositions position = 0)
            {
                PlayerAlias = name;
                PlayerImage = image;
                Position = position;
            }

            public PlayerInfo(string name, Sprite image, int position)
            {
                PlayerAlias = name;
                PlayerImage = image;
                Position = (KeyboardPositions)position;
            }

            /// <summary>
            ///  Nome que vai aparecer na tela de registro
            /// Ex.: Jogador 1, Jogador Azul, Curupira, Flor Vermelha...
            /// </summary>
            public string PlayerAlias;

            /// <summary>
            /// Imagem que simboliza o jogador sendo registrado
            /// Ex.: Imagem do Curupira, de uma Flot Vermelha, do icone do jogo...
            /// </summary>
            public Sprite PlayerImage;

            /// <summary>
            /// Posição em que o jogador está representado na playtable
            /// </summary>
            public KeyboardPositions Position;

            [Header("Deixe vazio se estiver no Inspector")]
            /// <summary>
            /// Nome registrado
            /// </summary>
            public string Name;

            /// <summary>
            /// Turma Selecionada
            /// </summary>
            public long ClassId;

            public override string ToString()
            {
                return string.Format("Nome: {0} Turma: {1}", Name, ClassId);
            }
        }

        /// A parte inteira do enum é utilizada como index para adicionar os registros.
        public enum KeyboardPositions
        {
            DownLeft,
            DownRight,
            UpperRight,
            UpperLeft,
            DownCenter,     // Usado apenas para registrar um único usuário
        }

        [Header("NamesManagerPopup")]
        [SerializeField]
        private float _transitionTime = 0.75f;

        [SerializeField]
        private float _keyboardPosition;

        [SerializeField]
        private GameObject Background;

        [Header("Names Window Refs")]
        [SerializeField]
        private RegisterDropdownManager NamesWindow;

        [Header("Class Window Refs")]
        [SerializeField]
        private RegisterDropdownManager ClassWindow;

        #region props

        private RegisterDropdownListNames NamesList { get { return (RegisterDropdownListNames)NamesWindow.List; } }
        private RegisterDropdownListClass ClassList { get { return (RegisterDropdownListClass)ClassWindow.List; } }

        private string _typedName;

        public string TypedName
        {
            get { return _typedName; }
            set { _typedName = NamesWindow.LabelText = value; }
        }

        private NamesPlayerInfo _playerInformations;

        private NamesPlayerInfo PlayerInformations
        {
            get
            {
                if (!_playerInformations)
                    _playerInformations = GetComponentInChildren<NamesPlayerInfo>();
                return _playerInformations;
            }
        }

        private string ClassSelected { get { return ClassWindow.LabelText; } }

        private PYTweenAnimation _backgroundEnter;

        private PYTweenAnimation BackgroundEnter
        {
            get
            {
                if (!_backgroundEnter)
                    _backgroundEnter = PYTweenAnimation.AddNew(Background.gameObject, 999)
                        .SetDuration(0.3f)
                        .SetEaseType(Ease.Type.InExpo)
                        .SetAlpha(0f, 0.5f);
                return _backgroundEnter;
            }
        }

        private Camera CameraNames { get { return GetComponentInChildren<Camera>(); } }

        #endregion props

        private bool _multipleRegistries { get { return _actualNameIndexToAdd != _namesToAdd.Length - 1/* > 1*/; } }

        private int _actualNameIndexToAdd;
        private static PlayerInfo[] _namesToAdd;
        private KeyboardPositions _actualPosition;
        private Vector3 _soundButtonOriginalPositon;
        private Quaternion _cameraOriginalRotation;
        private bool _noClassConfirmation = false;
        private bool _isRegisteringSingleUser = false;

        #region Create Popup / Clear Filter - Static Stuff

        public static bool IsCreated { get { return Instance != null; } }
        public static NamesManagerPopup MyReference;
        public static Action<PlayerInfo[]> ReturnNamesCallback;
        public static PlayerInfo[] NamesRegistred;

        private List<string> _filterNames = new List<string>();

        public static void RegisterName(Action<PlayerInfo> callback)
        {
            RegisterNames(true, (users) =>
            {
                if (callback != null && users != null && users.Length > 0)
                    callback(users[0]);
            }, null);
        }

        public static void RegisterNames(Action<PlayerInfo[]> callback = null, params PlayerInfo[] registerNames)
        {
            RegisterNames(false, callback, registerNames);
        }

        private static void RegisterNames(bool registerSingleUser, Action<PlayerInfo[]> callback, params PlayerInfo[] registerNames)
        {
            if (registerSingleUser || (registerNames == null && registerNames.Length == 0)) // Se params vazio, iniciar estado Default
            {
                registerNames = new PlayerInfo[1];
                string defaultPlayerName = PYBundleManager.Localization.GetAsset<string>("NamesManagerPlayer", "Jogador");
                registerNames[0] = new PlayerInfo(defaultPlayerName, null, registerSingleUser ? KeyboardPositions.DownCenter : KeyboardPositions.DownLeft);
            }
            _namesToAdd = registerNames;

            NamesRegistred = new PlayerInfo[registerNames.Length];

            ReturnNamesCallback = callback;

            if (IsCreated)
            {
                Instance.Open();
                return;
            }

            NamesManagerPopup resourceObj = Resources.Load<NamesManagerPopup>("NamesPopup");
            MyReference = Instantiate(resourceObj);

            Camera.main.cullingMask = ~(1 << LayerMask.NameToLayer("NamesRegister"));
            MyReference._isRegisteringSingleUser = registerSingleUser;

            Instance.Open();
        }

        #endregion Create Popup / Clear Filter - Static Stuff

        #region Unity

        private void OnEnable()
        {
            _instance = this;
        }

        private void OnDestroy()
        {
            _instance = null;
        }

        #endregion Unity

        #region PYOpenable

        public override void Open()
        {
            CameraNames.cullingMask = (1 << LayerMask.NameToLayer("NamesRegister"));
            PlayTableKeyboard.Instance.SetCameraRef(CameraNames);
            ChangeLayer(PlayTableKeyboard.Instance.transform, gameObject.layer);

            base.Open();
            _cameraOriginalRotation = CameraNames.transform.rotation = Camera.main.transform.rotation;
            if (SoundControlButton.Instance != null)
                _soundButtonOriginalPositon = SoundControlButton.Instance.transform.position;
            _actualNameIndexToAdd = 0;

            if (PYButtonGroupManager.Instance != null)
                PYButtonGroupManager.Instance.DisableAll(1);

            PlayTableKeyboard.Instance.DisableCapsLock();
            PlayTableKeyboard.Instance.onConfirm.AddListener(AddName);
            PlayTableKeyboard.Instance.onCancel.AddListener(Close);
            PlayTableKeyboard.Instance.OnTextChange.AddListener(OnKeyboardTextChange);

            if (NamesList.ListEmpty)
                PlayAudio(PYAudioTags.B_EscrevaONomeQueDeseja);
            else
                PlayAudio(PYAudioTags.B_SelecioneNaListaOu);

            ClassWindow.List.onItemClicked.AddListener(ClassSelectedOnList);

            NamesWindow.List.Initialize();
            ClassWindow.List.Initialize();
            _actualPosition = _namesToAdd[_actualNameIndexToAdd].Position;

            BackgroundEnter.Play(BaseOpen);
        }

        private void BaseOpen()
        {
            // Reposição do teclado e chamada para entrada DEVE sempre vir antes da reposição de itens, uma vez que o teclado utiliza de dados da camera para se posicinar.
            RepositionKeyboard();
            PlayTableKeyboard.Instance.Open(_keyboardPosition);

            SetCameraRotation();
            RepositionPlayerInfo();

            // Quando estamos registrando apenas um único user não precisamos mostrar o PlayerInformations
            if (_isRegisteringSingleUser)
                PlayerInformations.gameObject.SetActive(false);
            else
                PlayerInformations.Open(_namesToAdd[_actualNameIndexToAdd].PlayerAlias, _namesToAdd[_actualNameIndexToAdd].PlayerImage);

            NamesWindow.OpenHorizontal();
            for (int i = 0; i < _filterNames.Count; i++) // Adicionar ao filtro de nomes os nomes que já foram registrados na sequencia
            {
                if (_filterNames[i] != null)
                    NamesWindow.List.AddFilter(_filterNames[i]);
            }

            ClassWindow.OpenHorizontal();

            //if (SoundControlButton.Instance != null)
            //    SoundControlButton.Instance.TurnDiscrete();

            Opened();
        }

        public override void Close()
        {
            if (_actualNameIndexToAdd < _namesToAdd.Length) // No caso de clicar em cancelar do teclado e ainda estiver nome para registrar
            {
                AddName(string.Empty);
                return;
            }

            PlayTableKeyboard.Instance.Close();
            PlayTableKeyboard.Instance.onConfirm.RemoveAllListeners();
            PlayTableKeyboard.Instance.onCancel.RemoveAllListeners();
            //PlayTableKeyboard.Instance.OnTextChange -= OnKeyboardTextChange;
            PlayTableKeyboard.Instance.OnTextChange.AddListener(OnKeyboardTextChange);

            PlayerInformations.Close();
            NamesWindow.CloseHorizontal();
            ClassWindow.CloseHorizontal();

            Invoke("BaseClose", .75f);
        }

        private void BaseClose()
        {
            base.Close();
            CameraNames.transform.rotation = _cameraOriginalRotation;
            transform.position = Vector3.zero;

            if (SoundControlButton.Instance != null)
                SoundControlButton.Instance.transform.position = _soundButtonOriginalPositon;
            if (PYButtonGroupManager.Instance != null)
                PYButtonGroupManager.Instance.DisableAll(1);

            TypedName = "";

            NamesWindow.List.ClearFilter();

            PlayTableKeyboard.Instance.OnTextChange.RemoveAllListeners();
            PlayTableKeyboard.Instance.ClearText();

            BackgroundEnter.Reverse(Closed);
        }

        protected override void Closed()
        {
            base.Closed();
            if (ReturnNamesCallback != null)
                ReturnNamesCallback.Invoke(NamesRegistred);
            ReturnNamesCallback = null;

            //if (SoundControlButton.Instance != null)
            //    SoundControlButton.Instance.RemoveDiscrete();

            PlayTableKeyboard.Instance.SetCameraRef(Camera.main);
            ChangeLayer(PlayTableKeyboard.Instance.transform, LayerMask.NameToLayer("Default"));
        }

        #endregion PYOpenable

        public void ClearFilter()
        {
            _filterNames = new List<string>();
        }

        public void RemoveFromFilter(string name)
        {
            _filterNames.Remove(name);
            NamesList.RemoveFilter(name);
        }

        private void OnKeyboardTextChange(string text)
        {
            TypedName = text;
            //if (!string.IsNullOrEmpty(text))
            NamesList.UpdateListView(text);
            if (NamesList.ContaisName(TypedName))
            {
                ClassWindow.DisableList = true;
                long classId = NamesList.GetClassId();
                ClassWindow.SetListItem(classId);
                //ClassWindow.SetListItem(ClassList.GetSelectedClassIndex());
            }
            else
            {
                ClassWindow.DisableList = false;
                ClassWindow.ClearLabel();
            }
        }

        private void ClassSelectedOnList(RegisterDropdownItem item)
        {
            ClassWindow.LabelText = item.Text;
            ClassWindow.List.Close();
            _noClassConfirmation = true;
        }

        public void NameSelectedOnList(string name)
        {
            TypedName = name;
            NamesWindow.List.Close();
            long classId = NamesList.GetClassId();
            ClassWindow.SetListItem(classId);
            if (classId != 0)
                ClassWindow.DisableList = true;
        }

        private void AddName(string name)
        {
            long classId = ClassList.GetSelectedClassId();
            bool nameRegistred = true;
            if (!string.IsNullOrEmpty(name)) // Se o nome está vazio, quer dizer que pulou o registro deste nome
            {
                if (!NameAlreadyRegistred(name)) // Nome ainda não registrado
                {
                    if (classId == 0) // Se campo sem turma ou vazio
                    {
                        if (string.IsNullOrEmpty(ClassSelected) || !_noClassConfirmation) // se campo vazio ou sem confirmação de que é sem turma
                        {
                            ClassWindow.LabelText = ClassList.GetItemText(0);// GetClassList()[0];
                            ClassWindow.List.Open();
                            PlayAudio(PYAudioTags.B_SelecioneSuaTurma);
                            _noClassConfirmation = true;
                            return;
                        }
                    }
                }
                else
                {
                    PlayAudio(PYAudioTags.B_DesculpeMasEsteNomeJaEstaRegistrado);
                    return;
                }
            }
            else
            {
                PlayAudio(PYAudioTags.B_NaoRegistrado);
                nameRegistred = false;
            }

            _noClassConfirmation = false;

            int nameIndex = _actualNameIndexToAdd++;

            if (nameRegistred)
            {
                NamesRegistred[nameIndex] = new PlayerInfo()
                {
                    Name = name,
                    ClassId = classId,
                    Position = _namesToAdd[nameIndex].Position,
                    PlayerAlias = _namesToAdd[nameIndex].PlayerAlias,
                    PlayerImage = _namesToAdd[nameIndex].PlayerImage
                };

                PYStudentManager.Student n = new PYStudentManager.Student();
                n.Name = NamesRegistred[nameIndex].Name;
                n.ClassId = NamesRegistred[nameIndex].ClassId;
                PlayAudio(PYAudioTags.B_Registrado);

                NamesWindow.List.AddFilter(name); // Adicionar nome para não aparecer na janelas de nomes
                _filterNames.Add(name);
            }

            PlayTableKeyboard.Instance.ClearText();

            if (_actualNameIndexToAdd >= _namesToAdd.Length) // Finaliza se todos os nomes foram registrados
            {
                if (_multipleRegistries)
                    PlayAudio(PYAudioTags.B_TudoTerminado);
                Close();
            }
            else
                NextName();

            TypedName = "";
        }

        private static void PlayAudio(PYAudioTags tag, Action<PYAudioSource.PYAudioSourceEventData> callback = null, float delay = 0)
        {
            if (PYBundleManager.Instance != null)
                PYAudioManager.Instance.StartAudio(tag, PYGroupTag.Voice).Delay(delay).Play(callback);
            else
            {
                string tagString = tag.ToString().Replace("B_", "Voice_" + PlaytableWin32.Instance.Language.Replace("-", "") + "_");
                PYAudioManager.Instance.StartAudio((PYAudioTags)Enum.Parse(typeof(PYAudioTags), tagString)).Delay(delay).Play(callback);
            }
        }

        private bool NameAlreadyRegistred(string name)
        {
            foreach (string filter in _filterNames)
                if (filter == name)
                    return true;

            //foreach (PlayerInfo player in NamesRegistred)
            //    if (player != null && player.Name == name)
            //        return true;
            return false;
        }

        private void NextName()
        {
            PlayerInformations.Close();

            _actualPosition = _namesToAdd[_actualNameIndexToAdd].Position;
            KeyboardPositions previousPosition = _namesToAdd[_actualNameIndexToAdd - 1].Position;

            RepositionKeyboard();

            if (KeyboardChangedOrientation(previousPosition))
            {
                NamesWindow.CloseVertical();
                ClassWindow.CloseVertical();
                PlayTableKeyboard.Instance.Close();
            }
            else
            {
                PlayTableKeyboard.Instance.Open(_keyboardPosition);
            }

            ClassWindow.ClearLabel();

            Invoke("SetPositionsAndEnter", _transitionTime);
        }

        private bool KeyboardChangedOrientation(KeyboardPositions previousPosition)
        {
            // Se o teclado sair da posição de baixo para cima ou o contrário
            return ((previousPosition == KeyboardPositions.DownLeft || previousPosition == KeyboardPositions.DownRight) &&
                   (_actualPosition == KeyboardPositions.UpperLeft || _actualPosition == KeyboardPositions.UpperRight))
                   ||
                   ((_actualPosition == KeyboardPositions.DownLeft || _actualPosition == KeyboardPositions.DownRight) &&
                   (previousPosition == KeyboardPositions.UpperLeft || previousPosition == KeyboardPositions.UpperRight));
        }

        private void SetPositionsAndEnter()
        {
            SetCameraRotation();

            if (PlayTableKeyboard.Instance.CurrentState == KeyboardState.Closed)
                PlayTableKeyboard.Instance.Open(_keyboardPosition);
            else
            {
                if (!NamesWindow.List.IsOpen)
                    NamesWindow.List.Open();
            }

            RepositionPlayerInfo();

            PlayerInformations.Open(_namesToAdd[_actualNameIndexToAdd].PlayerAlias, _namesToAdd[_actualNameIndexToAdd].PlayerImage);

            if (NamesList.ListEmpty)
                PlayAudio(PYAudioTags.B_EscrevaONomeQueDeseja, null, 0.35f);
            else
                PlayAudio(PYAudioTags.B_SelecioneNaListaOu, null, 0.35f);

            NamesWindow.OpenVertical();
            ClassWindow.OpenVertical();
        }

        private void RepositionPlayerInfo()
        {
            switch (_actualPosition)
            {
                case KeyboardPositions.DownLeft:
                    PlayerInformations.SetQuadrant(new Vector3(Mathf.Abs(PlayerInformations.transform.position.x) * -1, PlayerInformations.transform.position.y, PlayerInformations.transform.position.z));
                    break;

                case KeyboardPositions.DownRight:
                    PlayerInformations.SetQuadrant(new Vector3(Mathf.Abs(PlayerInformations.transform.position.x), PlayerInformations.transform.position.y, PlayerInformations.transform.position.z));
                    break;

                case KeyboardPositions.UpperRight:
                    PlayerInformations.SetQuadrant(new Vector3(Mathf.Abs(PlayerInformations.transform.position.x) * -1, PlayerInformations.transform.position.y, PlayerInformations.transform.position.z));
                    break;

                case KeyboardPositions.UpperLeft:
                    PlayerInformations.SetQuadrant(new Vector3(Mathf.Abs(PlayerInformations.transform.position.x), PlayerInformations.transform.position.y, PlayerInformations.transform.position.z));
                    break;

                default:
                    break;
            }
        }

        private void SetCameraRotation()
        {
            switch (_actualPosition)
            {
                case KeyboardPositions.DownLeft:
                    break;

                case KeyboardPositions.DownRight:
                    CameraNames.transform.rotation = _cameraOriginalRotation;
                    break;

                case KeyboardPositions.UpperRight:
                    CameraNames.transform.rotation = Quaternion.Euler(_cameraOriginalRotation.eulerAngles.x, _cameraOriginalRotation.eulerAngles.y, _cameraOriginalRotation.eulerAngles.z + 180);
                    break;

                case KeyboardPositions.UpperLeft:
                    CameraNames.transform.rotation = Quaternion.Euler(_cameraOriginalRotation.eulerAngles.x, _cameraOriginalRotation.eulerAngles.y, _cameraOriginalRotation.eulerAngles.z + 180);
                    break;

                default:
                    break;
            }
        }

        private float RepositionKeyboard()
        {
            SetKeyboardButtonsTexts();
            switch (_actualPosition)
            {
                case KeyboardPositions.DownCenter:
                    _keyboardPosition = Mathf.Abs(_keyboardPosition) * 0.05f;
                    break;

                case KeyboardPositions.DownLeft:
                    _keyboardPosition = Mathf.Abs(_keyboardPosition);
                    break;

                case KeyboardPositions.DownRight:
                    _keyboardPosition = Mathf.Abs(_keyboardPosition) * -1;
                    break;

                case KeyboardPositions.UpperRight:
                    _keyboardPosition = Mathf.Abs(_keyboardPosition);
                    break;

                case KeyboardPositions.UpperLeft:
                    _keyboardPosition = Mathf.Abs(_keyboardPosition) * -1;
                    break;
            }
            return _keyboardPosition;
        }

        private void ChangeLayer(Transform item, int layer)
        {
            item.gameObject.layer = layer;
            foreach (Transform child in item)
            {
                child.gameObject.layer = layer;
                ChangeLayer(child, layer);
            }
        }

        private void SetKeyboardButtonsTexts()
        {
            PlayTableKeyboard.Instance.SetCancelButtonText(PYBundleManager.Localization.GetAsset<string>(PYBundleTags.Text_Skip, "PULAR"));
            if (_multipleRegistries)
            {
                PlayTableKeyboard.Instance.SetConfirmButtonText(PYBundleManager.Localization.GetAsset<string>(PYBundleTags.Text_Next, "PRÓXIMO"));
            }
            else
            {
                //PlayTableKeyboard.Instance.SetCancelButtonText(PYBundleManager.Localization.GetAsset<string>(PYBundleTags.Text_Geral_Cancelar, "CANCELAR"));
                PlayTableKeyboard.Instance.SetConfirmButtonText(PYBundleManager.Localization.GetAsset<string>(PYBundleTags.Text_Confirm, "CONFIRMAR"));
            }
        }
    }
}