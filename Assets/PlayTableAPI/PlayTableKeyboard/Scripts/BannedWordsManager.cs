using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Playmove
{
    [Serializable]
    public class BannedWordsData
    {
        public double Version;
        public List<string> Words;

        public BannedWordsData()
        {
            Version = 0;
            Words = new List<string>();
        }

        public BannedWordsData(double version, List<string> words)
        {
            Version = version;
            Words = new List<string>(words);
        }
    }

    public class BannedWordsManager
    {
        public static BannedWordsData _bannedWords = new BannedWordsData();

        public static BannedWordsData BannedWords
        {
            get
            {
                return _bannedWords;
            }
            set
            {
                _bannedWords = value;
            }
        }

        public static void LoadBannedWords()
        {
            TextAsset gameBannedWords = Resources.Load<TextAsset>(string.Format("Localization/{0}/BannedWords", PlaytableWin32.Instance.Language));

            List<string> textContent = new List<string>(gameBannedWords.text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None));
            double version;
            if (double.TryParse(textContent[0], out version))
            {
                textContent.RemoveAt(0);
                BannedWords = new BannedWordsData(version, textContent);
            }
            else
            {
                BannedWords = new BannedWordsData(0, new List<string>());
                Debug.Log("Words empty...");
            }

            PlaytableWin32.Instance.Data.LoadBannedWords(SynchronizeBannedWords);
        }

        private static void SynchronizeBannedWords(string data)
        {
            BinaryFormatter b = new BinaryFormatter();

            if (!string.IsNullOrEmpty(data))
            {
                try
                {
                    MemoryStream m = new MemoryStream(Convert.FromBase64String(data));
                    BannedWordsData words = (BannedWordsData)b.Deserialize(m);

                    if (words.Version > BannedWords.Version)
                    {
                        //Debug.Log("Launcher possui ultima versao. Launcher Version: " + words.Version + ". Game Version: " + BannedWords.Version);
                        BannedWords = words;
                    }
                    else
                    {
                        //Debug.Log("Atualizando Launcher. Launcher Version: " + words.Version + ". Game Version: " + BannedWords.Version);
                        SaveBannedWords(BannedWords);
                    }
                }
                catch (Exception)
                {
                    Debug.LogWarning("Palavras com erro, reincerindo");
                    SaveBannedWords(BannedWords);
                }
            }
            else
            {
                Debug.LogWarning("Inserindo lista de palavras banidas ao launcher");
                SaveBannedWords(BannedWords);
            }
        }

        private static void SaveBannedWords(BannedWordsData words)
        {
            MemoryStream m = new MemoryStream();
            BinaryFormatter b = new BinaryFormatter();
            b.Serialize(m, words);
            PlaytableWin32.Instance.Data.SaveBannedWords(Convert.ToBase64String(m.GetBuffer()));
        }
    }
}