//
// Copyright (c) 2013-2016 Ancient Light Studios
// All Rights Reserved
// 
// http://www.ancientlightstudios.com
//

#if !UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2 // VR: 5.3
namespace AncientLightStudios.uTomate
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Text.RegularExpressions;
    using API;
    using UnityEditor;
    using UnityEngine;
    using UnityWsaCapability=UnityEditor.PlayerSettings.WSACapability;

    [UTDoc(title = "Set Windows Store Player Settings", description = "Sets the player settings for Windows Store builds.")]
    [UTActionInfo(actionCategory = "Build", minUnityVersion = "5.3", sinceUTomateVersion = "1.7.0")]
    [UTInspectorGroups(groups = new[] { "Resolution & Presentation", "Icon", 
        "Windows Tiles & Logos", "Windows Phone Tiles & Logos",
        "Universal 10 Tiles & Logos",
        "Splash Image", "Rendering", "Configuration", "Optimization", "Publishing Settings" })]
    [UTDefaultAction]
    public class UTSetPlayerSettingsWindowsStoreAction : UTSetPlayerSettingsActionBase, UTICanLoadSettingsFromEditor
    {
        private static readonly Regex PackageNameRegex = new Regex("^[A-Za-z0-9\\.\\-]{2,49}[A-Za-z0-9\\-]$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
        private static readonly Regex PackageVersionRegex = new Regex("^(\\d+)\\.(\\d+)\\.(\\d+)\\.(\\d+)$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        // ----------- RESOLUTION ------------
        [UTDoc(description = "Should the application be able to run in the background? Warning: Setting this will cause Windows Application Certification to fail.")]
        [UTInspectorHint(@group = "Resolution & Presentation", order = 10)]
        public UTBool runInBackground;


        // ----------- ICON ------------------
        [UTInspectorHint(@group = "Icon", subSectionHeader = "Store Logo", fixedLength = 8, captions =new []{"100% (50x50)", "125% (63x63)", "140% (70x70)", "150% (75x75)", "180% (90x90)", "200% (100x100)", "240% (120x120)", "400% (200x200)"},
            displayAs = UTInspectorHint.DisplayAs.OpenFileSelect, extension = "jpg,png", order = 0)]
        public UTTexture2D[] storeLogo;

        private static readonly ImageMap[] StoreLogoMaps = MakeImageMap(50, 50, PlayerSettings.WSAImageType.PackageLogo,
            PlayerSettings.WSAImageScale._100, PlayerSettings.WSAImageScale._125, PlayerSettings.WSAImageScale._140, PlayerSettings.WSAImageScale._150,
            PlayerSettings.WSAImageScale._180, PlayerSettings.WSAImageScale._200, PlayerSettings.WSAImageScale._240, PlayerSettings.WSAImageScale._400);

        [UTDoc(description = "An abbreviated name for the app")]
        [UTInspectorHint(@group = "Icon", subSectionHeader  = "Tile", order = 10)]
        public UTString shortName;

        [UTDoc(title = "Show on medium tile", description = "Should the name be shown on the medium tile?")]
        [UTInspectorHint(@group = "Icon", order = 30)]
        public UTBool showNameOnMediumTile;

        [UTDoc(title = "Show on large tile", description = "Should the name be shown on the large tile?")]
        [UTInspectorHint(@group = "Icon", order = 40)]
        public UTBool showNameOnLargeTile;

        [UTDoc(title = "Show on wide tile", description = "Should the name be shown on the wide tile?")]
        [UTInspectorHint(@group = "Icon", order = 50)]
        public UTBool showNameOnWideTile;

        [UTDoc(description = "Value of the text color relative to the app tile's background color.")]
        [UTInspectorHint(@group = "Icon", order = 60)]
        public UTWSAApplicationForegroundText foregroundText;

        [UTDoc(description = "The background color of the app's tile in Windows.")]
        [UTInspectorHint(@group = "Icon", order = 70)]
        public UTColor backgroundColor;

        [UTDoc(description = "The default size of the tile.")]
        [UTInspectorHint(@group = "Icon", order = 80)]
        public UTWSADefaultTileSize defaulSize;

        // ---------- WINDOWS LOGOS & TILES -------------------------

        [UTInspectorHint(@group = "Windows Tiles & Logos", subSectionHeader = "Small Logo", fixedLength = 4, captions = new[] { "80% (24x24)", "100% (30x30)", "140% (42x42)", "180% (54x54)" },
            displayAs = UTInspectorHint.DisplayAs.OpenFileSelect, extension = "jpg,png", order=10)]
        public UTTexture2D[] windowsSmallLogo;

        private static readonly ImageMap[] WindowsSmallLogoMaps = MakeImageMap(30, 30, PlayerSettings.WSAImageType.StoreTileSmallLogo,
            PlayerSettings.WSAImageScale._80, PlayerSettings.WSAImageScale._100, PlayerSettings.WSAImageScale._140, PlayerSettings.WSAImageScale._180);

        [UTInspectorHint(@group = "Windows Tiles & Logos", subSectionHeader = "Medium Tile", fixedLength = 4, captions = new[] { "80% (120x120)", "100% (150x150)", "140% (210x210)", "180% (270x270)" },
             displayAs = UTInspectorHint.DisplayAs.OpenFileSelect, extension = "jpg,png", order=20)]
        public UTTexture2D[] windowsMediumTile;

        private static readonly ImageMap[] WindowsMediumTileMaps = MakeImageMap(150, 150, PlayerSettings.WSAImageType.StoreTileLogo,
            PlayerSettings.WSAImageScale._80, PlayerSettings.WSAImageScale._100, PlayerSettings.WSAImageScale._140, PlayerSettings.WSAImageScale._180);

        [UTInspectorHint(@group = "Windows Tiles & Logos", subSectionHeader = "Wide Tile", fixedLength = 4, captions = new[] { "80% (248x120)", "100% (310x150)", "140% (434x210)", "180% (558x270)" },
             displayAs = UTInspectorHint.DisplayAs.OpenFileSelect, extension = "jpg,png", order=30)]
        public UTTexture2D[] windowsWideTile;

        private static readonly ImageMap[] WindowsWideTileMaps = MakeImageMap(310, 150, PlayerSettings.WSAImageType.StoreTileWideLogo,
            PlayerSettings.WSAImageScale._80, PlayerSettings.WSAImageScale._100, PlayerSettings.WSAImageScale._140, PlayerSettings.WSAImageScale._180);

        [UTInspectorHint(@group = "Windows Tiles & Logos", subSectionHeader = "Small Tile", fixedLength = 4, captions = new[] { "80% (56x56)", "100% (70x70)", "140% (98x98)", "180% (126x126)" },
             displayAs = UTInspectorHint.DisplayAs.OpenFileSelect, extension = "jpg,png", order=40)]
        public UTTexture2D[] windowsSmallTile;

        private static readonly ImageMap[] WindowsSmallTileMaps = MakeImageMap(70, 70, PlayerSettings.WSAImageType.StoreSmallTile,
            PlayerSettings.WSAImageScale._80, PlayerSettings.WSAImageScale._100, PlayerSettings.WSAImageScale._140, PlayerSettings.WSAImageScale._180);


        [UTInspectorHint(@group = "Windows Tiles & Logos", subSectionHeader = "Large Tile", fixedLength = 4, captions = new[] { "80% (248x248)", "100% (310x310)", "140% (434x434)", "180% (558x558)" },
            displayAs = UTInspectorHint.DisplayAs.OpenFileSelect, extension = "jpg,png", order=50)]
        public UTTexture2D[] windowsLargeTile;

        private static readonly ImageMap[] WindowsLargeTileMaps = MakeImageMap(310, 310, PlayerSettings.WSAImageType.StoreLargeTile,
            PlayerSettings.WSAImageScale._80, PlayerSettings.WSAImageScale._100, PlayerSettings.WSAImageScale._140, PlayerSettings.WSAImageScale._180);


        // ------ WINDOWS PHONE TILES & LOGOS --------------

        [UTInspectorHint(@group = "Windows Phone Tiles & Logos", subSectionHeader = "Application Icon", fixedLength = 3, captions = new[] { "100% (44x44)", "140% (62x62)", "240% (106x106)" },
            displayAs = UTInspectorHint.DisplayAs.OpenFileSelect, extension = "jpg,png", order = 10)]
        public UTTexture2D[] windowsPhoneApplicationIcon;

        private static readonly ImageMap[] WindowsPhoneApplicationIconMaps = MakeImageMap(44, 44, PlayerSettings.WSAImageType.PhoneAppIcon,
            PlayerSettings.WSAImageScale._100, PlayerSettings.WSAImageScale._140, PlayerSettings.WSAImageScale._240);

        [UTInspectorHint(@group = "Windows Phone Tiles & Logos", subSectionHeader = "Small Tile", fixedLength = 3, captions = new[] { "100% (71x71)", "140% (99x99)", "240% (170x170)" },
            displayAs = UTInspectorHint.DisplayAs.OpenFileSelect, extension = "jpg,png", order = 20)]
        public UTTexture2D[] windowsPhoneSmallTile;

        private static readonly ImageMap[] WindowsPhoneSmallTileMaps = MakeImageMap(71, 71, PlayerSettings.WSAImageType.PhoneSmallTile,
            PlayerSettings.WSAImageScale._100, PlayerSettings.WSAImageScale._140, PlayerSettings.WSAImageScale._240);

        [UTInspectorHint(@group = "Windows Phone Tiles & Logos", subSectionHeader = "Medium Tile", fixedLength = 3, captions = new[] { "100% (150x150)", "140% (210x210)", "240% (360x360)" },
            displayAs = UTInspectorHint.DisplayAs.OpenFileSelect, extension = "jpg,png", order = 30)]
        public UTTexture2D[] windowsPhoneMediumTile;

        private static readonly ImageMap[] WindowsPhoneMediumTileMaps = MakeImageMap(150, 150, PlayerSettings.WSAImageType.PhoneMediumTile,
            PlayerSettings.WSAImageScale._100, PlayerSettings.WSAImageScale._140, PlayerSettings.WSAImageScale._240);

        [UTInspectorHint(@group = "Windows Phone Tiles & Logos", subSectionHeader = "Wide Tile", fixedLength = 3, captions = new[] { "100% (310x150)", "140% (434x210)", "240% (744x360)" },
            displayAs = UTInspectorHint.DisplayAs.OpenFileSelect, extension = "jpg,png", order = 40)]
        public UTTexture2D[] windowsPhoneWideTile;

        private static readonly ImageMap[] WindowsPhoneWideTileMaps = MakeImageMap(310, 150, PlayerSettings.WSAImageType.PhoneWideTile,
            PlayerSettings.WSAImageScale._100, PlayerSettings.WSAImageScale._140, PlayerSettings.WSAImageScale._240);

        // ------ UNIVERSAL WINDOWS 10 TILES & LOGOS --------------
        [UTInspectorHint(@group = "Universal 10 Tiles & Logos", subSectionHeader = "Square 44x44 Logo", fixedLength = 9, captions = new[]
        {
            "100% (44x44)", "125% (55x55)", "150% (66x66)", "200% (88x88)", "400% (176x176)",
            "16x16", "24x24", "48x48", "256x256"
        },
         displayAs = UTInspectorHint.DisplayAs.OpenFileSelect, extension = "jpg,png", order=10)]
        public UTTexture2D[] universal10Square44Logo;

        private static readonly ImageMap[] Universal10Square44LogoMaps = MakeImageMap(44, 44, PlayerSettings.WSAImageType.UWPSquare44x44Logo,
            PlayerSettings.WSAImageScale._100, PlayerSettings.WSAImageScale._125, PlayerSettings.WSAImageScale._150, PlayerSettings.WSAImageScale._200, PlayerSettings.WSAImageScale._400,
            PlayerSettings.WSAImageScale.Target16, PlayerSettings.WSAImageScale.Target24, PlayerSettings.WSAImageScale.Target48, PlayerSettings.WSAImageScale.Target256);

        [UTInspectorHint(@group = "Universal 10 Tiles & Logos", subSectionHeader = "Square 71x71 Logo", fixedLength = 5, captions = new[] {"100% (71x71)", "125% (89x89)", "150% (107x107)", "200% (142x142)", "400% (284x284)"},
            displayAs = UTInspectorHint.DisplayAs.OpenFileSelect, extension = "jpg,png", order = 20)]
        public UTTexture2D[] universal10Square71Logo;

        private static readonly ImageMap[] Universal10Square71LogoMaps = MakeImageMap(71, 71, PlayerSettings.WSAImageType.UWPSquare71x71Logo,
            PlayerSettings.WSAImageScale._100, PlayerSettings.WSAImageScale._125, PlayerSettings.WSAImageScale._150, PlayerSettings.WSAImageScale._200, PlayerSettings.WSAImageScale._400);

        [UTInspectorHint(@group = "Universal 10 Tiles & Logos", subSectionHeader = "Square 150x150 Logo", fixedLength = 5, captions = new[] {"100% (150x150)", "125% (188x188)", "150% (225x225)", "200% (300x300)", "400% (600x600)"},
            displayAs = UTInspectorHint.DisplayAs.OpenFileSelect, extension = "jpg,png", order = 30)]
        public UTTexture2D[] universal10Square150Logo;

        private static readonly ImageMap[] Universal10Square150LogoMaps = MakeImageMap(150, 150, PlayerSettings.WSAImageType.UWPSquare150x150Logo,
            PlayerSettings.WSAImageScale._100, PlayerSettings.WSAImageScale._125, PlayerSettings.WSAImageScale._150, PlayerSettings.WSAImageScale._200, PlayerSettings.WSAImageScale._400);

        [UTInspectorHint(@group = "Universal 10 Tiles & Logos", subSectionHeader = "Square 310x310 Logo", fixedLength = 5, captions = new[] {"100% (310x310)", "125% (388x388)", "150% (465x465)", "200% (620x620)", "400% (1240x1240)"},
            displayAs = UTInspectorHint.DisplayAs.OpenFileSelect, extension = "jpg,png", order=40)]
        public UTTexture2D[] universal10Square310Logo;

        private static readonly ImageMap[] Universal10Square310LogoMaps = MakeImageMap(310, 310, PlayerSettings.WSAImageType.UWPSquare310x310Logo,
            PlayerSettings.WSAImageScale._100, PlayerSettings.WSAImageScale._125, PlayerSettings.WSAImageScale._150, PlayerSettings.WSAImageScale._200, PlayerSettings.WSAImageScale._400);

        [UTInspectorHint(@group = "Universal 10 Tiles & Logos", subSectionHeader = "Wide 310x150 Logo", fixedLength = 5, captions = new[] {"100% (310x150)", "125% (388x188)", "150% (465x225)", "200% (620x300)", "400% (1240x600)"},
            displayAs = UTInspectorHint.DisplayAs.OpenFileSelect, extension = "jpg,png", order = 50)]
        public UTTexture2D[] universal10Wide310Logo;

        private static readonly ImageMap[] Universal10Wide310LogoMaps = MakeImageMap(310, 150, PlayerSettings.WSAImageType.UWPWide310x150Logo,
            PlayerSettings.WSAImageScale._100, PlayerSettings.WSAImageScale._125, PlayerSettings.WSAImageScale._150, PlayerSettings.WSAImageScale._200, PlayerSettings.WSAImageScale._400);

        // ----------- SPLASH IMAGE ------------
        [UTInspectorHint(@group = "Splash Image", subSectionHeader = "Windows Splash Image", fixedLength = 7, captions = new[] { "100% (620x300)", "125% (775x375)", "140% (868x420)", "150% (930x450)", "180% (1116x540)", "200% (1240x600)", "400% (2480x1200)" },
          displayAs = UTInspectorHint.DisplayAs.OpenFileSelect, extension = "jpg,png", order=20)]
        public UTTexture2D[] windowsSplashImage;

        private static readonly ImageMap[] WindowsSplashImageMaps = MakeImageMap(620, 300, PlayerSettings.WSAImageType.SplashScreenImage,
            PlayerSettings.WSAImageScale._100, PlayerSettings.WSAImageScale._125, PlayerSettings.WSAImageScale._140, PlayerSettings.WSAImageScale._150, 
            PlayerSettings.WSAImageScale._180, PlayerSettings.WSAImageScale._200, PlayerSettings.WSAImageScale._400);

        [UTInspectorHint(@group = "Splash Image", subSectionHeader = "Windows Phone Splash Image", fixedLength = 3, captions = new[] { "100% (480x800)", "140% (672x1120)", "240% (1152x1920)" },
          displayAs = UTInspectorHint.DisplayAs.OpenFileSelect, extension = "jpg,png", order=30)]
        public UTTexture2D[] windowsPhoneSplashImage;

        private static readonly ImageMap[] WindowsPhoneSplashImageMaps = MakeImageMap(480, 800, PlayerSettings.WSAImageType.PhoneSplashScreen,
            PlayerSettings.WSAImageScale._100, PlayerSettings.WSAImageScale._140, PlayerSettings.WSAImageScale._240);

        [UTDoc(description = "Should the default background color be changed to something else?")]
        [UTInspectorHint(@group = "Splash Image", order=40)]
        public UTBool overwriteBackgroundColor;

        [UTDoc(title="Background Color", description = "The new background color for the splash screen.")]
        [UTInspectorHint(@group = "Splash Image", order=50)]
        public UTColor splashScreenBackgroundColor;

        // ---------------- RENDERING ---------------------
        [UTDoc(description = "The color space for lightmaps.")]
        [UTInspectorHint(group = "Rendering", order = 2)]
        public UTColorSpace colorSpace;

#if !UNITY_5_3 // VR: 5.4
        [HideInInspector]
#endif
        [UTDoc(description = "Enable stereoscopic rendering?")]
        [UTInspectorHint(group = "Rendering", order = 10)]
        public UTBool stereoscopicRendering;


        // --------------- CONFIGURATION ------------------
        [UTInspectorHint(group = "Configuration", order = 0)]
        [UTDoc(description = "Which scripting backend should be used?")]
        public UTScriptingImplementation scriptingBackend;

        [UTDoc(description = "Accelerometer frequency in Hertz. Valid values are 0, 15, 30, 60 and 100.")]
        [UTInspectorHint(group = "Configuration", order = 5, allowedValues = new []{"0", "15", "30", "60", "100"})]
        public UTInt accelerometerFrequency;

        // --------------- PUBLISHING SETTINGS -------------
        [UTDoc(description = "Unique name that identifies the package on the system.")]
        [UTInspectorHint(group="Publishing Settings", order = 10)]
        public UTString packageName;

        [UTDoc(description = "The version in quad notation: major.minor.build.revision")]
        [UTInspectorHint(group="Publishing Settings", order = 20)]
        public UTString packageVersion;

        [UTDoc(description = "The certificate used to sign your application.")]
        [UTInspectorHint(group = "Publishing Settings", order = 30, displayAs = UTInspectorHint.DisplayAs.OpenFileSelect, extension = "pfx")]
        public UTString certificate;

        [UTDoc(description = "The certificate password.")]
        [UTInspectorHint(group = "Publishing Settings", order = 40, displayAs = UTInspectorHint.DisplayAs.Password)]
        public UTString certificatePassword;


        [UTDoc(description = "Text that describes the app on it's tile in Windows.")]
        [UTInspectorHint(group="Publishing Settings", order = 50)]
        public UTString applicationDescription;

        [UTDoc(title="File Type Name", description = "Name that should be used for the file type associated with the app.")]
        [UTInspectorHint(group="Publishing Settings", order = 60)]
        public UTString ftaName;

        [UTDoc(title="File Types", description = "A list of file types associated with the app.")]
        [UTInspectorHint(group="Publishing Settings", order = 70)]
        public UTFileType[] fileTypes;

        [UTDoc(description = "Name of the protocol for which the app is registered.")]
        [UTInspectorHint(group="Publishing Settings", order = 80)]
        public UTString protocol;

        [UTDoc(description = "Compilation behaviour setting.")]
        [UTInspectorHint(group="Publishing Settings", order = 90)]
        public UTWSACompilationOverrides compilationOverrides;

        [UTDoc(description = "Ticking this makes your input more responsive, and usually you want this option to be enabled.")]
        [UTInspectorHint(group = "Publishing Settings", order = 100)]
        public UTBool independentInputSource;

#if !UNITY_5_3 // VR: 5.4
        [HideInInspector]
#endif
        [UTDoc(title="Low latency presentation API", description = "Ticking this should increase input responsiveness. This option is disabled by default because on hardware with older GPU drivers, this option makes game laggy, if you enable this option - be sure to profile your game if the performance is still acceptable.")]
        [UTInspectorHint(group = "Publishing Settings", order = 110)]
        public UTBool lowLatencyPresentationApi;

        [UTDoc(description = "Capabilities of the application.")]
        [UTInspectorHint(group = "Publishing Settings", order = 120, multiSelect = true)]
        public UTWSACapability capabilities;


        [SuppressMessage("ReSharper", "CoVariantArrayConversion")]
        public override IEnumerator Execute(UTContext context)
        {
            if (UTPreferences.DebugMode)
            {
                Debug.Log("Modifying Windows Store player settings.", this);
            }

            var theFrequency = accelerometerFrequency.EvaluateIn(context);
            if (theFrequency != 0 && theFrequency != 15 && theFrequency != 30 && theFrequency != 60 && theFrequency != 100)
            {
                throw new UTFailBuildException("Invalid accelerometer frequency. Valid values for accelerometer frequencies are 0, 15, 30, 60 and 100.", this);
            }

            var thePackageName = packageName.EvaluateIn(context);
            if (!PackageNameRegex.IsMatch(thePackageName) || thePackageName.ToUpperInvariant().In("CON", "PRN", "AUX", "NUL", "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9", "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"))
            {
                throw new UTFailBuildException("Package name '" + thePackageName + "' is no valid package name.", this);
            }

            var thePackageVersion = packageVersion.EvaluateIn(context);
            if (!PackageVersionRegex.IsMatch(thePackageVersion))
            {
                throw new UTFailBuildException("Package version '" + thePackageVersion + "' is no valid package version.", this);
            }

            var theCertificatePath = certificate.EvaluateIn(context);
            if (!string.IsNullOrEmpty(theCertificatePath))
            {
                if (!UTFileUtils.IsBelow(UTFileUtils.ProjectRoot, theCertificatePath))
                {
                    throw new UTFailBuildException("The certificate must be located inside the project.", this);
                }

                var relativePath = UTFileUtils.FullPathToProjectPath(theCertificatePath);

                var fullPath = UTFileUtils.CombineToPath(UTFileUtils.ProjectRoot, relativePath);
                if (!UTFileUtils.IsFile(fullPath))
                {
                    throw new UTFailBuildException("The certificate at '" + theCertificatePath + "' does not exist.", this);
                }

                theCertificatePath = relativePath;
            }
            var theCertificatePassword = certificatePassword.EvaluateIn(context);

            using (var wrapper = new UTPlayerSettingsWrapper())
            {
                wrapper.SetBool("runInBackground", runInBackground.EvaluateIn(context));
                wrapper.SetEnum("defaultScreenOrientation", defaultOrientation.EvaluateIn(context));

                SetImages(EvaluateAll(storeLogo, context), StoreLogoMaps);

                wrapper.SetString("metroTileShortName", shortName.EvaluateIn(context));
                PlayerSettings.WSA.mediumTileShowName = showNameOnMediumTile.EvaluateIn(context);
                PlayerSettings.WSA.largeTileShowName = showNameOnLargeTile.EvaluateIn(context);
                PlayerSettings.WSA.wideTileShowName = showNameOnWideTile.EvaluateIn(context);
                PlayerSettings.WSA.tileForegroundText = foregroundText.EvaluateIn(context);
                PlayerSettings.WSA.tileBackgroundColor = backgroundColor.EvaluateIn(context);
                PlayerSettings.WSA.defaultTileSize = defaulSize.EvaluateIn(context);

                SetImages(EvaluateAll(windowsSmallLogo, context), WindowsSmallLogoMaps);
                SetImages(EvaluateAll(windowsMediumTile, context), WindowsMediumTileMaps);
                SetImages(EvaluateAll(windowsWideTile, context), WindowsWideTileMaps);
                SetImages(EvaluateAll(windowsSmallTile, context), WindowsSmallTileMaps);
                SetImages(EvaluateAll(windowsLargeTile, context), WindowsLargeTileMaps);

                SetImages(EvaluateAll(windowsPhoneApplicationIcon, context), WindowsPhoneApplicationIconMaps);
                SetImages(EvaluateAll(windowsPhoneSmallTile, context), WindowsPhoneSmallTileMaps);
                SetImages(EvaluateAll(windowsPhoneMediumTile, context), WindowsPhoneMediumTileMaps);
                SetImages(EvaluateAll(windowsPhoneWideTile, context), WindowsPhoneWideTileMaps);

                SetImages(EvaluateAll(universal10Square44Logo, context), Universal10Square44LogoMaps);
                SetImages(EvaluateAll(universal10Square71Logo, context), Universal10Square71LogoMaps);
                SetImages(EvaluateAll(universal10Square150Logo, context), Universal10Square150LogoMaps);
                SetImages(EvaluateAll(universal10Square310Logo, context), Universal10Square310LogoMaps);
                SetImages(EvaluateAll(universal10Wide310Logo, context), Universal10Wide310LogoMaps);

                SetImages(EvaluateAll(windowsSplashImage, context), WindowsSplashImageMaps);
                SetImages(EvaluateAll(windowsPhoneSplashImage, context), WindowsPhoneSplashImageMaps);

                var doOverwriteBackgroundColor = overwriteBackgroundColor.EvaluateIn(context);
                PlayerSettings.WSA.splashScreenBackgroundColor = doOverwriteBackgroundColor ? splashScreenBackgroundColor.EvaluateIn(context) : (Color?) null;

                PlayerSettings.colorSpace = colorSpace.EvaluateIn(context);
#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 // VR: [5.0, 5.3]
                PlayerSettings.stereoscopic3D = stereoscopicRendering.EvaluateIn(context);
#endif
                PlayerSettings.SetPropertyInt("ScriptingBackend", (int)scriptingBackend.EvaluateIn(context), BuildTargetGroup.WSA);

                wrapper.SetInt("accelerometerFrequency", theFrequency);

                wrapper.SetString("metroPackageName", thePackageName);
                wrapper.SetString("metroPackageVersion", thePackageVersion);

                if (!PlayerSettings.WSA.SetCertificate(theCertificatePath, theCertificatePassword))
                {
                    throw new UTFailBuildException("The certificate password is incorrect.", this);
                }
                wrapper.SetString("metroApplicationDescription", applicationDescription.EvaluateIn(context));
                wrapper.SetString("metroFTAName", ftaName.EvaluateIn(context));
                wrapper.DoWith("metroFTAFileTypes", property =>
                {
                    var theFileTypes = EvaluateAll(fileTypes, context);
                    // overwrite existing elements / append if required
                    for (var i = 0; i < theFileTypes.Length; i++)
                    {
                        var theFileType = theFileTypes[i];
                        if (i >= property.arraySize)
                        {
                            property.InsertArrayElementAtIndex(i);
                        }

                        var serializedElement = property.GetArrayElementAtIndex(i);
                        serializedElement.FindPropertyRelative("contentType").stringValue = theFileType.contentType;
                        serializedElement.FindPropertyRelative("fileType").stringValue = theFileType.fileType;
                    }

                    // kill any remaining elements
                    while (property.arraySize > theFileTypes.Length)
                    {
                        property.DeleteArrayElementAtIndex(property.arraySize-1);
                    }
                });
                wrapper.SetString("metroProtocolName", protocol.EvaluateIn(context));
                PlayerSettings.WSA.compilationOverrides = compilationOverrides.EvaluateIn(context);
                wrapper.SetBool("metroEnableIndependentInputSource", independentInputSource.EvaluateIn(context));

#if UNITY_5_3 // VR: [5.3,5.3]
                wrapper.SetBool("metroEnableLowLatencyPresentationAPI", lowLatencyPresentationApi.EvaluateIn(context));
#endif

                var theCapabilities = capabilities.EvaluateIn(context);

                var values = Enum.GetValues(typeof (WSACapability));
                foreach (var value in values)
                {
                    // ReSharper disable once AssignNullToNotNullAttribute
                    var realEnumEntry = (UnityWsaCapability) Enum.Parse(typeof(UnityWsaCapability), Enum.GetName(typeof(WSACapability), value));
                    PlayerSettings.WSA.SetCapability(realEnumEntry, (theCapabilities & (WSACapability)value) == (WSACapability)value);
                }

                ApplyCommonSettings(wrapper, context);
            }


            if (UTPreferences.DebugMode)
            {
                Debug.Log("Windows Store player settings modified.", this);
            }

            yield return "";
        }


        /// <summary>
        /// Loads current player settings.
        /// </summary>
        public void LoadSettings()
        {
            var wrapper = new UTPlayerSettingsWrapper();

            runInBackground.StaticValue = wrapper.GetBool("runInBackground");
            defaultOrientation.StaticValue = wrapper.GetEnum<UIOrientation>("defaultScreenOrientation");

            GetImages(storeLogo, StoreLogoMaps);

            shortName.StaticValue = wrapper.GetString("metroTileShortName");
            showNameOnMediumTile.StaticValue = PlayerSettings.WSA.mediumTileShowName;
            showNameOnLargeTile.StaticValue = PlayerSettings.WSA.largeTileShowName;
            showNameOnWideTile.StaticValue = PlayerSettings.WSA.wideTileShowName;
            foregroundText.StaticValue = PlayerSettings.WSA.tileForegroundText;
            backgroundColor.StaticValue = PlayerSettings.WSA.tileBackgroundColor;
            defaulSize.StaticValue = PlayerSettings.WSA.defaultTileSize;

            GetImages(windowsSmallLogo, WindowsSmallLogoMaps);
            GetImages(windowsMediumTile, WindowsMediumTileMaps);
            GetImages(windowsWideTile, WindowsWideTileMaps);
            GetImages(windowsSmallTile, WindowsSmallTileMaps);
            GetImages(windowsLargeTile, WindowsLargeTileMaps);

            GetImages(windowsPhoneApplicationIcon, WindowsPhoneApplicationIconMaps);
            GetImages(windowsPhoneSmallTile, WindowsPhoneSmallTileMaps);
            GetImages(windowsPhoneMediumTile, WindowsPhoneMediumTileMaps);
            GetImages(windowsPhoneWideTile, WindowsPhoneWideTileMaps);

            GetImages(universal10Square44Logo, Universal10Square44LogoMaps);
            GetImages(universal10Square71Logo, Universal10Square71LogoMaps);
            GetImages(universal10Square150Logo, Universal10Square150LogoMaps);
            GetImages(universal10Square310Logo, Universal10Square310LogoMaps);
            GetImages(universal10Wide310Logo, Universal10Wide310LogoMaps);

            GetImages(windowsSplashImage, WindowsSplashImageMaps);
            GetImages(windowsPhoneSplashImage, WindowsPhoneSplashImageMaps);

            overwriteBackgroundColor.StaticValue = PlayerSettings.WSA.splashScreenBackgroundColor != null;
            splashScreenBackgroundColor.StaticValue = PlayerSettings.WSA.splashScreenBackgroundColor ?? Color.black;

            colorSpace.StaticValue = PlayerSettings.colorSpace;
                
#if UNITY_5_3 // VR: [5.3, 5.3]           
            stereoscopicRendering.StaticValue = PlayerSettings.stereoscopic3D;
#endif
            scriptingBackend.StaticValue = (ScriptingImplementation) PlayerSettings.GetPropertyInt("ScriptingBackend", BuildTargetGroup.WSA);

            accelerometerFrequency.StaticValue = wrapper.GetInt("accelerometerFrequency");

            packageName.StaticValue = wrapper.GetString("metroPackageName");
            packageVersion.StaticValue = wrapper.GetString("metroPackageVersion");

            certificate.StaticValue = PlayerSettings.WSA.certificatePath;
            applicationDescription.StaticValue = wrapper.GetString("metroApplicationDescription");
            ftaName.StaticValue = wrapper.GetString("metroFTAName");
            wrapper.DoWith("metroFTAFileTypes", property =>
            {
                fileTypes = new UTFileType[property.arraySize];

                for (var i = 0; i < property.arraySize; i++)
                {
                    var serializedElement = property.GetArrayElementAtIndex(i);
                    var fileType = new FileType
                    {
                        contentType = serializedElement.FindPropertyRelative("contentType").stringValue,
                        fileType = serializedElement.FindPropertyRelative("fileType").stringValue
                    };
                    fileTypes[i] = new UTFileType {StaticValue = fileType};
                }
            });
            protocol.StaticValue = wrapper.GetString("metroProtocolName");
            compilationOverrides.StaticValue = PlayerSettings.WSA.compilationOverrides;
            independentInputSource.StaticValue = wrapper.GetBool("metroEnableIndependentInputSource");

#if UNITY_5_3 // VR: [5.3, 5.3]
            lowLatencyPresentationApi.StaticValue = wrapper.GetBool("metroEnableLowLatencyPresentationAPI");
#endif

            var values = Enum.GetValues(typeof(WSACapability));
            var capabilitiesValue = 0;
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var value in values)
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                var realEnumEntry = (UnityWsaCapability)Enum.Parse(typeof(UnityWsaCapability), Enum.GetName(typeof(WSACapability), value));
                if (PlayerSettings.WSA.GetCapability(realEnumEntry))
                {
                    capabilitiesValue |= (int) value;
                }
            }

            capabilities.StaticValue = (WSACapability) capabilitiesValue;
            LoadCommonSettings(wrapper);

            // we cannot load the password, so we keep it as it is and notify the user that it might be wrong.
            Debug.LogWarning("Settings successfully loaded. Please verify if the certificate password is still correct.", this);

        }

        [MenuItem("Assets/Create/uTomate/Build/Set Windows Store Player Settings", false, 240)]
        public static void AddAction()
        {
            var result = Create<UTSetPlayerSettingsWindowsStoreAction>();

            result.storeLogo = MkArray(StoreLogoMaps.Length);
            result.windowsSmallLogo = MkArray(WindowsSmallLogoMaps.Length);
            result.windowsMediumTile = MkArray(WindowsMediumTileMaps.Length);
            result.windowsWideTile = MkArray(WindowsWideTileMaps.Length);
            result.windowsSmallTile = MkArray(WindowsSmallTileMaps.Length);
            result.windowsLargeTile = MkArray(WindowsLargeTileMaps.Length);
            result.windowsPhoneApplicationIcon =MkArray(WindowsPhoneApplicationIconMaps.Length);
            result.windowsPhoneSmallTile = MkArray(WindowsPhoneSmallTileMaps.Length);
            result.windowsPhoneMediumTile = MkArray(WindowsPhoneMediumTileMaps.Length);
            result.windowsPhoneWideTile = MkArray(WindowsPhoneMediumTileMaps.Length);
            result.universal10Square44Logo = MkArray(Universal10Square44LogoMaps.Length);
            result.universal10Square71Logo = MkArray(Universal10Square71LogoMaps.Length);
            result.universal10Square150Logo = MkArray(Universal10Square150LogoMaps.Length);
            result.universal10Square310Logo = MkArray(Universal10Square310LogoMaps.Length);
            result.universal10Wide310Logo = MkArray(Universal10Wide310LogoMaps.Length);
            result.windowsPhoneSplashImage = MkArray(WindowsPhoneSplashImageMaps.Length);
            result.windowsSplashImage = MkArray(WindowsSplashImageMaps.Length);


            result.LoadSettings();
        }

        private static UTTexture2D[] MkArray(int length)
        {
            var array = new UTTexture2D[length];

            for (var i = 0; i < array.Length; i++)
            {
                    array[i] = new UTTexture2D();
            }
            return array;
        }


        public string LoadSettingsUndoText
        {
            get { return "Load Windows Store player settings"; }
        }
        
        protected override bool IsMobilePlatform
        {
            get
            {
                return false;
            }
        }

        protected override BuildTarget Platform
        {
            get
            {
                return BuildTarget.WSAPlayer;
            }
        }

        // Image madness

        private void SetImages(IList<Texture2D> input, IList<ImageMap> maps)
        {
            if (input.Count != maps.Count)
            {
                throw new ArgumentException("Mapping does not fit to the length of input. This is a bug in uTomate, please report it to support@ancientlightstudios.com.");
            }
            for (var i = 0; i < input.Count; i++)
            {
                var texture = input[i];
                var mapping = maps[i];
                var path = texture != null ? AssetDatabase.GetAssetPath(texture) : null;
                VerifyImageConstraints(path, mapping);

                PlayerSettings.WSA.SetVisualAssetsImage(path, mapping.ImageType, mapping.ImageScale);
            }
        }

        /// <summary>
        /// We can basically set every conceivable image size without checking in the player settings however we want to be close to what the editor
        /// does so we need to verify that the image size constraints are met.
        /// </summary>
        /// <param name="path">the path to the texture</param>
        /// <param name="mapping">the image mapping describing the expected image</param>
        private void VerifyImageConstraints(string path, ImageMap mapping)
        {
            if (string.IsNullOrEmpty(path))
            {
                return; // no image is ok
            }

            // we load the texture explicitely here, because if you use the already imported texture from the unity editor
            // the width/height of the textures get rounded to the next potence of 2.
            var texture2D = new Texture2D(1, 1);
            texture2D.LoadImage(File.ReadAllBytes(path));
            var actualWidth = texture2D.width;
            var actualHeight = texture2D.height;
            DestroyImmediate(texture2D);

            if (actualWidth != mapping.Width || actualHeight != mapping.Height)
            {
                throw new UTFailBuildException(
                    string.Format("Invalid image size ({0}x{1}), should be {2}x{3} for image of type {4} ({5})",actualWidth, actualHeight, mapping.Width, mapping.Height, mapping.ImageType, path), this);
            }
        }

        private static void GetImages(IList<UTTexture2D> output, IList<ImageMap> maps)
        {
            if (output.Count != maps.Count)
            {
                throw new ArgumentException("Mapping does not fit to the length of input. This is a bug in uTomate, please report it to support@ancientlightstudios.com.");
            }

            for (var i = 0; i < output.Count; i++)
            {
                var mapping = maps[i];

                var path = PlayerSettings.WSA.GetVisualAssetsImage(mapping.ImageType, mapping.ImageScale);
                var asset = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                output[i].StaticValue = asset;
            }
        }


        public static ImageMap[] MakeImageMap(int baseWidth, int baseHeight, PlayerSettings.WSAImageType type, params PlayerSettings.WSAImageScale[] scales)
        {
            var result = new ImageMap[scales.Length];
            for (var i = 0; i < scales.Length; i++)
            {
                // this is a hack but seriously, the enum values are unlikely to change (famous last words, probably...)
                int width;
                int height;

                var scale = scales[i];
                // ReSharper disable once PossibleNullReferenceException
                if (Enum.GetName(typeof(PlayerSettings.WSAImageScale), scale).StartsWith("Target"))
                {
                    width = (int) scale;
                    height = (int) scale;
                }
                else
                {
                    var scaleFactor = ((int)scales[i])/100f;
                    width = (int) Math.Round(scaleFactor*baseWidth, MidpointRounding.AwayFromZero);
                    height = (int) Math.Round(scaleFactor*baseHeight, MidpointRounding.AwayFromZero);
                }
                result[i] = new ImageMap(type, scale, width , height );
            }
            return result;
        }

        public class ImageMap
        {
            public int Width { get; private set; }
            public int Height { get; private set; }
            public PlayerSettings.WSAImageType ImageType { get; private set; }
            public PlayerSettings.WSAImageScale ImageScale { get; private set; }

            public ImageMap(PlayerSettings.WSAImageType imageType, PlayerSettings.WSAImageScale imageScale, int width, int height)
            {
                ImageType = imageType;
                ImageScale = imageScale;
                Width = width;
                Height = height;
            }
        }

        [Serializable]
        public struct FileType
        {
            public string contentType;
            public string fileType;

        }

        [Flags]
        // ReSharper disable once InconsistentNaming
        public enum WSACapability
        {
            EnterpriseAuthentication = 1 << 0,
            InternetClient = 1 << 1,
            InternetClientServer = 1 << 2,
            MusicLibrary = 1 << 3,
            PicturesLibrary = 1 << 4,
            PrivateNetworkClientServer = 1 << 5,
            RemovableStorage = 1 << 6,
            SharedUserCertificates = 1 << 7,
            VideosLibrary = 1 << 8,
            WebCam = 1 << 9,
            Proximity = 1 << 10,
            Microphone = 1 << 11,
            Location = 1 << 12,
            HumanInterfaceDevice = 1 << 13,
            AllJoyn = 1 << 14,
            BlockedChatMessages = 1 << 15,
            Chat = 1 << 16,
            CodeGeneration = 1 << 17,
            Objects3D = 1 << 18,
            PhoneCall = 1 << 19,
            UserAccountInformation = 1 << 20,
            VoipCall = 1 << 21,
            Bluetooth = 1 << 22,
        }

        public override bool HasAutorotation
        {
            get { return true; }
        }
    }
}

#endif
