//
// Copyright (c) 2013-2016 Ancient Light Studios
// All Rights Reserved
// 
// http://www.ancientlightstudios.com
//

namespace AncientLightStudios.uTomate.API
{
    using System;
    using UnityEngine;

    /// <summary>
    ///  Wrapper around the <code>Color</code> data type.
    /// </summary>
    [Serializable]
    public class UTColor : UTProperty<Color>
    {
        [SerializeField]
        private Color propertyValue = Color.blue;

        public override Color Value
        {
            get
            {
                return propertyValue;
            }
            set
            {
                propertyValue = value;
            }
        }
    }
}
