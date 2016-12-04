//
// Copyright (c) 2013-2016 Ancient Light Studios
// All Rights Reserved
// 
// http://www.ancientlightstudios.com
//

namespace AncientLightStudios.uTomate
{
    using UnityEngine;

    public class UTPositionFilter : UTFilter
    {
        private Vector3 position;
        private float distance;

        public UTPositionFilter(Vector3 position, float distance)
        {
            this.position = position;
            this.distance = distance;
        }

        public bool Accept(object o)
        {
            GameObject go = o as GameObject;
            if (go == null)
            {
                return false;
            }

            return (go.transform.position - position).magnitude <= distance;
        }
    }
}
