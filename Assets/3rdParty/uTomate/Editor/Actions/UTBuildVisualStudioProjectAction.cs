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
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using API;
    using UnityEditor;
    using UDebug = UnityEngine.Debug;

    [UTActionInfo(actionCategory = "Build", sinceUTomateVersion = "1.6.0")]
    [UTDoc(title = "Build Visual Studio project", description = "Builds a Visual Studio project using MSBuild. Requires Visual Studio 2013, therefore it is available on Windows platforms, only.")]
    [UTInspectorGroups(groups = new[] {"General", "Advanced"})]
    public class UTBuildVisualStudioProjectAction : UTAction
    {
        [UTDoc(description = "Location of the project file. This may be a SLN or CSPROJ file.")]
        [UTInspectorHint(group = "General", required = true, order = 1, displayAs = UTInspectorHint.DisplayAs.OpenFileSelect)]
        public UTString projectFile;
        

        [UTDoc(description = "The build configuration to use (e.g. 'Debug' or 'Release'")]
        [UTInspectorHint(group="General", order = 2)]
        public UTString buildConfiguration;

        [UTDoc(description = "The target platform to use, e.g. 'x86' or 'ARM' or 'Any CPU'")]
        [UTInspectorHint(group = "General", order = 3)]      
        public UTString targetPlatform;

        [UTDoc(description = "Additional command line options for MSBuild. Please add one option per line.")]
        [UTInspectorHint(group = "Advanced", order = 1)]      
        public UTString[] additionalOptions;


        public override IEnumerator Execute(UTContext context)
        {
			#if UNITY_EDITOR_WIN 

            var arguments = new List<string>();

            var theProjectFile = projectFile.EvaluateIn(context);
            if (!File.Exists(theProjectFile))
            {
                throw new UTFailBuildException("Project file '" + theProjectFile + "' does not exist.", this );
            }

            arguments.Add(UTExecutableParam.Quote(theProjectFile));

            var programFiles = ProgramFilesx86();
            var msBuild = Path.Combine(programFiles, "MSBuild/12.0/Bin/MSBuild.exe");
            if (!File.Exists(msBuild))
            {
                throw new UTFailBuildException("Unable to find MSBuild at '" + msBuild + "'. Make sure you have Visual Studio 2013 installed.", this);
            }

            var theBuildConfiguration = buildConfiguration.EvaluateIn(context);
            if (!string.IsNullOrEmpty(theBuildConfiguration))
            {
                arguments.Add("/p:Configuration=" + UTExecutableParam.Quote(theBuildConfiguration));
            }

            var theTargetPlatform = targetPlatform.EvaluateIn(context);
            if (!string.IsNullOrEmpty(theTargetPlatform))
            {
                arguments.Add("/p:Platform=" + UTExecutableParam.Quote(theTargetPlatform));
            }

            // ReSharper disable once CoVariantArrayConversion
            var theOptions = EvaluateAll(additionalOptions, context);
            arguments.AddRange(theOptions.Select(theOption => UTExecutableParam.Quote(theOption)));

            var finalArgs = string.Join(" ", arguments.ToArray());

            var process = new Process {StartInfo =
            {
                FileName = msBuild,
                Arguments = finalArgs,  
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = Path.GetDirectoryName(theProjectFile)
            }};

            if (UTPreferences.DebugMode)
            {
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.OutputDataReceived += (sender, argv) => UDebug.Log("[MSBuild]" + argv.Data);
                process.ErrorDataReceived += (sender, argv) => UDebug.LogWarning("[MSBuild]" + argv.Data);
            }

            try
            {
                UDebug.Log("Starting process " + msBuild);
                if (UTPreferences.DebugMode)
                {
                    UDebug.Log("Args: " + finalArgs );
                }

                process.Start();
                if (UTPreferences.DebugMode)
                {
                    process.BeginOutputReadLine();
                }
            }
            catch (Win32Exception e)
            {
                throw new UTFailBuildException("Couldn't start process: " + e.Message, this);
            }

            // wait for build to finish
            do
            {
                yield return "";
                if (context.CancelRequested && !process.HasExited) {
                    process.Kill();
                    yield break;
                }
            } while (!process.HasExited);

            if (process.ExitCode != 0)
            {
                throw new UTFailBuildException("Process exited with non-zero exit code " + process.ExitCode, this);
            }
			#else
			throw new UTFailBuildException("The 'Build Visual Studio Project' action is only available on Windows platforms.", this);
			#endif
        }


        static string ProgramFilesx86()
        {
            // there are dedicated functions for this in .Net 4, but we don't have that yet.
            if (8 == IntPtr.Size || (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432"))))
            {
                return Environment.GetEnvironmentVariable("ProgramFiles(x86)");
            }

            return Environment.GetEnvironmentVariable("ProgramFiles");
        }

        [MenuItem("Assets/Create/uTomate/Build/Build Visual Studio Project", false, 395)]
        public static void AddAction()
        {
            Create<UTBuildVisualStudioProjectAction>();
        }
    }
}
