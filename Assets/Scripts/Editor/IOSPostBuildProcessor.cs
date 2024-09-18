#if UNITY_IOS

using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.iOS.Xcode;

namespace Editor
{
    public class IOSPostBuildProcessor : IPostprocessBuildWithReport
    {
        public void OnPostprocessBuild(BuildReport report)
    {
        if (report.summary.platform == BuildTarget.iOS)
        {
            // Path to the built Xcode project
            string pathToBuiltProject = report.summary.outputPath;

            // Path to the Info.plist file
            string plistPath = Path.Combine(pathToBuiltProject, "Info.plist");

            // Load and modify the Info.plist file
            var plist = new PlistDocument();
            plist.ReadFromFile(plistPath);
            var root = plist.root;
            root.SetBoolean("ITSAppUsesNonExemptEncryption", false);
            plist.WriteToFile(plistPath);

            UnityEngine.Debug.Log("Set ITSAppUsesNonExemptEncryption to false in Info.plist");

            // Path to the project.pbxproj file
            string pbxPath = Path.Combine(pathToBuiltProject, "Unity-iPhone.xcodeproj", "project.pbxproj");

            // Load the project.pbxproj file
            var project = new PBXProject();
            project.ReadFromFile(pbxPath);

            // Disable Bitcode
            string target = project.TargetGuidByName("Unity-iPhone");
            project.SetBuildProperty(target, "ENABLE_BITCODE", "NO");

            // Add -ld64 to Other Linker Flags for UnityFramework
            string unityFrameworkTarget = project.TargetGuidByName("UnityFramework");
            if (!string.IsNullOrEmpty(unityFrameworkTarget))
            {
                var buildSettings = project.BuildSettingsForTarget(unityFrameworkTarget);
                if (buildSettings != null)
                {
                    var otherLinkerFlags = buildSettings["OTHER_LDFLAGS"];
                    if (!otherLinkerFlags.Contains("-ld64"))
                    {
                        buildSettings["OTHER_LDFLAGS"] = otherLinkerFlags + " -ld64";
                    }
                }
            }

            // Save the modified project.pbxproj file
            project.WriteToFile(pbxPath);

            UnityEngine.Debug.Log("Updated build settings in project.pbxproj");
        }
    }

        // Implement the required method for IPostprocessBuildWithReport
        public int callbackOrder => 0;
    }
}

#endif