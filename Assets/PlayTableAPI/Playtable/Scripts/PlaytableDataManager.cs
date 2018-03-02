using System;
using UnityEngine;

namespace Playmove
{
    [RequireComponent(typeof(PlaytableWin32))]
    public class PlaytableDataManager : MonoBehaviour
    {
        private PlaytableWin32 _playtable { get { return GetComponent<PlaytableWin32>(); } }
        private SopServiceMethods _service = new SopServiceMethods();

        public PYScoreData ScoreManager { get; private set; }
        public PYClassManager ClassManager { get; private set; }
        public PYGradeManager GradesManager { get; private set; }
        public PYStudentManager StudentsManager { get; private set; }
        public PYFileManager FilesManager { get; private set; }
        public PYGameServiceManager GamesManager { get; private set; }
        public PYGroupFilesManager GroupFilesManager { get; private set; }
        public PYHistoricManager HistoricManager { get; private set; }
        public PYFileApplicationManager FileApplicationManager { get; private set; }
        public PYPlaytableConfigurationManager ConfigurationManager { get; private set; }
        public PYPlaytableInfoManager PlaytableInfoManager { get; private set; }

        public void Initialize()
        {
            ScoreManager = new PYScoreData();
            StudentsManager = new PYStudentManager();
            ClassManager = new PYClassManager();
            GradesManager = new PYGradeManager();
            FilesManager = new PYFileManager();
            GamesManager = new PYGameServiceManager();
            GroupFilesManager = new PYGroupFilesManager();
            HistoricManager = new PYHistoricManager();
            FileApplicationManager = new PYFileApplicationManager();
            ConfigurationManager = new PYPlaytableConfigurationManager();
            PlaytableInfoManager = new PYPlaytableInfoManager();

            StudentsManager.LoadStudents();
            ScoreManager.Load();
        }

        #region Volume

        public void GetMute(Action<string> callback)
        {
            var url = "Settings/IsInMute";
            _service.Get(url, "v1/", callback);
        }

        public void DoMute()
        {
            var url = "Settings/Mute";
            _service.Get<string>(url, "v1/", VolumeCallback);
        }

        public void DoUnMute()
        {
            var url = "Settings/UnMute";
            _service.Get<string>(url, "v1/", VolumeCallback);
        }

        public void GetVolume(Action<int> callback)
        {
            var url = "Settings/GetVolume";
            _service.Get(url, "v1/", callback);
        }

        public void SetVolume(int volume)
        {
            var url = string.Format("Settings/SetVolume?volume={0}", volume.ToString());
            _service.Get<string>(url, "v1/", VolumeCallback);
        }

        private void VolumeCallback(string result) // TODO: utilizar retorno
        {
            Debug.Log("VolumeCallback: " + result);
        }

        #endregion Volume

        #region BannedWords

        public void LoadBannedWords(Action<string> callback)
        {
            var url = string.Concat("Settings/GetRecordes?gameId=", "BannedWords", "&gameDifficult=", 0);
            _service.Get(url, "v1/", callback);
        }

        public void SaveBannedWords(string words)
        {
            var url = string.Concat("Settings/SetRecordes?gameId=", "BannedWords", "&gameDifficult=", 0, "&gameScores=", words);
            _service.Post(url);
        }

        #endregion BannedWords
    }
}