using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class DownloadManager : Singleton<DownloadManager>{

    public string urlBase = "https://s3-sa-east-1.amazonaws.com/jogosxmile/newmds/";

    public void DownloadScene(string sceneName)
    {
        StartCoroutine(Download(sceneName));
    }

    private IEnumerator Download(string sceneName)
    {
        if(sceneName != "Inicio")
        {
            WWW www = new WWW(urlBase + sceneName);
            yield return www;
        }
       
        SceneManager.LoadScene(sceneName);

    }

}
