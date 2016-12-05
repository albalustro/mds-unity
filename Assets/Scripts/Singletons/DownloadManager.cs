using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class DownloadManager : Singleton<DownloadManager>{

    public string remoteUrlBase = "https://s3-sa-east-1.amazonaws.com/jogosxmile/newmds/";
    public string localUrlBase = "file://D:\\XMILE\\Projetos\\MdS Unity\\Misterio dos Sonhos\\Build\\WebGL\\AssetBundles\\";

    public bool useLocal = false;
    public bool cleanCache = true;

    private WWW _www;

    public void DownloadScene(string sceneName)
    {
        StartCoroutine(Download(sceneName));
    }

    private IEnumerator Download(string sceneName)
    {
        if(sceneName != "Inicio")
        {
            
            while(!Caching.ready)
                yield return null;

            if(cleanCache)
                Caching.CleanCache();

            string urlBase = useLocal ? localUrlBase : remoteUrlBase;

            string url = urlBase + sceneName;

            Log("Baixando " + url);
            //_www = WWW.LoadFromCacheOrDownload(url, 1);
            //yield return _www;
            UnityWebRequest request = UnityWebRequest.GetAssetBundle(url);
            yield return request.Send();


            Log("Terminou de baixar");
            //if (_www.error!=null)
            //{
            //    LogError(_www.error);
            //    yield return null;
            //}
            if (request.isError)
            {
                LogError(request.error);
            }
            else
            {
                AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);
            }
        }
        
        SceneManager.LoadScene(sceneName);

        Log("Após carregar a cena");

        //_www.assetBundle.Unload(false);
        //_www.Dispose();
        //_www = null;

        

        Log("Tudo liberado..");
    }

}
