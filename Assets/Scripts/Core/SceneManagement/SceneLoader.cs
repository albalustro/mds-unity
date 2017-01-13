using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MDS.Utilities;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace MDS.Core.SceneManagement
{
    public class SceneLoader : Singleton<SceneLoader>
    {
        public string remoteUrlBase = "https://s3-sa-east-1.amazonaws.com/jogosxmile/newmds/";
        public string localUrlBase = "file://C:\\XMILE\\Projetos\\MdS Unity\\Misterio dos Sonhos\\Build\\AssetBundles\\";

        public bool useLocal = true;
        public bool byPassAssetbundles = true;

        [SerializeField] private GameObject _loadingObj;

        private AssetBundle _assetbundle;
        private string _goAfterChallengeSceneName;

        protected override void Awake()
        {
            base.Awake();
            if(SceneLoader.Instance != this)
                Destroy(gameObject);
            else
            {
                DontDestroyOnLoad(gameObject);
                gameObject.name = "__ SCENE LOADER __";
            }
        }

        public void LoadChallenge(int index)
        {
            Scene curScene = SceneManager.GetActiveScene();

            _goAfterChallengeSceneName = curScene.name;

            if(curScene.IsMap())
            {

            }
            else if(curScene.IsEpisode())
            {
                
                // iniciando a variavel que sera usada para verificar o conceito 
                // adquirido no desafio que esta sendo aberto nesse momento
                Challenge.ChallengeConcept = ConceptTypes.CONCEPT_GREEN;

#if UNITY_WEBGL
#if UNITY_EDITOR
                if(byPassAssetbundles)
                    LoadChallengeWebGLSim(index, curScene);
                else
#endif
                    LoadChallengeWebGL(index, curScene);
#else
                LoadChallengeLocal(index, curScene);
#endif
            }
        }

        public void GoBackAfterChallenge()
        {
            UserProfile.Instance.UpdateConcept(SceneManager.GetActiveScene(), Challenge.ChallengeConcept, DateTime.Now);
            SceneManager.LoadScene(_goAfterChallengeSceneName);
        }

        private void LoadChallengeWebGL(int index, Scene curScene)
        {

            string challengeSceneName = curScene.name + "C" + index.ToString();
            string assetBundleName = curScene.name.ToLower();

            if(_assetbundle != null && _assetbundle.GetAllScenePaths().Any(path =>
                                                    path.Contains(challengeSceneName)))
            {
                SceneManager.LoadScene(challengeSceneName);
            }
            else
            {
                _loadingObj.SetActive(true);
                _loadingObj.transform.position = Camera.main.transform.position;
                StartCoroutine(Download(assetBundleName,
                    (error) =>
                    {
                        if(error == false)
                        {
                            SceneManager.LoadScene(challengeSceneName);
                            _loadingObj.SetActive(false);
                        }
                        else
                        {
                            // todo: tratar erro
                        }
                    }));

            }
        }

        private void LoadChallengeLocal(int index, Scene curScene)
        {
            string challengeSceneName = curScene.name + "C" + index.ToString();
            _loadingObj.SetActive(true);
            _loadingObj.transform.position = Camera.main.transform.position;
            _goAfterChallengeSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(challengeSceneName);
            _loadingObj.SetActive(false);

        }

#if UNITY_EDITOR

        private void LoadChallengeWebGLSim(int index, Scene curScene)
        {

            string challengeSceneName = curScene.name + "C" + index.ToString();
            string assetBundleName = curScene.name.ToLower();

            LoadSceneInPlayMode(challengeSceneName, assetBundleName);
        }

        /// <summary>
        /// Metodo usado para carregar uma cena que nao esta no build settings.
        /// Criado exclusivamente para o modo de simulacao
        /// </summary>
        private void LoadSceneInPlayMode(string sceneName, string bundleName)
        {
            string[] assetPaths = UnityEditor.AssetDatabase
                .GetAssetPathsFromAssetBundleAndAssetName(bundleName, sceneName);

            if(assetPaths.Length == 0)
            {
                LogError("Cena nao encontrada [ " + sceneName + " ] no bundle " + bundleName);
                return;
            }

            UnityEditor.EditorApplication.LoadLevelInPlayMode(assetPaths[0]);

        }
#endif

        private IEnumerator Download(string assetBundleName, Action<bool> callback)
        {

            while(!Caching.ready)
                yield return null;

            string urlBase = useLocal ? localUrlBase : remoteUrlBase;

            string plataform = "WebGL\\";

#if UNITY_EDITOR
            plataform = "StandaloneWindows\\";
#endif

            string url = urlBase + plataform + assetBundleName;

            Log("Baixando " + url);

            using(UnityWebRequest request = UnityWebRequest.GetAssetBundle(url))
            {

                yield return request.Send();

                Log("Terminou de baixar");

                if(request.isError)
                {
                    LogError(request.error);
                    if(callback != null)
                        callback(true);
                }
                else
                {
                    _assetbundle = DownloadHandlerAssetBundle.GetContent(request);
                    if(callback != null)
                        callback(false);
                }

            }


        }

    }
}
