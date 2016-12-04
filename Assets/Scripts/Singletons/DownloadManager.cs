using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class DownloadManager : Singleton<DownloadManager>{

    public string remoteUrlBase = "https://s3-sa-east-1.amazonaws.com/jogosxmile/newmds/";
    public string localUrlBase = "file://D:\\XMILE\\Projetos\\MdS Unity\\Misterio dos Sonhos\\Build\\WebGL\\AssetBundles\\";

    public bool useLocal = true;
    public bool cleanCache = true;

    

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
            WWW www = WWW.LoadFromCacheOrDownload(url, 1);
            yield return www;

            Log("Terminou de baixar");
            if (www.error!=null)
            {
                LogError(www.error);
                yield return null;
            }
        }
       
        SceneManager.LoadScene(sceneName);

    }

}
