//
// Copyright (c) 2013-2016 Ancient Light Studios
// All Rights Reserved
// 
// http://www.ancientlightstudios.com
//

namespace AncientLightStudios.uTomate
{
    using API;
    using Microsoft.CSharp;
    using System;
    using System.CodeDom.Compiler;
    using System.Collections;
    using System.IO;
    using UnityEditor;
    using UnityEngine;
    using System.Text;


    [UTActionInfo(actionCategory = "Build")]
    [UTDoc(title = "Build DLL", description = "Builds a DLL from C# sources.")]
    [UTInspectorGroups(groups = new[] {"General", "Sources", "Resources", "Dependencies"})]
    [UTDefaultAction]
    public class UTBuildDllAction : UTAction
    {
        [UTDoc(title = "Base Folder", description = "The base folder where to look for sources. Defaults to the current project's assets folder.")]
        [UTInspectorHint(order = 0, group = "Sources")]
        public UTString baseDirectory;

        [UTDoc(description = "The sources to include.")]
        [UTInspectorHint(order = 1, group = "Sources")]
        public UTString[] includes;

        [UTDoc(description = "The sources to exclude.")]
        [UTInspectorHint(order = 2, group = "Sources")]
        public UTString[] excludes;

        [UTDoc(title = "Resources Base Folder", description = "The base folder where to look for resources. Defaults to the current project's assets folder.")]
        [UTInspectorHint(order = 1, group = "Resources")]
        public UTString resourcesBaseDirectory;

        [UTDoc(description = "A list of resources to include. These will be embedded into the DLL. If empty, no resources will be included into the DLL.")]
        [UTInspectorHint(order = 2, group = "Resources")]
        public UTString[] includeResources;

        [UTDoc(description = "A list of resources to exclude. These will NOT be embedded into the DLL.")]
        [UTInspectorHint(order = 3, group = "Resources")]
        public UTString[] excludeResources;

        [UTDoc(description = "Include UnityEngine.dll als dependency.")]
        [UTInspectorHint(order = 1, group = "Dependencies")]
        public UTBool includeEngineDll;

        [UTDoc(description = "Include UnityEditor.dll als dependency.")]
        [UTInspectorHint(order = 2, group = "Dependencies")]
        public UTBool includeEditorDll;

        [UTDoc(description = "Include UnityEditor.Graphs.dll als dependency.")]
        [UTInspectorHint(order = 3, group = "Dependencies")]
        public UTBool includeGraphsDll;

        [UTDoc(description = "A list of DLL files which should be referenced when building. The UnityEngine.dll and UnityEditor.dll will be added automatically if you tick the checkboxes above.")]
        [UTInspectorHint(order = 5, group = "Dependencies")]
        public UTString[] referencedAssemblies;

        [UTDoc(description = "The full path of the output DLL.")]
        [UTInspectorHint(order = 1, required = true, displayAs = UTInspectorHint.DisplayAs.SaveFileSelect, caption = "Select output file.", group = "General")]
        public UTString outputFile;

        [UTDoc(description = "Any symbols that should be defined,separated by a semicolon or comma, e.g. 'UNITY_EDITOR;UNITY_4_0'")]
        [UTInspectorHint(order = 2, group = "General")]
        public UTString defineSymbols;

		[UTDoc(title = "Emit Debug Information", description = "Whether or not to create debug information for the dll.")]
		[UTInspectorHint(order = 3, group = "General")]
		public UTBool debugInformation;

        [UTDoc(title = "Compiler Options", description = "Additional compiler options that you want to set.")]
        [UTInspectorHint(order = 4, group = "General")]
        public UTString additionalCompilerOptions;


        public override IEnumerator Execute(UTContext context)
        {
            var options = new StringBuilder();
            var codeFiles = new StringBuilder();

            var theBaseDirectory = baseDirectory != null ? baseDirectory.EvaluateIn(context) : null;
            if (string.IsNullOrEmpty(theBaseDirectory))
            {
                theBaseDirectory = Application.dataPath;
            }

            if (!Directory.Exists(theBaseDirectory))
            {
                throw new UTFailBuildException("The base directory " + theBaseDirectory + " does not exist or is not a directory", this);
            }

			var realIncludes = EvaluateAll(includes, context);
			var realExcludes = EvaluateAll(excludes, context);			
			var fileList = UTFileUtils.CalculateFileset(theBaseDirectory, realIncludes, realExcludes, UTFileUtils.FileSelectionMode.Files);


			var theResourcesBaseDirectory = resourcesBaseDirectory != null ? resourcesBaseDirectory.EvaluateIn(context) : null;
			if (string.IsNullOrEmpty(theResourcesBaseDirectory))
			{
				theResourcesBaseDirectory = Application.dataPath;
			}
			
			if (!Directory.Exists(theResourcesBaseDirectory))
			{
				throw new UTFailBuildException("The resources base directory " + theResourcesBaseDirectory + " does not exist or is not a directory", this);
			}
			
			var theIncludedResources = EvaluateAll(includeResources, context);
			var theExcludedResources = EvaluateAll(excludeResources, context);
			var resourceFiles = UTFileUtils.CalculateFileset(theResourcesBaseDirectory, theIncludedResources, theExcludedResources, UTFileUtils.FileSelectionMode.Files);

			if (fileList.Length == 0 && resourceFiles.Length == 0)
			{
				throw new UTFailBuildException("No files were selected for build. Please check includes and excludes.", this);
			}
			
			var compiler = new CSharpCodeProvider();
			if (resourceFiles.Length > 0)
			{
				if (compiler.Supports(GeneratorSupport.Resources))
				{
					foreach (var resourceFile in resourceFiles)
					{
						options.Append(" /resource:" + UTExecutableParam.Quote(resourceFile));
					}
				}
				else
				{
					Debug.LogWarning("The underlying compiler does not support resources. Resources will not be added to the DLL.");
				}
			}

            var theOutputFile = outputFile.EvaluateIn(context);
            if (!theOutputFile.EndsWith(".dll"))
            {
                if (UTPreferences.DebugMode)
                {
                    Debug.LogWarning("The output file does not end with .dll. The built DLL will not be picked up by Unity.");
                }
            }

            UTFileUtils.EnsureParentFolderExists(theOutputFile);

            var doIncludeEngineDll = true;
            if (includeEngineDll != null)
            {
                // can happen when we migrate older automation plans which didn't have this setting.
                doIncludeEngineDll = includeEngineDll.EvaluateIn(context);
            }
            if (doIncludeEngineDll)
            {
                options.Append(" /reference:" + UTExecutableParam.Quote(UnityDll()));
            }
            if (includeEditorDll.EvaluateIn(context))
            {
                options.Append(" /reference:" + UTExecutableParam.Quote(UnityEditorDll()));
            }
            if (includeGraphsDll.EvaluateIn(context))
            {
                options.Append(" /reference:" + UTExecutableParam.Quote(UnityEditorGraphsDll()));
            }
            var realAssemblies = EvaluateAll(referencedAssemblies, context);
            foreach (var asm in realAssemblies)
            {
                options.Append(" /reference:" + UTExecutableParam.Quote(asm));
            }

            options.Append(" /target:library");

            var theSymbols = defineSymbols.EvaluateIn(context);
            if (!string.IsNullOrEmpty(theSymbols))
            {
                options.Append(" /define:" + UTExecutableParam.Quote(theSymbols.Trim()));
            }

			var theDebugInformation = debugInformation.EvaluateIn(context);

            var theAdditionalOptions = additionalCompilerOptions.EvaluateIn(context);
            if (!string.IsNullOrEmpty(theAdditionalOptions))
            {
                options.Append(" " + theAdditionalOptions.Trim());
            }

            SetPath();
            foreach (var file in fileList)
            {
                if (UTPreferences.DebugMode)
                {
                    Debug.Log("Adding source file: " + file);
                }
                codeFiles.Append(" " + UTExecutableParam.Quote(file));
            }

            Debug.Log("Compiling " + fileList.Length + " files and " + resourceFiles.Length + " resources.");


            var responseFile = UTFileUtils.TempFile(".rsp");
            File.WriteAllText(responseFile, options + codeFiles.ToString());

            var parameters = new CompilerParameters {CompilerOptions = "@" + UTExecutableParam.Quote(responseFile), OutputAssembly = theOutputFile, IncludeDebugInformation = theDebugInformation};

            var path = Environment.GetEnvironmentVariable("PATH");

            // on OSX Unity has messed up the paths to gmcs so we need to fix this here...
            if (Application.platform == RuntimePlatform.OSXEditor)
            {

                var pathIncludingMono = path + Path.PathSeparator + EditorApplication.applicationContentsPath +
#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 // VR: [5.0, 5.3]
                                        "/Frameworks/Mono/bin";
#else
                                        "/Mono/bin";
#endif
                Environment.SetEnvironmentVariable("PATH", pathIncludingMono, EnvironmentVariableTarget.Process);
            }

            CompilerResults results;
            try
            {
                results = compiler.CompileAssemblyFromFile(parameters);
            }
            finally
            {
                // make sure the path variable is reset.
                if (Application.platform == RuntimePlatform.OSXEditor)
                {
                    Environment.SetEnvironmentVariable("PATH", path, EnvironmentVariableTarget.Process);
                }
            }


            // ensure that temp file is deleted
            File.Delete(responseFile);

            if (UTPreferences.DebugMode)
            {
                var output = results.Output;
                foreach (var line in output)
                {
                    Debug.Log(line);
                }
            }

            var errors = results.Errors;
            var hadErrors = false;
            foreach (CompilerError error in errors)
            {
                if (error.IsWarning || error.ToString().Contains("related to previous warning"))
                {
                    Debug.LogWarning(error.ToString());
                }
                else
                {
                    hadErrors = true;
                    Debug.LogError(error.ToString()); // TODO link errors to file.
                }
            }
            if (hadErrors)
            {
                throw new UTFailBuildException("There were compilation errors.", this);
            }
            Debug.Log("Built " + theOutputFile + " from " + fileList.Length + " source file(s) " + (resourceFiles.Length > 0 ? " and " + resourceFiles.Length + " resource file(s)." : "."));
            yield return "";
        }

        private static string UnityDll()
        {
#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 // VR: [5.0,5.3]            
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                return EditorApplication.applicationContentsPath + "/Frameworks/Managed/UnityEngine.dll";
            }
#endif
            return EditorApplication.applicationContentsPath + "/Managed/UnityEngine.dll";
        }

        private static string UnityEditorDll()
        {
#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 // VR: [5.0,5.3]            
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                return EditorApplication.applicationContentsPath + "/Frameworks/Managed/UnityEditor.dll";
            }
#endif
            return EditorApplication.applicationContentsPath + "/Managed/UnityEditor.dll";
        }

        private static string UnityEditorGraphsDll()
        {
#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 // VR: [5.0,5.3]            
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                return EditorApplication.applicationContentsPath + "/Frameworks/Managed/UnityEditor.Graphs.dll";
            }
#endif
            return EditorApplication.applicationContentsPath + "/Managed/UnityEditor.Graphs.dll";
        }

        private static void SetPath()
        {
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                var monoPath = EditorApplication.applicationPath + "/Contents/Frameworks/Mono/bin";
                if (!Environment.GetEnvironmentVariable("PATH").Contains(monoPath))
                {
                    Environment.SetEnvironmentVariable("PATH", monoPath + ":" + Environment.GetEnvironmentVariable("PATH"));
                }
            }
        }

        [MenuItem("Assets/Create/uTomate/Build/Build DLL", false, 390)]
        public static void AddAction()
        {
            Create<UTBuildDllAction>();
        }
    }
}
