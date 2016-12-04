//
// Copyright (c) 2013-2016 Ancient Light Studios
// All Rights Reserved
// 
// http://www.ancientlightstudios.com
//

namespace AncientLightStudios.uTomate
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;
    using API;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;
    using UObject = UnityEngine.Object;


    /// <summary>
    /// UTility class (pun intended) for various stuff that doesn't fit in somehwere else.
    /// </summary> 
    public static class UTils
    {
        /// <summary>
        /// Creates an asset of the given type and writes it to a new file at the currently active selection point.
        /// </summary>
        /// <returns>
        /// The generated asset.
        /// </returns>
        public static T CreateAssetOfType<T>(string preferredName) where T : ScriptableObject
        {
            var name = String.IsNullOrEmpty(preferredName) ? typeof (T).Name : preferredName;

            var path = "Assets";
            foreach (var obj in Selection.GetFiltered(typeof (UObject), SelectionMode.Assets))
            {
                path = AssetDatabase.GetAssetPath(obj);
                if (File.Exists(path))
                {
                    path = Path.GetDirectoryName(path);
                }
                break;
            }

            path = AssetDatabase.GenerateUniqueAssetPath(path + "/" + name + ".asset");
            var item = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(item, path);
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = item;
            return item;
        }

        /// <summary>
        /// Adds an asset to the asset at the given path.
        /// </summary>
        /// <returns>
        /// The added asset.
        /// </returns>
        /// <param name='path'>
        /// Path.
        /// </param>
        /// <param name='hide'>
        /// Hide the asset?
        /// </param>
        /// <typeparam name='T'>
        /// The type of asset to be added.
        /// </typeparam>
        public static T AddAssetOfType<T>(string path, bool hide) where T : ScriptableObject
        {
            var item = ScriptableObject.CreateInstance<T>();
            if (hide)
            {
                item.hideFlags = HideFlags.HideInHierarchy;
            }
            AssetDatabase.AddObjectToAsset(item, path);
            EditorUtility.SetDirty(item);
            return item;
        }

        /// <summary>
        /// Clears the unused entries in the automation plan asset located at the given path. This is necessary
        /// because we support undo/redo with the visual editor and therefore don't delete automation plan entries
        /// when they are removed from the graphical view. The unused entries are cleared when the plan is saved.
        /// </summary>
        /// <param name='path'>
        /// Path.
        /// </param>
        /// <seealso cref="UTSaveInterceptor"/>
        public static void ClearUnusedEntriesIn(string path)
        {
            var asset = AssetDatabase.LoadMainAssetAtPath(path);
            if (asset is UTAutomationPlan)
            {
                var plan = (UTAutomationPlan) asset;
                UTGraph graph = null;
                var allAssets = AssetDatabase.LoadAllAssetsAtPath(path);
                var entries = new List<UTAutomationPlanEntry>();
                foreach (var anAsset in allAssets)
                {
                    if (anAsset is UTGraph)
                    {
                        graph = (UTGraph) anAsset;
                    }
                    if (anAsset is UTAutomationPlanEntry)
                    {
                        entries.Add((UTAutomationPlanEntry) anAsset);
                    }
                }

                var deps = EditorUtility.CollectDependencies(new UObject[] {plan, graph});
                foreach (var dep in deps)
                {
                    if (dep is UTAutomationPlanEntry)
                    {
                        entries.Remove((UTAutomationPlanEntry) dep);
                    }
                }

                if (UTPreferences.DebugMode && entries.Count > 0)
                {
                    Debug.Log("Clearing " + entries.Count + " leaked entries from " + plan.name + ". This message is harmless.");
                }
                foreach (var entry in entries)
                {
                    UObject.DestroyImmediate(entry, true);
                }

                UTStatistics.CleanUp();
            }
        }

        private static object filteredHierarchy;

        private static object CreateFilteredHierarchy()
        {
            if (filteredHierarchy == null)
            {
                filteredHierarchy = UTInternalCall.CreateInstance("UnityEditor.FilteredHierarchy", HierarchyType.Assets);
            }
            UTInternalCall.Invoke(filteredHierarchy, "ResultsChanged");
            return filteredHierarchy;
        }

        /// <summary>
        /// Finds all visible assets of the given type.
        /// </summary>
        public static List<T> AllVisibleAssetsOfType<T>() where T : ScriptableObject
        {
            var result = new List<T>();

            var filteredHierarchy = CreateFilteredHierarchy();

            var searchFilter = UTInternalCall.InvokeStatic("UnityEditor.SearchableEditorWindow", "CreateFilter", typeof (T).Name, UTInternalCall.EnumValue("UnityEditor.SearchableEditorWindow+SearchMode", "Type"));
            UTInternalCall.Set(filteredHierarchy, "searchFilter", searchFilter);

            var hierarchyProperty = UTInternalCall.InvokeStatic("UnityEditor.FilteredHierarchyProperty", "CreateHierarchyPropertyForFilter", filteredHierarchy);

            var emptyIntArray = new int[0];
            while ((bool) UTInternalCall.Invoke(hierarchyProperty, "Next", emptyIntArray))
            {
                var instanceId = (int) UTInternalCall.Get(hierarchyProperty, "instanceID");
                var path = AssetDatabase.GetAssetPath(instanceId);
                var t = AssetDatabase.LoadAssetAtPath(path, typeof (T)) as T;
                if (t != null)
                {
                    result.Add(t);
                }
            }

            return result;
        }

        /// <summary>
        /// Checks if this script is running inside Unity Pro.
        /// </summary>
        /// <value>
        /// <c>true</c> if this script runs inside Unity Pro otherwise, <c>false</c>.
        /// </value>
        public static bool IsUnityPro
        {
            get { return InternalEditorUtility.HasPro(); }
        }

        /// <summary>
        /// Checks if the build pipeline supports the given build target.
        /// </summary>
        public static bool IsBuildTargetSupported(BuildTarget target)
        {
            return (bool) UTInternalCall.InvokeStatic("UnityEditor.BuildPipeline", "IsBuildTargetSupported", target);
        }

        /// <summary>
        /// Checks if the user has the advanced license (PRO-license) for the given build target.
        /// </summary>
        public static bool HasAdvancedLicenseOn(BuildTarget target)
        {
            return InternalEditorUtility.HasAdvancedLicenseOnBuildTarget(target);
        }

        /// <summary>
        /// Computes the SHA-1 hash of the given bytes.
        /// </summary>
        /// <returns>
        /// The hash.
        /// </returns>
        /// <param name='bytes'>
        /// Bytes.
        /// </param>
        public static string ComputeHash(byte[] bytes)
        {
            using (var sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(bytes);
                var formatted = new StringBuilder(2*hash.Length);
                foreach (var b in hash)
                {
                    formatted.AppendFormat("{0:X2}", b);
                }
                return formatted.ToString().ToLower();
            }
        }

        /// <summary>
        /// Builds an URL from the given parts and removes duplicate slashes.
        /// </summary>
        /// <returns>
        /// The URL.
        /// </returns>
        /// <param name='parts'>
        /// Parts.
        /// </param>
        public static string BuildUrl(params string[] parts)
        {
            var result = String.Join("/", parts);
            // replace duplicate slashes
            result = Regex.Replace(result, "(?<!:)//+", "/");
            return result;
        }

        /// <summary>
        /// Clears Unity's console.
        /// </summary>
        public static void ClearConsole()
        {
            UTInternalCall.InvokeStatic("UnityEditorInternal.LogEntries", "Clear");
        }

        /// <summary>
        /// Shows an async progress bar in the lower right corner of Unity's window (like the one shown when
        /// rendering lightmaps).
        /// </summary>
        /// <param name='text'>
        /// Text to show.
        /// </param>
        /// <param name='progress'>
        /// Progress to show (anything between 0f and 1f)
        /// </param>
        public static void ShowAsyncProgressBar(string text, float progress)
        {
            UTInternalCall.InvokeStatic("UnityEditor.AsyncProgressBar", "Display", text, progress);
        }

        /// <summary>
        /// Hides the async progress bar.
        /// </summary>
        public static void ClearAsyncProgressBar()
        {
            UTInternalCall.InvokeStatic("UnityEditor.AsyncProgressBar", "Clear");
        }

        /// <summary>
        /// Gets the editor executable.
        /// </summary>
        /// <returns>
        /// The editor executable.
        /// </returns>
        public static string GetEditorExecutable()
        {
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                return EditorApplication.applicationPath + "/Contents/MacOS/Unity";
            }
            return EditorApplication.applicationPath;
        }

        /// <summary>
        /// Completes the editor executable path depending on the current operating system.
        /// </summary>
        public static string CompleteEditorExecutable(string executablePath)
        {
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                if (executablePath.EndsWith(".app"))
                {
                    return executablePath + "/Contents/MacOS/Unity";
                }
            }
            return executablePath;
        }

        /// <summary>
        /// Extension methods for structs which allows to check whether or not a certain value
        /// is inside a list of other values.
        /// </summary>
        /// <typeparam name="T">the struct type</typeparam>
        /// <param name="val">the actual value to check</param>
        /// <param name="values">the values to check against</param>
        /// <returns></returns>
        public static bool In<T>(this T val, params T[] values) where T : struct
        {
            return values.Contains(val);
        }

        /// <summary>
        /// Extension method for strings which allows to check if a string is within a certain set of alternatives.
        /// </summary>
        /// <param name="val">the string to test</param>
        /// <param name="values">the allowed values</param>
        /// <returns>true if the given string is inside the given values</returns>
        public static bool In(this string val, params string[] values)
        {
            return values.Contains(val);
        }

        /// <summary>
        /// Formats a given time span to a human readable representation.
        /// </summary>
        /// <param name="duration">the time span</param>
        /// <param name="shortLayout">should a short layout be used?</param>
        /// <returns></returns>
        public static string FormatTime(TimeSpan duration, bool shortLayout)
        {
            if (shortLayout)
            {
                return duration.Minutes.ToString("00") + ":" + duration.Seconds.ToString("00");
            }
            return (duration.Minutes > 0 ? duration.Minutes + "minutes " : "") + duration.Seconds + " seconds";
        }

        /// <summary>
        /// Converts a byte count into some human readable size.
        /// </summary>
        /// <param name="byteCount">the byte count</param>
        /// <returns>the human readable size</returns>
        public static string BytesToHumanReadable(long byteCount)
        {
            // based on some code from http://stackoverflow.com/users/483179/deepee1

            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
            if (byteCount == 0)
            {
                return "0" + suf[0];
            }
            var bytes = Math.Abs(byteCount);
            var place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            var num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num) + suf[place];
        }

        /// <summary>
        /// Checks wether or not a module for a certain build target is loaded in the editor.
        /// </summary>
        /// <param name="buildTarget">the target platform</param>
        /// <returns></returns>
        public static bool IsPlatformSupportLoaded(BuildTarget buildTarget)
        {
#if !(UNITY_5_1 || UNITY_5_1 || UNIY_5_2) // VR: 5.3
            var targetString = UTInternalCall.InvokeStatic("UnityEditor.Modules.ModuleManager", "GetTargetStringFromBuildTarget", buildTarget);
            return (bool) UTInternalCall.InvokeStatic("UnityEditor.Modules.ModuleManager", "IsPlatformSupportLoaded", targetString);
#else
            return true;
#endif
        }

        public static void OpenHelpFile(string filename, string url)
        {
            var assets = AssetDatabase.FindAssets(filename);
            if (assets.Length == 0)
            {
                // fallback if someone killed the PDF
                if (url != null)
                {
                    Debug.LogWarning("Unable to find " + filename + " in the project. Redirecting you to the website.");
                    Application.OpenURL(url);
                }
                else
                {
                    Debug.LogWarning("Unable to find " + filename + " in the project. Did you accidently delete it?");
                }
            }
            else
            {
                AssetDatabase.OpenAsset(AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(assets[0])));
            }
        }


        /// <summary>
        /// Checks whether the given action needs to be migrated and executes the migration if needed.
        /// </summary>
        /// <param name="action">the action to check</param>
        /// <param name="versionIntroducingTheChange">the version that introduced the change.</param>
        /// <param name="migration">the migration to execute</param>
        public static void MigrateActionIfRequired<T>(T action, float versionIntroducingTheChange, Action<T> migration) where T:UTAction
        {
            if (action.CreatedWithActionVersion == null || !(float.Parse(action.CreatedWithActionVersion) < versionIntroducingTheChange)) return;

            migration(action);
            action.CreatedWithActionVersion = versionIntroducingTheChange.ToString(CultureInfo.InvariantCulture);
            EditorUtility.SetDirty(action);
        }
    }
}
