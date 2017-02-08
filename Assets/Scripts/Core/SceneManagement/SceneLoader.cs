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
    /// <summary>
    /// Esse singleton é responsavel por carregar as cenas pertinentes, de acordo com
    /// o metodo chamado, os parametros passados e com o contexto (cena a partir de onde
    /// o metodo do SceneLoader foi chamado)
    /// Todos os metodos publicos são contextualizados, (LoadLogin, LoadRoom, LoadChallenge, etc)
    /// </summary>
    public class SceneLoader : Singleton<SceneLoader>
    {
        [FullInspector.InspectorComment("Atribua a configuração pertinente: produção / desenvolvimento")]
        ScriptableObjects.ConnectionConfig _connectionConfig;

        private AssetBundle _assetbundle;
        private bool _backToMap;
        private int _backToEpisodeIndex;

        private FadeTransition _fadeTransitionInstance;

#if UNITY_EDITOR
        public bool byPassAssetbundles = true;
        public bool destroyDebugObjectsOnSceneLoad = false;
#endif

        #region Unity Methods

        protected override void Awake()
        {
            base.Awake();

            if(SceneLoader.Instance != this)
                Destroy(gameObject);
            else
                DontDestroyOnLoad(gameObject);

            _fadeTransitionInstance = FadeTransition.Instance;

        }

        #endregion

        #region PUBLIC LOAD SCENES METHODS

        /// <summary>
        /// Esse é o metodo publico chamado após a cena de splash
        /// (pode tb ser usado para voltar à cena de login ao clicar no computador das cenas de quarto)
        /// </summary>
        public void LoadLogin()
        {
            // Estamos assumindo que, em qualquer build, a cena de login sera a segunda, sempre.
            StartCoroutine(LoadSceneByIndex(1));
        }

        /// <summary>
        /// Metodo publico usado para carregar e abrir uma cena de desafio
        /// Ao finalizar a cena, o SceneLoader se encarregará de voltar para a cena correta (mapa ou episodio)
        ///     caso use o metodo GoBackAfterChallenge();
        /// </summary>
        /// <param name="challengeIndex">Indice do desafio a ser carregado. Valores válidos entre 1 e 5</param>
        public void LoadChallenge(int challengeIndex, int episodeIndex)
        {
            Scene curScene = SceneManager.GetActiveScene();
            _backToMap = curScene.IsMap();
            _backToEpisodeIndex = episodeIndex;

            // iniciando a variavel que sera usada para verificar o conceito 
            // adquirido no desafio que esta sendo aberto nesse momento
            Challenge.ChallengeConcept = ConceptTypes.CONCEPT_GREEN;

            int game = curScene.GetGameIndex();
            int world = curScene.GetWorldIndex();

            string challengeSceneName = string.Format("G{0}W{1}E{2}C{3}", 
                                game, world, episodeIndex, challengeIndex);

#if UNITY_WEBGL
#if UNITY_EDITOR
            if(byPassAssetbundles)
                LoadSceneWebGLSim(challengeSceneName);
            else
#endif
                LoadSceneWebGL(challengeSceneName);
#else
                LoadScene(challengeSceneName);
#endif
        }

        /// <summary>
        /// Metodo que carrega uma cena de episodio a partir de seu indice (entre 1 e 8)
        /// ** SOMENTE FUNCIONA A PARTIR DO MAPA **
        /// </summary>
        /// <param name="episodeIndex">Indice do episodio que deve ser carregado</param>
        public void LoadEpisodeScene(int episodeIndex)
        {
            Scene scene = SceneManager.GetActiveScene();

            //if(scene.IsMap() == false)
            //{
            //    LogError("Tentativa de carregar uma cena de episodio sem que estivesse no mapa.");
            //    return;
            //}

            int game = scene.GetGameIndex();
            int world = scene.GetWorldIndex();

            string challengeSceneName = string.Format("G{0}W{1}E{2}",
                                                game, world, episodeIndex);

#if UNITY_WEBGL
#if UNITY_EDITOR
            if(byPassAssetbundles)
                LoadSceneWebGLSim(challengeSceneName);
            else
#endif
                LoadSceneWebGL(challengeSceneName);
#else
                LoadScene(challengeSceneName);
#endif

        }

        /// <summary>
        /// Metodo que carrega a cena de escolha de avatar (quarto)
        /// </summary>
        public void LoadRoomScene()
        {
            Scene scene = SceneManager.GetActiveScene();
            int gameIndex = scene.GetGameIndex();
            LoadScene("G" + gameIndex + "Room");
        }

        /// <summary>
        /// Metodo para carregar o mapa relacionado com a cena atual.
        /// ** SO FUNCIONA CASO A CENA ATUAL SEJA UM EPISODE OU UM CHALLENGE **
        /// </summary>
        public void LoadMapScene()
        {
            Scene scene = SceneManager.GetActiveScene();
            int gameIndex = scene.GetGameIndex();
            int worldIndex = scene.GetWorldIndex();
            LoadScene("G" + gameIndex + "W" + worldIndex + "EpisodeMap");
        }

        /// <summary>
        /// Metodo que cuidará de carregar MAP ou EPISODE após *concluir* um desafio.
        /// Neste método o Conceito obtido no challenge já é atualizao no user profile
        /// </summary>
        public void GoBackAfterChallenge()
        {
            Scene curScene = SceneManager.GetActiveScene();
            if(curScene.IsChallenge())
                UserProfile.Instance.UpdateConcept(curScene, 
                            Challenge.ChallengeConcept, DateTime.Now);
            else
                LogError("GoBackAfterChallenge sendo invocado a partir de uma cena que não é um desafio");

            if(_backToMap)
                LoadMapScene();
            else
                LoadEpisodeScene(_backToEpisodeIndex);
        }

        #endregion

        #region Quit Game (nao deveria estar em outro lugar??)

        public void Quit()
        {
            StartCoroutine(InternalQuit());
        }

        private IEnumerator InternalQuit()
        {
            yield return new WaitForSeconds(_fadeTransitionInstance.BeginFade(FadeDirection.Out));

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
        }

        #endregion

        #region PRIVATE (UNDER THE HOOD) LOAD SCENES METHODS

        /// <summary>
        /// Esse método interno é usado para carregar qualquer cena (exceto a de login que é carregada pelo buildIndex)
        /// </summary>
        /// <param name="sceneNameToLoad">Nome da cena a ser carregada</param>
        void LoadScene(string sceneNameToLoad)
        {
            StartCoroutine(Loading(sceneNameToLoad));
        }

        /// <summary>
        /// Esse metodo carrega a cena pelo indice no build settings
        /// Antes da cena ser efetivamente carregada, aguarda o fade terminar
        /// </summary>
        IEnumerator LoadSceneByIndex(int index)
        {
            yield return new WaitForSeconds(_fadeTransitionInstance.BeginFade(FadeDirection.Out));
            SceneManager.LoadScene(index);
        }

        /// <summary>
        /// Esse é o metodo que, efetivamente efetua o fade out e carrega a proxima cena
        /// </summary>
        /// <param name="name">Nome da cena a ser carregada</param>
        IEnumerator Loading(string name)
        {
            yield return new WaitForSeconds(_fadeTransitionInstance.BeginFade(FadeDirection.Out));
            SceneManager.LoadScene(name);
        }


        #region Load WEBGL

        /// <summary>
        /// Esse metodo so devera ser chamado para carregar EPISODIO E CHALLENGE
        /// Ambos estao dentro do mesmo assetbundle cujo nome é o mesmo nome do
        ///    episodio porem com todas as letras minuculas
        /// </summary>
        void LoadSceneWebGL(string sceneName)
        {
            Log("Caching.compressionEnabled = " + Caching.compressionEnabled.ToString());
            Log("Caching.enabled = " + Caching.enabled.ToString());
            Log("Caching.expirationDelay = " + Caching.expirationDelay.ToString());
            Log("Caching.maximumAvailableDiskSpace = " + Caching.maximumAvailableDiskSpace.ToString());
            Log("Caching.spaceAvailable = " + Caching.spaceFree.ToString());
            Log("Caching.spaceOccupied = " + Caching.spaceOccupied.ToString());


            string assetBundleName = sceneName.Substring(0, 6).ToLower();

            if(_assetbundle != null && _assetbundle.GetAllScenePaths().Any(path =>
                                                    path.Contains(sceneName)))
            {
                Log("Carregando " + sceneName + " do assetbundle em memoria");
                LoadScene(sceneName);
            }
            else
            {
                Log("Cena " + sceneName + " não esta carregada no assetbundle, iniciando download");
                StartCoroutine(Download(assetBundleName,
                    (error) =>
                    {
                        if(error == false)
                        {
                            LoadScene(sceneName);
                        }
                        else
                        {
                            // todo: tratar erro
                            LogError(string.Format("Nao foi possivel baixar o assetbundle {0}. ", assetBundleName));
                        }
                    }));

            }
        }
       
        #endregion

        #region Load LOCAL

        private void LoadEpisodeLocal(int index)
        {

        }

        #endregion

        #region Load WEBGL simulation (in editor)
#if UNITY_EDITOR
        private void LoadSceneWebGLSim(string sceneName)
        {
            string assetBundleName = sceneName.Substring(0,6).ToLower();
            LoadSceneInPlayMode(sceneName, assetBundleName);
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
        #endregion

        #endregion

        #region ASSETBUNDLE

        private IEnumerator Download(string assetBundleName, Action<bool> callback)
        {

            //while(!Caching.ready)
            //    yield return null;

            string urlBase = _connectionConfig.assetbundlesURL;

            string plataform = "WebGL\\";

#if UNITY_EDITOR
            plataform = "StandaloneWindows\\";
#endif

            string url = urlBase + plataform + assetBundleName;

            Log("Baixando assetbundle em: " + url);

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

        #endregion


        
    }
}
