using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class MDSTexturePreProcessor : AssetPostprocessor
{

    Texture2D asset;
    TextureImporter i;
    TextureImporterPlatformSettings defaultIS;
    TextureImporterPlatformSettings androidImporterSettings;
    TextureImporterPlatformSettings webglImporterSettings;

    void OnPreprocessTexture()
    {
		return;

        i = (TextureImporter)assetImporter;
        i.textureType = TextureImporterType.Sprite;
        i.spriteImportMode = SpriteImportMode.Single;
        i.spritePackingTag = Path.GetDirectoryName(i.assetPath).Replace("/","_");
        i.spritePixelsPerUnit = 100f;
        i.alphaIsTransparency = true;
        i.maxTextureSize = 2048;
        i.alphaSource = TextureImporterAlphaSource.FromInput;
        i.filterMode = FilterMode.Bilinear;
        i.anisoLevel = 0;

        //asset = AssetDatabase.LoadAssetAtPath<Texture2D>(i.assetPath);
        //if(asset == null)
        //{
        //    i.SaveAndReimport();
        //    return;
        //}
        //bool canCrunch = asset.width % 4 == 0 && asset.height % 4 == 0;
        //Resources.UnloadAsset(asset);

        defaultIS = new TextureImporterPlatformSettings();
        defaultIS.name = "Default";
        defaultIS.maxTextureSize = 2048;
        defaultIS.format = TextureImporterFormat.RGBA16;
        defaultIS.compressionQuality = 50;


        androidImporterSettings = new TextureImporterPlatformSettings();
        androidImporterSettings.name = "Android";
        androidImporterSettings.overridden = true;
        androidImporterSettings.maxTextureSize = 2048;

        if (i.DoesSourceTextureHaveAlpha())
            androidImporterSettings.format =TextureImporterFormat.ETC2_RGBA8;
        else
            androidImporterSettings.format = TextureImporterFormat.ETC2_RGBA8;

        androidImporterSettings.compressionQuality = 50;


        webglImporterSettings = new TextureImporterPlatformSettings();
        webglImporterSettings.name = "WebGL";
        webglImporterSettings.overridden = true;
        webglImporterSettings.maxTextureSize = 2048;
        //if(canCrunch)
        //{
        //    if(i.DoesSourceTextureHaveAlpha())
                webglImporterSettings.format = TextureImporterFormat.DXT5Crunched;
        //    else
        //        webglImporterSettings.format = TextureImporterFormat.DXT1Crunched;
        //}
        //else
        //{
        //    if(i.DoesSourceTextureHaveAlpha())
        //        webglImporterSettings.format = TextureImporterFormat.ARGB16;
        //    else
        //        webglImporterSettings.format = TextureImporterFormat.RGB16;
        //}
        webglImporterSettings.compressionQuality = 50;
        webglImporterSettings.crunchedCompression = true;
        webglImporterSettings.textureCompression = TextureImporterCompression.Compressed;

        i.SetPlatformTextureSettings(defaultIS);
        i.SetPlatformTextureSettings(androidImporterSettings);
        i.SetPlatformTextureSettings(webglImporterSettings);


    }



}
