using UnityEngine;

namespace Playmove
{
    public class TagManager
    {
        #region Py

        public enum Scenes
        {
            Initialize,
            MainMenu,
            Info,
            Score,
            GameMode,
            GamePlay,
            PYBundleExample
        }

        public enum PYButtonEvents
        {
            OnClick,
            OnDown,
            OnUp,
            OnEnter,
            OnExit
        }

        public enum Axis
        {
            None,
            X,
            Y,
            Z
        }

        public enum Type
        {
            From,
            To
        }

        public enum LoopType
        {
            None,
            Loop,
            PingPong
        }

        public enum GameDifficulty
        {
            Easy,
            Normal,
            Hard,
            None,
        }

        public enum GameState
        {
            Starting,
            Running,
            Paused,
            Completed
        }

        public enum CountDirection
        {
            Crescent,
            Decrescent
        }

        public enum GamePrefs
        {
            GameToken,
            GameDifficulty,
            SomethingToSave
        }

        public enum SortingLayerNames
        {
            Background,
            Game,
            Default,
            Foreground,
            GUI
        }

        public const string POPUP_RESOURCES_PATH = "Popups/";
        public const string POPUP_BUTTON_RESOURCES_PATH = "Popups/Buttons/";

        public const float BLACK_FADER_IN_GAME = 31;
        public const float BLACK_FADER_IN_CAMERA = 1;

        /// <summary>
        /// Camera child
        /// </summary>
        public const float BLACK_FADER_Z_POSITION = 10;

        /// <summary>
        /// Global
        /// </summary>
        public const float PYPOPUPS_Z_POSITION = -40;

        #endregion Py

        public const string LOCALIZATION_BACK = "Geral_Voltar";
        public const string LOCALIZATION_RESTRICTED_ACCESS = "Placar_MsgAcessoRestrito";
        public const string LOCALIZATION_CONFIRM_DELETE_ALL = "Placar_MsgApagarPlacar";
        public const string LOCALIZATION_SCORE_DELETED = "Placar_MsgPlacarApagado";
        public const string LOCALIZATION_ATENTION = "Geral_Atencao";
        public const string LOCALIZATION_CONTINUE = "Geral_Continuar";

        public static string DIRECTORY_TO_SAVE_PENDRIVE = "{0}" + Application.companyName + "/";

        public enum EMessageType
        {
            Success,
            Normal,
            Warning,
            Error
        }

        public enum EGameName
        {
            Memoria,
            CentralDeAtividades,
            Pintura,
            QuebraCabecas
        }

        public enum EFilterOrder
        {
            NameAsc,
            NameDesc,
            ClassAsc,
            ClassDesc,
            MaisNovos,
            MaisVelhos
        }
    }
}