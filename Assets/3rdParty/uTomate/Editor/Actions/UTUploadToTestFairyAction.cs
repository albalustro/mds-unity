//
// Copyright (c) 2013-2016 Ancient Light Studios
// All Rights Reserved
// 
// http://www.ancientlightstudios.com
//

namespace AncientLightStudios.uTomate
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using API;
    using ThirdParty.MiniJson;
    using UnityEditor;
    using UnityEngine;

    [UTActionInfo(actionCategory = "Import & Export", sinceUTomateVersion="1.6.0")]
    [UTDoc(title = "Upload to TestFairy", description = "Uploads a build to TestFairy.")]
    [UTInspectorGroups(groups = new[] {"General", "Files", "Release Settings", "Recording Settings", "Metrics"})]
    public class UTUploadToTestFairyAction : UTAction
    {
        private const string TestFairyUploadUrl = "https://app.testfairy.com/api/upload/";

        [UTDoc(title = "API Key", description = "The API key you want to use for uploading to TestFairy.")]
        [UTInspectorHint(displayAs = UTInspectorHint.DisplayAs.Password, required = true, group = "General", order = 1)]
        public UTString apiKey;

        [UTDoc(description = "Optional property name. If set, the action will put the parsed JSON as a dictionary into the context under " +
                             "the given property name. This is useful for further processing, e.g. inviting testers.")]
        [UTInspectorHint(order = 2, group = "General")]
        public UTString resultProperty;

        [UTDoc(description = "The file that should be uploaded. Must be an IPA, APK or ZIP file.")]
        [UTInspectorHint(displayAs = UTInspectorHint.DisplayAs.OpenFileSelect, caption = "Please select file to upload", group = "Files", required = true, order = 1)]
        public UTString fileToUpload;

        [UTDoc(description = "The file that contains your symbol mappings, if you are using an obfuscator.")]
        [UTInspectorHint(displayAs = UTInspectorHint.DisplayAs.OpenFileSelect, caption = "Please select the symbol file to upload", order = 2, group = "Files")]
        public UTString symbolMappingFile;


        [UTDoc(description = "Release notes for this upload. This text will be added to email notifications.")]
        [UTInspectorHint(displayAs = UTInspectorHint.DisplayAs.TextArea, group = "Release Settings", order = 10)]
        public UTString releaseNotes;

        [UTDoc(description = "A list of tester groups that you want to notify about the new build. Enter 'all' to notify all testers.")]
        [UTInspectorHint(group = "Release Settings", order = 20)]
        public UTString[] notifyTesters;

        [UTDoc(description = "Should all previous installations be automatically updated to this one?")]
        [UTInspectorHint(group = "Release Settings", order = 25)]
        public UTBool autoUpdate;

        [UTDoc(description = "Add a small watermark to app icon?")]
        [UTInspectorHint(group = "Release Settings", order = 30)]
        public UTBool addWatermarkToIcon;

        [UTDoc(description = "Should android instrumentation be enabled?")]
        [UTInspectorHint(group = "Release Settings", order = 40)]
        public UTBool enableInstrumentation;

        [UTDoc(description = "Let the tester shake their device and fill in a bug report that openes up.")]
        [UTInspectorHint(group = "Release Settings", order = 40)]
        public UTBool shakeForBugReport;



        [UTDoc(description = "Maximum session recording length, eg 20m or 1h. Default is '10m'. Maximum 24h.")]
        [UTInspectorHint(group = "Recording Settings", order = 1)]
        public UTString sessionRecordingLength;
        
        [UTDoc(description = "Should video recording be enabled?")]
        [UTInspectorHint(group = "Recording Settings", order = 2)]
        public UTTestFairyVideoRecording videoRecording; 
        
        
        [UTDoc(description = "Quality of video recording.")]
        [UTInspectorHint(group = "Recording Settings", order = 3)]
        public UTTestFairyVideoQuality videoQuality; 
        
        [UTDoc(title="Video FPS", description = "Target video recording frames per second. Defaults to 1.0")]
        [UTInspectorHint(group = "Recording Settings", order = 4, displayAs = UTInspectorHint.DisplayAs.Slider, minValue = 1.0f, maxValue = 30.0f)]
        public UTFloat videoFps;

        [UTDoc(description = " When using this option, sessions are anonymous and account information is not collected from device.")]
        [UTInspectorHint(group = "Recording Settings", order = 5)]
        public UTBool anonymousRecording; 



        [UTDoc(title="CPU Statistics", description = "Collect user/kernel usage statistics.")]
        [UTInspectorHint(group = "Metrics", order=10)]
        public UTBool cpuStatistics;

        [UTDoc(description = "Collect process private/shared memory statistics.")]
        [UTInspectorHint(group = "Metrics", order=20)]
        public UTBool memoryStatistics;

        [UTDoc(description = "Collect process network utilization statistics.")]
        [UTInspectorHint(group = "Metrics", order=30)]
        public UTBool networkUsage;

        [UTDoc(description = "Collect phone signal strength statistics.")]
        [UTInspectorHint(group = "Metrics", order=40)]
        public UTBool signalStrength;

        [UTDoc(description = "Collect process logs from logcat (Adds android.permission.READ_LOGS permission).")]
        [UTInspectorHint(group = "Metrics", order=50)]
        public UTBool logs;

        [UTDoc(title="GPS", description = "Collect raw GPS location data, if used by app.")]
        [UTInspectorHint(group = "Metrics", order=60)]
        public UTBool gps;

        [UTDoc(description = "Collect battery status and drainage statistics (Adds android.permission.BATTERY_STATS permission).")]
        [UTInspectorHint(group = "Metrics", order=70)]
        public UTBool battery;

        [UTDoc(description = "Collect OpenGL statistics.")]
        [UTInspectorHint(group = "Metrics", order = 70)]
        public UTBool openGL;

        [UTDoc(description = "Keep microphone audio data, if used by app.")]
        [UTInspectorHint(group = "Metrics", order=80)]
        public UTBool microphone;

        [UTDoc(title = "Wi-Fi", description =  "Track WIFI signal strength and connectivity.")]
        [UTInspectorHint(group = "Metrics", order=90)]
        public UTBool wiFi;


        public void OnEnable()
        {
            UTils.MigrateActionIfRequired(this, 1.4f, delegate(UTUploadToTestFairyAction action)
            {
                action.enableInstrumentation.StaticValue = true;
                Debug.Log("The action '" + action.name + "' has been migrated. Please verify the settings of this action.", action);
            });
        }

        public override IEnumerator Execute(UTContext context)
        {
            var theApiKey = apiKey.EvaluateIn(context);
            if (string.IsNullOrEmpty(theApiKey))
            {
                throw new UTFailBuildException("Please specify an API key for uploading to TestFairy.", this);
            }

            var theFileToUpload = fileToUpload.EvaluateIn(context);
            if (!File.Exists(theFileToUpload))
            {
                throw new UTFailBuildException("The file '" + theFileToUpload + "' does not exist. Please check if you have set up the correct file to upload.", this);
            }

            var theSymbolMappingFile = symbolMappingFile.EvaluateIn(context);
            if (!string.IsNullOrEmpty(theSymbolMappingFile) && !File.Exists(theSymbolMappingFile))
            {
                throw new UTFailBuildException("The file '" + theSymbolMappingFile + "' does not exist. Please check if you have set up the correct symbol mapping file.", this);
            }

            var theResultProperty = resultProperty.EvaluateIn(context);
            if (!string.IsNullOrEmpty(theResultProperty) && !UTContext.IsValidPropertyName(theResultProperty))
            {
                throw new UTFailBuildException("The name '" + theResultProperty + "' is no valid property name.", this);
            }

            var wwwForm = new WWWForm();
            wwwForm.AddField("api_key", theApiKey);

            var fileContents = File.ReadAllBytes(theFileToUpload);
            yield return null; // give the editor a break
            wwwForm.AddBinaryData("file", fileContents, Path.GetFileName(theFileToUpload));
            yield return null; // give the editor another break

            if (!string.IsNullOrEmpty(theSymbolMappingFile))
            {
                var symbolFileContents = File.ReadAllBytes(theSymbolMappingFile);
                yield return null;
                wwwForm.AddBinaryData("symbols_file", symbolFileContents);
                yield return null;
            }

            // ReSharper disable once CoVariantArrayConversion
            var theTesterGroups = EvaluateAll(notifyTesters, context);
            if (theTesterGroups.Length > 0)
            {
                wwwForm.AddField("testers_groups", string.Join(",", theTesterGroups));
            }

            // build metrics.
            var metrics = new HashSet<string>();
            AddIfTrue(cpuStatistics, "cpu", metrics, context);
            AddIfTrue(memoryStatistics, "memory", metrics, context);
            AddIfTrue(networkUsage, "network", metrics, context);
            AddIfTrue(signalStrength, "phone-signal", metrics, context);
            AddIfTrue(logs, "logcat", metrics, context);
            AddIfTrue(gps, "gps", metrics, context);
            AddIfTrue(battery, "battery", metrics, context);
            AddIfTrue(openGL, "opengl", metrics, context);
            AddIfTrue(microphone, "mic", metrics, context);
            AddIfTrue(wiFi, "wifi", metrics, context);

            if (metrics.Count > 0)
            {
                wwwForm.AddField("metrics", string.Join(",", metrics.ToArray()));
            }

            var theSessionRecordingLength = sessionRecordingLength.EvaluateIn(context);
            if (!string.IsNullOrEmpty(theSessionRecordingLength))
            {
                wwwForm.AddField("max-duration", theSessionRecordingLength);
            }

            var theInstrumentation = enableInstrumentation.EvaluateIn(context);
            wwwForm.AddField("instrumentation", theInstrumentation ? "on" : "off");

            var theAutoUpdate = autoUpdate.EvaluateIn(context);
            wwwForm.AddField("auto-update", theAutoUpdate ? "on" : "off");

            var theVideoRecording = videoRecording.EvaluateIn(context);
            wwwForm.AddField("video", 
                    theVideoRecording == VideoRecording.Off ? "off" :  
                    theVideoRecording == VideoRecording.On ?  "on" : 
                 /* theVideoRecording == VideoRecording.WifiOnly ? */ "wifi");

            var theVideoQuality = videoQuality.EvaluateIn(context);
            // ReSharper disable once PossibleNullReferenceException
            wwwForm.AddField("video-quality", Enum.GetName(typeof (VideoQuality), theVideoQuality).ToLower());

            var theVideoFps = videoFps.EvaluateIn(context);
            wwwForm.AddField("video-rate", string.Format("{0:0.#}", theVideoFps));

            var theIconWatermark = addWatermarkToIcon.EvaluateIn(context);
            if (theIconWatermark)
            {
                wwwForm.AddField("icon-watermark", "on");
            }

            var theReleaseNotes = releaseNotes.EvaluateIn(context);
            if (!string.IsNullOrEmpty(theReleaseNotes))
            {
                wwwForm.AddField("comment", theReleaseNotes);
            }


            var options = new HashSet<string>();
            AddIfTrue(shakeForBugReport, "shake", options, context);
            AddIfTrue(anonymousRecording, "anonymous", options, context);

            if (options.Count > 0)
            {
                wwwForm.AddField("options", string.Join(",", options.ToArray()));
            }


            if (UTPreferences.DebugMode)
            {
                Debug.Log("Uploading file: " + theFileToUpload + " (" + fileContents.Length + " bytes) to TestFairy.");
            }

            using (var www = new WWW(TestFairyUploadUrl, wwwForm.data, wwwForm.headers))
            {
                try
                {
                    do
                    {
                        UTils.ShowAsyncProgressBar(string.Format("Uploading to TestFairy: {0:P2}", www.uploadProgress), www.uploadProgress);
                        // upload in background
                        yield return "";
                    } while (!www.isDone && !context.CancelRequested);

                    if (context.CancelRequested)
                    {
                        yield break;
                    }
                }
                finally
                {
                    UTils.ClearAsyncProgressBar();
                }


                if (UTPreferences.DebugMode)
                {
                    Debug.Log("Server Response: " + www.text);
                }

                if (!string.IsNullOrEmpty(www.error))
                {
                    throw new UTFailBuildException("Upload failed. Reason: '" + www.error + "'.", this);
                }

                var result = Json.Deserialize(www.text) as Dictionary<string, object>;
                if (result == null)
                {
                    throw new UTFailBuildException("Unable to parse response from TestFairy.", this);
                }
                var status = result["status"];
                if (!"ok".Equals(status))
                {
                    var code = result["code"];
                    throw new UTFailBuildException("TestFairy reported an error: '" + status + "' with code '" + code + "'.", this);
                }

                Debug.Log("Successfully uploaded '" + theFileToUpload + "' to TestFairy.", this);

                if (!string.IsNullOrEmpty(theResultProperty))
                {
                    if (UTPreferences.DebugMode)
                    {
                        Debug.Log("Setting result property '" + theResultProperty + "'.", this);
                    }
                    context[theResultProperty] = result;
                }
            }
        }

        private static void AddIfTrue(UTBool utBool, string toAdd, HashSet<string> hashSet, UTContext context)
        {
            if (utBool.EvaluateIn(context))
            {
                hashSet.Add(toAdd);
            }
        }


        [MenuItem("Assets/Create/uTomate/Import + Export/Upload to TestFairy", false, 270)]
        public static void AddAction()
        {
            var result = Create<UTUploadToTestFairyAction>();
            result.enableInstrumentation.StaticValue = true;
        }

        public enum VideoRecording
        {
            On,
            Off,
            WiFiOnly
        }

        public enum VideoQuality
        {
            High,
            Medium,
            Low
        }
    }
}
