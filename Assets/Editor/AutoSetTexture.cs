using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AutoSetTexture : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        //自动设置类型;
        VideoClipImporter clipImporter = (VideoClipImporter)assetImporter;
        clipImporter.keepAlpha = true;
    }
}
