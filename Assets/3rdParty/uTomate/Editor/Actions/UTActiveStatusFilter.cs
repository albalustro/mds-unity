//
// Copyright (c) 2013-2016 Ancient Light Studios
// All Rights Reserved
// 
// http://www.ancientlightstudios.com
//

namespace AncientLightStudios.uTomate
{
    using UnityEngine;

    public class UTActiveStatusFilter : UTFilter
    {
        private bool activeStatus;

        public UTActiveStatusFilter(bool activeStatus)
        {
            this.activeStatus = activeStatus;
        }

        public bool Accept(object o)
        {
            GameObject go = o as GameObject;
            if (go == null)
            {
                return false;
            }

            if (go.activeSelf == activeStatus)
            {
                return true;
            }

            return false;
        }
    }
}
