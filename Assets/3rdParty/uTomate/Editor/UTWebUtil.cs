namespace AncientLightStudios.uTomate
{
    using System.Collections;
    using System.Collections.Generic;
    using API;
    using ThirdParty.MiniJson;
    using UnityEngine;

    /// <summary>
    /// Wrapper around Unity's WWW class which removes a lot of boilerplate in the actions.
    /// </summary>
    public class UTWebUtil
    {
        private readonly string url;
        private readonly WWWForm wwwForm;
        private readonly Dictionary<string, string> headers;
        private readonly UTContext context;
        private readonly string progressText;
        private IEnumerator driver;

        /// <summary>
        /// Changes the request method in the headers.
        /// </summary>
        /// <param name="headers">the headers</param>
        /// <param name="method">the new request method to use (e.g PUT or DELETE)</param>
        public static void SetRequestMethod(Dictionary<string, string> headers, string method)
        {
            headers["X-HTTP-Method-Override"] = method;
        }

        /// <summary>
        /// Runs the given form in the background displaying a progress bar. 
        /// </summary>
        /// <param name="url">the URL to post the form against</param>
        /// <param name="wwwForm">the form to be posted</param>
        /// <param name="headers">the headers to use</param>
        /// <param name="context">the context (used to detect cancellation)</param>
        /// <param name="progressText">the text to display in the progress bar</param>
        /// <returns></returns>
        public static UTWebUtil ExecuteInContext(string url, WWWForm wwwForm, Dictionary<string, string> headers, UTContext context, string progressText)
        {
            return new UTWebUtil(url, wwwForm, headers, context, progressText);
        }

        /// <summary>
        /// Runs a GET to the given URL in the background, displaying a progress bar.
        /// </summary>
        /// <param name="url">the URL to GET from</param>
        /// <param name="headers">the headers to use</param>
        /// <param name="context">the context (used to detect cancellation)</param>
        /// <param name="progressText">the text to display in the progress bar</param>
        /// <returns></returns>
        public static UTWebUtil ExecuteInContext(string url, Dictionary<string, string> headers, UTContext context, string progressText)
        {
            return ExecuteInContext(url, null, headers, context, progressText);
        }

        private UTWebUtil(string url, WWWForm wwwForm, Dictionary<string, string> headers, UTContext context, string progressText)
        {
            this.url = url;
            this.wwwForm = wwwForm;
            this.headers = headers;
            this.context = context;
            this.progressText = progressText;
        }

        /// <summary>
        /// The response text. 
        /// </summary>
        public string ResponseText { get; private set; }
        
        /// <summary>
        /// The error text. Will be null if Failed returns false.
        /// </summary>
        public string ErrorText { get; private set; }

        /// <summary>
        /// Has the operation been cancelled by the user?
        /// </summary>
        public bool Cancelled
        {
            get { return context.CancelRequested; }
        }

        /// <summary>
        /// Did the operation fail (error text was set on the WWW object)
        /// </summary>
        public bool Failed
        {
            get { return !string.IsNullOrEmpty(ErrorText); }
        }

        /// <summary>
        /// Called inside a coroutine to detect whether the request is still running.
        /// </summary>
        /// <returns>true when the request is still running, false when it is finished</returns>
        public bool Proceed()
        {
            if (driver != null)
            {
                return driver.MoveNext();
            }

            driver = Run();
            return true;
        }

        /// <summary>
        /// The request's response as a JSON dictionary.
        /// </summary>
        public Dictionary<string, object> ResponseAsJson
        {
            get
            {
              return  Json.Deserialize(ResponseText) as Dictionary<string, object>;
            }
        }

        private IEnumerator Run()
        {
            if (UTPreferences.DebugMode)
            {
                Debug.Log("Sending request to " + url);
            }

            using (var www =  wwwForm != null ? new WWW(url, wwwForm.data, headers) : new WWW(url, null, headers))
            {
                try
                {
                    do
                    {
                       
                        UTils.ShowAsyncProgressBar(string.Format(progressText, www.uploadProgress), www.uploadProgress);
                        // upload in background
                        yield return "";
                    } while (!www.isDone && !context.CancelRequested);

                    if (context.CancelRequested)
                    {
                        if (UTPreferences.DebugMode)
                        {
                            Debug.Log("Cancelling web operation.");
                        }
                        yield break;
                    }
                }
                finally
                {
                    UTils.ClearAsyncProgressBar();
                }

                ResponseText = www.text;
                ErrorText = www.error;

                if (UTPreferences.DebugMode)
                {
                    Debug.Log("Server Response: " + ResponseText);
                }
            }
        }
    }
}
