using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class MDSTexturePreProcessor : AssetPostprocessor
{

    Texture2D asset;
    TextureImporter i;
    TextureImporterPlatformSettings windowsIS;
    TextureImporterPlatformSettings defaultIS;
    TextureImporterPlatformSettings androidImporterSettings;
    TextureImporterPlatformSettings webglImporterSettings;

    void OnPreprocessTexture()
    {
        #region Common
        i = (TextureImporter)assetImporter;

        if(i.spriteImportMode == SpriteImportMode.Multiple)
        {
            Debug.LogWarningFormat("Skipping {0}", i.assetPath);
            return;
        }

        i.textureType = TextureImporterType.Sprite;
        i.spriteImportMode = SpriteImportMode.Single;
        i.mipmapEnabled = false;


//#if UNITY_ANDROID
//        i.spritePackingTag = Path.GetDirectoryName(i.assetPath).Replace("/","_");
//#elif UNITY_WEBGL
//        i.spritePackingTag = string.Empty;
//#endif

        i.spritePackingTag = string.Empty;

        i.spritePixelsPerUnit = 100f;
        i.alphaIsTransparency = true;
        i.maxTextureSize = 2048;
        i.alphaSource = TextureImporterAlphaSource.FromInput;
        i.filterMode = FilterMode.Bilinear;
        i.anisoLevel = 0;
        //i.wrapMode = TextureWrapMode.Repeat;

        asset = AssetDatabase.LoadAssetAtPath<Texture2D>(i.assetPath);
        if(asset == null)
        {
            i.SaveAndReimport();
            return;
        }
        bool canCrunch = asset.width % 4 == 0 && asset.height % 4 == 0;
        Resources.UnloadAsset(asset);

        #endregion

        #region Default

        defaultIS = new TextureImporterPlatformSettings();
        defaultIS.name = "Default";
        defaultIS.maxTextureSize = 2048;
        defaultIS.format = TextureImporterFormat.Automatic;
        defaultIS.compressionQuality = 100;
        
        #endregion

        #region Android

        androidImporterSettings = new TextureImporterPlatformSettings();
        androidImporterSettings.name = "Android";
        androidImporterSettings.overridden = true;
        androidImporterSettings.maxTextureSize = 2048;

        //if(i.DoesSourceTextureHaveAlpha())
        //    androidImporterSettings.format = TextureImporterFormat.ETC2_RGBA8;
        //else
        //    androidImporterSettings.format = TextureImporterFormat.ETC2_RGBA8;
        androidImporterSettings.format = TextureImporterFormat.RGBA32;

        androidImporterSettings.textureCompression = TextureImporterCompression.Uncompressed ;
        androidImporterSettings.compressionQuality = 100;

        #endregion

        #region Webgl

        webglImporterSettings = new TextureImporterPlatformSettings();
        webglImporterSettings.name = "WebGL";
        webglImporterSettings.overridden = true;
        webglImporterSettings.maxTextureSize = 2048;
        if(canCrunch)
        {
            if(i.DoesSourceTextureHaveAlpha())
                webglImporterSettings.format = TextureImporterFormat.DXT5Crunched;
            else
                webglImporterSettings.format = TextureImporterFormat.DXT1Crunched;
        }
        else
        {
            if(i.DoesSourceTextureHaveAlpha())
                webglImporterSettings.format = TextureImporterFormat.ARGB16;
            else
                webglImporterSettings.format = TextureImporterFormat.RGB16;
        }

        //webglImporterSettings.format = TextureImporterFormat.RGBA32;
        webglImporterSettings.compressionQuality = 50;
        //webglImporterSettings.crunchedCompression = true;
        webglImporterSettings.textureCompression = TextureImporterCompression.Compressed;

        #endregion


        #region Windows

        windowsIS = new TextureImporterPlatformSettings();
        windowsIS.name = "Standalone";
        windowsIS.overridden = true;
        windowsIS.format = TextureImporterFormat.RGBA32;
        windowsIS.textureCompression = TextureImporterCompression.Compressed;
        windowsIS.compressionQuality = 100;

        #endregion

        i.SetPlatformTextureSettings(defaultIS);
        i.SetPlatformTextureSettings(windowsIS);
        i.SetPlatformTextureSettings(androidImporterSettings);
        i.SetPlatformTextureSettings(webglImporterSettings);


    }



}
