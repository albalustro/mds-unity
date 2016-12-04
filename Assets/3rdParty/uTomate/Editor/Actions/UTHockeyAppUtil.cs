namespace AncientLightStudios.uTomate
{
    using System.Collections.Generic;
    using API;
    using UnityEngine;

    public abstract class UTHockeyAppUtil
    {

        /// <summary>
        /// Creates headers for HockeyApp, optionally based on a WWWForm
        /// </summary>
        /// <param name="apiToken">the api token to apply</param>
        /// <param name="wwwForm">the form which should be used.</param>
        /// <returns></returns>
        public static Dictionary<string, string> CreateHeaders(string apiToken, WWWForm wwwForm = null)
        {
            var result = wwwForm != null ? wwwForm.headers : new Dictionary<string, string>();
            result.Add("X-HockeyAppToken", apiToken);
            FixHockeyAppHeaders(result);
            return result;
        }

        /// workaround around HockeyApp middleware issue see
        /// http://support.hockeyapp.net/discussions/problems/37308-unable-to-upload-an-apk-file-using-the-api
        private static void FixHockeyAppHeaders(Dictionary<string, string> headers)
        {
            if (headers.ContainsKey("Content-Type"))
            {
                headers["Content-Type"] = headers["Content-Type"].Replace("\"", "");
            }
        }

        /// <summary>
        /// Checks the given hockeyapp response (inside the WebUtil). If response indicates failure throws an exception.
        /// </summary>
        /// <param name="util">the webutil holding the actual call respons</param>
        /// <param name="hostAction">the action that initiated the call. used to mark exceptions.</param>
        /// <param name="result">the parsed result</param>
        /// <returns>false if the operation has been cancelled, true otherwise.</returns>
        public static bool ProcessHockeyAppResponse(UTWebUtil util, UTAction hostAction, out Dictionary<string,object> result )
        {
            if (util.Cancelled)
            {
                result = null;
                return false;
            }

            if (util.Failed)
            {
                throw new UTFailBuildException("Operation failed. Reason: '" + util.ErrorText + "'.", hostAction);
            }

            result = util.ResponseAsJson;
            if (result == null)
            {
                throw new UTFailBuildException("Unable to parse response from HockeyApp.", hostAction);
            }

            if (result.ContainsKey("errors"))
            {
                throw new UTFailBuildException("HockeyApp reported an error." + (UTPreferences.DebugMode ? "" : " Please enable debug mode for details."), hostAction);
            }

            if (result.ContainsKey("status") && "error" == (result["status"] as string))
            {
                throw new UTFailBuildException("HockeyApp reported an error. Message: " + result["message"], hostAction);
            }
            return true;
        }
    }
}
