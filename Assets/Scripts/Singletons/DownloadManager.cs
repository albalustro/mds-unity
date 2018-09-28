using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System;

public class DownloadManager : Singleton<DownloadManager>
{

    public string remoteUrlBase = "https://s3-sa-east-1.amazonaws.com/jogosxmile/newmds/";
    public string localUrlBase = "file://D:\\XMILE\\Projetos\\MdS Unity\\Misterio dos Sonhos\\Build\\WebGL\\AssetBundles\\";

    public bool useLocal = false;
    public bool cleanCache = true;

    private AssetBundle _bundle;

    public void DownloadScene(string sceneName)
    {
        StartCoroutine(InternalDownloadScene(sceneName));
    }


    private IEnumerator InternalDownloadScene(string sceneName)
    {
        if(sceneName != "Inicio")
        {
            yield return Download(sceneName);
        }

        if (_bundle!=null)
        {
            _bundle.LoadAllAssets();
            _bundle.Unload(false);
            _bundle = null;
        }

        SceneManager.LoadScene(sceneName);

    }

    private IEnumerator Download(string assetbundleName)
    {

        while(!Caching.ready)
            yield return null;

        if(cleanCache)
            Caching.ClearCache();

        string urlBase = useLocal ? localUrlBase : remoteUrlBase;

        string url = urlBase + assetbundleName;

        Log("Baixando " + url);

        UnityWebRequest request = UnityWebRequest.GetAssetBundle(url);
        yield return request.Send();


        Log("Terminou de baixar");

        if(request.isNetworkError)
        {
            LogError(request.error);
        }
        else
        {
            _bundle = DownloadHandlerAssetBundle.GetContent(request);
        }
    }

}


