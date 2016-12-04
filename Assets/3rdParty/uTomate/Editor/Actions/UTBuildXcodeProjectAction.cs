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
	[UTDoc(title = "Build Xcode project", description = "Builds a Xcode project. Requires Xcode, therefore it is available on Mac platforms, only.")]
	[UTInspectorGroups(groups = new[] {"General", "Advanced"})]
	public class UTBuildXcodeProjectAction : UTAction
	{
		[UTDoc(description = "Location of the project file (.xcodeproj).")]
		[UTInspectorHint(group = "General", required = true, order = 1, displayAs = UTInspectorHint.DisplayAs.FolderSelect)]
		public UTString projectDirectory;
		
		[UTDoc(description = "The build scheme to use (e.g. 'Unity-iPhone')")]
		[UTInspectorHint(group="General", required = true, order = 2)]
		public UTString buildScheme;
		
		[UTDoc(description = "The name of the provisioning profile (e.g. 'iOSTeam Provisioning Profile: *')")]
		[UTInspectorHint(group="General", required = true, order = 3)]
		public UTString provisioningProfile;
		
		[UTDoc(description = "The name and path of the output file (the ipa)")]
		[UTInspectorHint(group = "General", required = true, order = 4)]      
		public UTString outputFile;
		
		[UTDoc(description = "Additional command line options for xcodebuild during build. Please add one option per line.")]
		[UTInspectorHint(group = "Advanced", order = 1)]      
		public UTString[] additionalBuildOptions;
		
		[UTDoc(description = "Additional command line options for xcodebuild during export. Please add one option per line.")]
		[UTInspectorHint(group = "Advanced", order = 2)]      
		public UTString[] additionalExportOptions;
		

		public override IEnumerator Execute(UTContext context)
		{
			#if UNITY_EDITOR_OSX 
			var theProjectDirectory = projectDirectory.EvaluateIn(context);
			if (!Directory.Exists(theProjectDirectory))
			{
				throw new UTFailBuildException("Project directory '" + theProjectDirectory + "' does not exist.", this );
			}
			
			var theBuildScheme = buildScheme.EvaluateIn(context);
			if (string.IsNullOrEmpty(theBuildScheme))
			{
				throw new UTFailBuildException("Build scheme must not be empty.", this);
			}
			
			var theProvisioningProfile = provisioningProfile.EvaluateIn(context);
			if (string.IsNullOrEmpty(theProvisioningProfile))
			{
				throw new UTFailBuildException("Provisioning file must not be empty.", this);
			}
			
			var theOutputFile = outputFile.EvaluateIn(context);
			if (string.IsNullOrEmpty(theOutputFile))
			{
				throw new UTFailBuildException("Output file must not be empty.", this);
			}

			var buildArguments = new List<string>();		
			buildArguments.Add("-scheme");
			buildArguments.Add(UTExecutableParam.Quote(theBuildScheme));
			buildArguments.Add("clean");
			buildArguments.Add("archive");
			buildArguments.Add("-archivePath");
			buildArguments.Add("build/BuildArchive");

			var theBuildOptions = EvaluateAll(additionalBuildOptions, context);
			buildArguments.AddRange(theBuildOptions.Select(theOption => UTExecutableParam.Quote(theOption)));
			
			var finalBuildArgs = string.Join(" ", buildArguments.ToArray());

			var process = new Process {StartInfo =
				{
					FileName = "xcodebuild",
					Arguments = finalBuildArgs,  
					UseShellExecute = false,
					CreateNoWindow = true,
					WorkingDirectory = theProjectDirectory
				}};
			
			if (UTPreferences.DebugMode)
			{
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.RedirectStandardError = true;
				process.OutputDataReceived += (sender, argv) => UDebug.Log("[xcodebuild]" + argv.Data);
				process.ErrorDataReceived += (sender, argv) => UDebug.LogWarning("[xcodebuild]" + argv.Data);
			}
			
			try
			{
				UDebug.Log("Starting xcodebuild process");
				if (UTPreferences.DebugMode)
				{
					UDebug.Log("Args: " + finalBuildArgs );
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


			var exportArguments = new List<string>();
			exportArguments.Add("-exportArchive");
			exportArguments.Add("-exportFormat");
			exportArguments.Add("ipa");
			exportArguments.Add("-archivePath");
			exportArguments.Add("build/BuildArchive.xcarchive");
			exportArguments.Add("-exportPath");
			exportArguments.Add(UTExecutableParam.Quote(theOutputFile));
			exportArguments.Add("-exportProvisioningProfile");
			exportArguments.Add(UTExecutableParam.Quote(theProvisioningProfile));

			var theExportOptions = EvaluateAll(additionalExportOptions, context);
			exportArguments.AddRange(theExportOptions.Select(theOption => UTExecutableParam.Quote(theOption)));
			
			var finalExportArgs = string.Join(" ", exportArguments.ToArray());
			
			process = new Process {StartInfo =
				{
					FileName = "xcodebuild",
					Arguments = finalExportArgs,  
					UseShellExecute = false,
					CreateNoWindow = true,
					WorkingDirectory = theProjectDirectory
				}};
			
			if (UTPreferences.DebugMode)
			{
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.RedirectStandardError = true;
				process.OutputDataReceived += (sender, argv) => UDebug.Log("[xcodebuild]" + argv.Data);
				process.ErrorDataReceived += (sender, argv) => UDebug.LogWarning("[xcodebuild]" + argv.Data);
			}
			
			try
			{
				UDebug.Log("Starting xcodebuild process");
				if (UTPreferences.DebugMode)
				{
					UDebug.Log("Args: " + finalExportArgs );
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
			throw new UTFailBuildException("The 'Build Xcode Project' action is only available on Mac platforms.", this);
			#endif
		}
		
		[MenuItem("Assets/Create/uTomate/Build/Build Xcode Project", false, 396)]
		public static void AddAction()
		{
			Create<UTBuildXcodeProjectAction>();
		}
	}
}
