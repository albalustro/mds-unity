//
// Copyright (c) 2013-2016 Ancient Light Studios
// All Rights Reserved
// 
// http://www.ancientlightstudios.com
//

namespace AncientLightStudios.uTomate
{
    using System;
    using UnityEngine;

    [Serializable]
    public class UTTypeInfo
    {
        [SerializeField]
        private string typeName;

        [NonSerialized]
        private Type theType;

        [NonSerialized]
        private bool lookupDone;

        public UTTypeInfo()
        {
        }

        public UTTypeInfo(string typeName)
        {
            this.typeName = typeName;
        }

        public string TypeName
        {
            get
            {
                return typeName;
            }
        }

        public Type Type
        {
            get
            {
                if (!lookupDone)
                {
                    lookupDone = true;
                    theType = UTInternalCall.GetType(typeName);
                }
                return theType;
            }
        }
    }
}
