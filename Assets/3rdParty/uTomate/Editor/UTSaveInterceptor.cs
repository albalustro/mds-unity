//
// Copyright (c) 2013-2016 Ancient Light Studios
// All Rights Reserved
// 
// http://www.ancientlightstudios.com
//

namespace AncientLightStudios.uTomate
{
    /// <summary>
    /// Interceptor which cleans up unused entries in automation plans.
    /// </summary>
    public class UTSaveInterceptor : UnityEditor.AssetModificationProcessor  // moved in Unity4
    {
        public static string[] OnWillSaveAssets(string[] paths)
        {
            foreach (var asset in paths)
            {
                UTils.ClearUnusedEntriesIn(asset);
            }
            return paths;
        }
    }
}
