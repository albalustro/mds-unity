using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MDS.DialogueSystem
{
    public class AudioClipHolder : MDSBehaviour
    {
        private static AudioClipHolder _instance;

        [SerializeField]
        private Dictionary<string, AudioClip> _audioLib = new Dictionary<string, AudioClip>();

        private void Start()
        {
            if(_audioLib == null || _audioLib.Count == 0)
                LogError("Audio lib sem nenhum audio");
            _instance = this;            
        }

        public void Add(string audioName, AudioClip clip)
        {
            if (_audioLib.ContainsKey(audioName))
            {
                if(_audioLib[audioName] != clip)
                {
                    _audioLib[audioName] = clip;
                    LogWarning(string.Format("Audio {0} já existe e o clip será substituido", audioName));
                }
            }   
            else
            {
                _audioLib.Add(audioName, clip);
            }
        }

        public static AudioClip Get(string audioName)
        {
            if(_instance._audioLib == null || _instance._audioLib.ContainsKey(audioName) == false)
            {
                _instance.LogError("Audio não existente: " + audioName);
                return null;
            }

            return _instance._audioLib[audioName];

        }

    }
}
