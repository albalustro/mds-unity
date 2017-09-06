using UnityEngine;

namespace MDS.ScriptableObjects
{
    [CreateAssetMenu(fileName = "URLConnectionConfig", menuName = "Connection URL Configuration")]
    public class ConnectionConfig : ScriptableObject
    {
        [SerializeField]
        private string _loginURL;
        [SerializeField]
        private string _conceptURL;
        [SerializeField]
        private string _assetbundlesURL;


        public string loginURL { get { return _loginURL; } }
        public string conceptURL { get { return _conceptURL; } }
        public string assetbundlesURL
        {
            get { return string.Format(_assetbundlesURL, Application.version); }
        }

    }
}
