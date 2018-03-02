using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Playmove
{
    public class SopServiceMethods
    {
        private const string PATH = SopService.Configuration.REQUEST_PATH; //PlaytableWin32.Instance.Authentication.WebServicePath;
        private const string VERSION = SopService.Configuration.REQUEST_VERSION;
        private string FULLPATH { get { return PATH + VERSION; } }

        public string ConstrucJson<T>(T data)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(data);
            //return JsonUtility.ToJson(data);
        }

        public T DeConstructJson<T>(string text)
        {
            //JSONData<T> data = Newtonsoft.Json.JsonConvert.DeserializeObject<JSONData<T>>(text);
            //return data.Dados;
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(text);
            //JSONData<T> jsonData = JsonUtility.FromJson<JSONData<T>>(text);
            //return jsonData.Dados;
        }

        #region Save

        public void Post(string method)
        {
            ConstructPost<string>(method);
        }

        /// <summary>
        /// Envia um Post para o SOP
        /// </summary>
        /// <param name="method">Url de envio ja formulada com os parametros desejados</param>
        public void Post<T>(string method, Action<SopRequest<T>> callback)
        {
            ConstructPost(method, callback);
        }

        public void Post<T>(string method, string version, Action<SopRequest<T>> callback)
        {
            ConstructPost(method, version, callback);
        }

        /// <summary>
        /// Envia um Post para o SOP serializando o dado para JSON com o tipo informado
        /// </summary>
        /// <typeparam name="T">Tipo para enviar o dado</typeparam>
        /// <param name="method">Url para envio</param>
        /// <param name="data">Dado para envio</param>
        /// <param name="callback">Callback quando retornado do SOP</param>
        public void Post<T>(string method, T data, Action<SopRequest<T>> callback)
        {
            string postData = ConstrucJson<T>(data);
            ConstructPost(method, postData, callback);
        }

        public void Post<T>(string method, long id, Action<SopRequest<T>> callback)
        {
            string postData = id.ToString();
            ConstructPost(method, postData, callback);
        }

        #endregion Save

        #region Load

        public void Get<T>(string method, Action<T> callback)
        {
            string url = FULLPATH + method;
            PlaytableWin32.Instance.StartCoroutine(ApiGet(url, callback));
        }

        public void Get<T>(string method, string version, Action<T> callback)
        {
            string url = PATH + version + method;
            PlaytableWin32.Instance.StartCoroutine(ApiGet(url, callback));
        }

        #endregion Load

        #region Get webrequest

        private IEnumerator ApiGet<T>(string url, Action<T> callback = null)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Content-Type", "application/json; charset=UTF-8");
            headers.Add("Cookie", "Our session cookie");
            if (!string.IsNullOrEmpty(PlaytableWin32.Instance.ValidationKey))
                headers.Add("access_token", PlaytableWin32.Instance.ValidationKey);

            //Debug.LogWarning("Get " + url);

            WWW www = new WWW(url, null, headers);
            yield return www;
            if (callback != null)
            {
                object response = www.text;
                if (!string.IsNullOrEmpty(www.error))
                {
                    Debug.LogWarning(url + " ERROR: " + www.error);
                    callback(default(T));
                }
                else
                {
                    try
                    {
                        if (www.responseHeaders.ContainsKey("CONTENT-TYPE") && www.responseHeaders["CONTENT-TYPE"].Contains("application/json"))
                            response = DeConstructJson<T>(www.text);
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning("Erro na leitura dos dados " + url + " ERRO: " + e.Message); // UNDONE remover try e configurar v1
                        response = www.text;
                    }

                    T obj = (T)Convert.ChangeType(response, typeof(T));
                    callback(obj);
                }
            }
        }

        #endregion Get webrequest

        #region Post webrequest

        private void ConstructPost<T>(string method, Action<SopRequest<T>> callback = null)
        {
            string url = string.Concat(FULLPATH + method);
            PlaytableWin32.Instance.StartCoroutine(ApiPost(url, callback));
        }

        private void ConstructPost<T>(string method, string postData, Action<SopRequest<T>> callback = null)
        {
            string url = string.Concat(FULLPATH + method);
            PlaytableWin32.Instance.StartCoroutine(ApiPost(url, postData, callback));
        }

        private void ConstructPost<T>(string methods, string postData, string version, Action<SopRequest<T>> callback = null)
        {
            string url = string.Concat(PATH + version + "Settings/" + methods);
            PlaytableWin32.Instance.StartCoroutine(ApiPost(url, postData, callback));
        }

        private IEnumerator ApiPost<T>(string url, Action<SopRequest<T>> callback = null)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Content-Type", "application/json; charset=UTF-8");
            headers.Add("Cookie", "Our session cookie");
            if (!string.IsNullOrEmpty(PlaytableWin32.Instance.ValidationKey))
                headers.Add("access_token", PlaytableWin32.Instance.ValidationKey);

            Debug.LogWarning("Get " + url);

            WWW www = new WWW(url, null, headers);
            yield return www;
            PostRequest<T>(www, callback);
        }

        private IEnumerator ApiPost<T>(string url, string postData, Action<SopRequest<T>> callback = null)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Content-Type", "application/json; charset=UTF-8");
            headers.Add("Cookie", "Our session cookie");
            if (!string.IsNullOrEmpty(PlaytableWin32.Instance.ValidationKey))
                headers.Add("access_token", PlaytableWin32.Instance.ValidationKey);

            Debug.LogWarning("Post " + url);

            byte[] pData = Encoding.UTF8.GetBytes(postData.ToCharArray());

            WWW www = new WWW(url, pData, headers);
            yield return www;
            PostRequest<T>(www, callback);
        }

        private static void PostRequest<T>(WWW www, Action<SopRequest<T>> callback)
        {
            if (callback != null)
            {
                SopRequest<T> request = new SopRequest<T>();
                try
                {
                    request = Newtonsoft.Json.JsonConvert.DeserializeObject<SopRequest<T>>(www.text);
                }
                catch (Exception) { }
                request.Success = string.IsNullOrEmpty(www.error);
                if (request.Success)
                {
                    try
                    {
                        request.Model = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(www.text);
                    }
                    catch (Exception e)
                    {
                        request.Message = www.text;
                    }
                }
                else
                {
                    if (request.ModelState.Count > 0)
                    {
                        request.Message = string.Empty;
                        foreach (var key in request.ModelState.Keys)
                            foreach (var value in request.ModelState[key])
                                request.Message += value;
                    }
                }
                callback(request);
            }
        }

        #endregion Post webrequest
    }
}

public class ModelStateIvalid
{
    public string Message;
    public Dictionary<string, string[]> ModelState;
}