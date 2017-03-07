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
	[UTDoc(title = "Build Xcode project", description = "Builds a Xcode project. Requires Xcode, therefore it is available on Mac platforms, only.", notice = "Make sure you can build the project with Xcode to avoid problems with missing certificates and provisioning profiles.")]
	[UTInspectorGroups(groups = new[] {"General", "Advanced"})]
	public class UTBuildXcodeProjectAction : UTAction
    {

        [UTDoc(description = "The installed Xcode version.")]
        [UTInspectorHint(group = "General", required = true, order = 1)]
        public UTXcodeVersion xcodeVersion;

		[UTDoc(description = "Location of the project file (.xcodeproj).")]
		[UTInspectorHint(group = "General", required = true, order = 2, displayAs = UTInspectorHint.DisplayAs.FolderSelect)]
		public UTString projectDirectory;
		
		[UTDoc(description = "The build scheme to use (e.g. 'Unity-iPhone')")]
		[UTInspectorHint(group="General", required = true, order = 3)]
		public UTString buildScheme;

        [UTDoc(description = "The id of the development team (you can find the id in your developer account https://developer.apple.com/account/#/membership)")]
        [UTInspectorHint(group="General", required = true, order = 4)]
        public UTString developmentTeamId;      // only important for xcode8+

        [UTDoc(description = "Should the project be built with an export options plist file?")]
        [UTInspectorHint(group="General", order = 5)]
        public UTBool useExportOptionsPlist;    // only important for xcode8+

        [UTDoc(description = "The name of the provisioning profile (e.g. 'iOSTeam Provisioning Profile: *')")]
		[UTInspectorHint(group="General", required = true, order = 6)]
		public UTString provisioningProfile;

        [UTDoc(description = "The name and path of the output file (the ipa)")]
        [UTInspectorHint(group = "General", required = true, order = 7)]
        public UTString outputFile;

        [UTDoc(description = "The export options plist file")]
        [UTInspectorHint(group="General", required = true, order = 8, displayAs = UTInspectorHint.DisplayAs.OpenFileSelect, caption = "Select the export options plist file.")]
        public UTString exportOptionsPlist;

        [UTDoc(description = "The path of the output directory")]
        [UTInspectorHint(group = "General", required = true, order = 9)]
        public UTString outputDirectory;

        [UTDoc(description = "Additional command line options for xcodebuild during build. Please add one option per line.")]
		[UTInspectorHint(group = "Advanced", order = 1)]      
		public UTString[] additionalBuildOptions;
		
		[UTDoc(description = "Additional command line options for xcodebuild during export. Please add one option per line.")]
		[UTInspectorHint(group = "Advanced", order = 2)]      
		public UTString[] additionalExportOptions;
		
        public void OnEnable()
        {
            // property is not yet initialized for new actions, only for existing ones
            UTils.MigrateActionIfRequired(this, "1.5", delegate(UTBuildXcodeProjectAction action)
            {
                action.xcodeVersion.StaticValue = XcodeVersion.Xcode7;
                action.useExportOptionsPlist.StaticValue = false;
                UDebug.Log("The action '" + action.name + "' has been migrated. Please verify the settings of this action.", action);
            });
        }

        public override IEnumerator Execute(UTContext context)
		{
			#if UNITY_EDITOR_OSX
		    var theXcodeVersion = xcodeVersion.EvaluateIn(context);

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
			
		    var theDevelopmentTeamId = developmentTeamId.EvaluateIn(context);
            if (theXcodeVersion == XcodeVersion.Xcode8 && string.IsNullOrEmpty((theDevelopmentTeamId)))
            {
                throw new UTFailBuildException("Development team id must not be empty.", this);
            }

		    var theUseExportOptionsPlist = false;
		    if (theXcodeVersion == XcodeVersion.Xcode8)
		    {
		        theUseExportOptionsPlist = useExportOptionsPlist.EvaluateIn(context);
		    }

		    string theProvisioningProfile = "";
		    string theExportOptionsPlist = "";
		    string theOutputLocation;

		    if (!theUseExportOptionsPlist)
		    {
		        theProvisioningProfile = provisioningProfile.EvaluateIn(context);
		        if (string.IsNullOrEmpty(theProvisioningProfile))
		        {
		            throw new UTFailBuildException("Provisioning file must not be empty.", this);
		        }

		        theOutputLocation = outputFile.EvaluateIn(context);
		        if (string.IsNullOrEmpty(theOutputLocation))
		        {
		            throw new UTFailBuildException("Output file must not be empty.", this);
		        }

		        if (Directory.Exists(theOutputLocation))
		        {
		            throw new UTFailBuildException("The specified output file " + theOutputLocation + " is a directory.", this);
		        }

		        // path to a single file
		        UTFileUtils.EnsureParentFolderExists(Path.Combine(theProjectDirectory, theOutputLocation));
		    }
		    else
		    {
		        theExportOptionsPlist = exportOptionsPlist.EvaluateIn(context);
		        if (string.IsNullOrEmpty(theExportOptionsPlist))
		        {
		            throw new UTFailBuildException("Export options plist must not be empty.", this);
		        }
		        if (Directory.Exists(theExportOptionsPlist))
                {
                    throw new UTFailBuildException("The specified export options plist file " + theExportOptionsPlist + " is a directory.", this);
                }

                if (!File.Exists(theExportOptionsPlist))
                {
                    throw new UTFailBuildException("The specified export options plist file " + theExportOptionsPlist + " does not exist.", this);
                }

		        theOutputLocation = outputDirectory.EvaluateIn(context);
		        if (string.IsNullOrEmpty(theOutputLocation))
		        {
		            throw new UTFailBuildException("Output directory must not be empty.", this);
		        }

		        if (File.Exists(theOutputLocation))
		        {
		            throw new UTFailBuildException("The specified output directory " + theOutputLocation + " is a file.", this);
		        }

                // path to a directory
		        UTFileUtils.EnsureFolderExists(Path.Combine(theProjectDirectory, theOutputLocation));
		    }

		    UTFileUtils.EnsureFolderExists(Path.Combine(theProjectDirectory, "build"));

			var buildArguments = new List<string>();		
			buildArguments.Add("-scheme");
			buildArguments.Add(UTExecutableParam.Quote(theBuildScheme));
			buildArguments.Add("clean");
			buildArguments.Add("archive");
			buildArguments.Add("-archivePath");
			buildArguments.Add("build/BuildArchive");
		    if (theXcodeVersion == XcodeVersion.Xcode8)
		    {
		        buildArguments.Add("DEVELOPMENT_TEAM=" + theDevelopmentTeamId);
		    }

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
			exportArguments.Add("-archivePath");
			exportArguments.Add("build/BuildArchive.xcarchive");
			exportArguments.Add("-exportPath");
			exportArguments.Add(UTExecutableParam.Quote(theOutputLocation));

		    if (!theUseExportOptionsPlist)
		    {
		        exportArguments.Add("-exportFormat");
		        exportArguments.Add("ipa");
		        exportArguments.Add("-exportProvisioningProfile");
		        exportArguments.Add(UTExecutableParam.Quote(theProvisioningProfile));
		    } else
		    {
		        exportArguments.Add("-exportOptionsPlist");
		        exportArguments.Add(UTExecutableParam.Quote(theExportOptionsPlist));
		    }

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
		    var result = Create<UTBuildXcodeProjectAction>();
		    result.xcodeVersion.StaticValue = XcodeVersion.Xcode8;
		    result.useExportOptionsPlist.StaticValue = false;
		}
	}
}
