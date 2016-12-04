//
// Copyright (c) 2013-2016 Ancient Light Studios
// All Rights Reserved
// 
// http://www.ancientlightstudios.com
//

namespace AncientLightStudios.uTomate
{
    using UnityEditor;
    using UnityEngine;

    public class UTPrefabFilter : UTFilter
    {
        private GameObject[] prefabs;

        public UTPrefabFilter(GameObject[] prefabs)
        {
            this.prefabs = prefabs;
        }

        public bool Accept(object o)
        {
            GameObject go = o as GameObject;
            if (go == null)
            {
                return false;
            }

            if (prefabs == null || prefabs.Length == 0)
            {
                return false;
            }

            var parent = PrefabUtility.GetPrefabParent(go);
            foreach (var prefab in prefabs)
            {
                if (parent == prefab)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
