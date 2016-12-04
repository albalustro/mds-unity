//
// Copyright (c) 2013-2016 Ancient Light Studios
// All Rights Reserved
// 
// http://www.ancientlightstudios.com
//

namespace AncientLightStudios.uTomate
{
    using System.Collections;
    using System.IO;
    using API;
    using UnityEditor;
    using UnityEngine;

    [UTActionInfo(actionCategory = "Import & Export", sinceUTomateVersion = "1.7.0")]
    [UTDoc(title = "Upload to FTP Server", description = "Uploads a file to an FTP server.")]
    [UTDefaultAction]
    [UTInspectorGroups(groups = new[] { "Server", "File" })]
    public class UTUploadToFtpServerAction : UTAction
    {

        // ------------- SERVER ------------
        [UTDoc(description = "Name of the server to connect to.")]
        [UTInspectorHint(@group = "Server", order = 10, required = true)]
        public UTString serverName;

        [UTDoc(description = "Username for connecting.")]
        [UTInspectorHint(@group = "Server", order = 20)]
        public UTString username;

        [UTDoc(description = "Password for connecting.")]
        [UTInspectorHint(@group = "Server", order = 30, displayAs = UTInspectorHint.DisplayAs.Password)]
        public UTString password;

        // ------------- FILE ------------
        [UTDoc(description = "File to upload.")]
        [UTInspectorHint(@group = "File", order = 10, displayAs = UTInspectorHint.DisplayAs.OpenFileSelect, required = true)]
        public UTString file;

        [UTDoc(description = "The remote directory where the file should be put. The file will have the same name as the source file.")]
        [UTInspectorHint(@group = "File", order = 20, required = true)]
        public UTString remoteDirectory;


        public override IEnumerator Execute(UTContext context)
        {

            var theFile = file.EvaluateIn(context);
            if (string.IsNullOrEmpty(theFile))
            {
                throw new UTFailBuildException("You need to specify a file to upload.", this);
            }
            if (!UTFileUtils.IsFile(theFile))
            {
                throw new UTFailBuildException("The given file " + theFile + " does not exist or is not a file.", this);
            }

            var theServerName = serverName.EvaluateIn(context);
            if (string.IsNullOrEmpty(theServerName))
            {
                throw new UTFailBuildException("You need to specify a server name.", this);
            }

            var remotePath = remoteDirectory.EvaluateIn(context);
            if (string.IsNullOrEmpty(remotePath))
            {
                throw new UTFailBuildException("You need to specify a remote path.", this);
            }

            using (var ftpClient = new UTFtpClient(theServerName, username.EvaluateIn(context), password.EvaluateIn(context)))
            {
                if (UTPreferences.DebugMode)
                {
                    Debug.Log("Creating remote folder: " + remotePath);
                }

                var result = ftpClient.MkDirs(remotePath);
                while (!result.Finished)
                {
                    yield return null;
                }
                Verify(result);

                if (UTPreferences.DebugMode)
                {
                    Debug.Log("Uploading file");
                }

                result = ftpClient.Upload(theFile, UTFileUtils.CombineToPath(remotePath, Path.GetFileName(theFile)));
                while (!result.Finished)
                {
                    try
                    {

                        UTils.ShowAsyncProgressBar("Uploading " + result.Status, result.Progress);
                        if (context.CancelRequested)
                        {
                            result.Cancel();
                        }
                        yield return null;
                    }
                    finally
                    {
                        UTils.ClearAsyncProgressBar();
                    }

                }

                if (!context.CancelRequested)
                {
                    Verify(result);
                    Debug.Log("Successfully uploaded file " + theFile + " to FTP server.");
                }
                else
                {
                    Debug.Log("Upload cancelled.");
                }
            }

        }

        private void Verify(UTFtpClient.UTDeferredExecution result)
        {
            if (!result.Successful)
            {
                throw new UTFailBuildException(result.ErrorMessage, this);
            }
        }


        [MenuItem("Assets/Create/uTomate/Import + Export/Upload to FTP Server", false, 273)]
        public static void AddAction()
        {
            Create<UTUploadToFtpServerAction>();
        }
    }
}
