//
// Copyright (c) 2013-2016 Ancient Light Studios
// All Rights Reserved
// 
// http://www.ancientlightstudios.com
//

namespace AncientLightStudios.uTomate
{
    using API;

    /// <summary>
    ///  Script extension for quer
    /// </summary>
    [UTScriptExtension("ut:context")]
    public class UTContextScriptExtension : UTIContextAware
    {
        public UTContext Context
        {
            set;
            get;
        }

        /// <summary>
        /// Determines whether a property with the given name exists in the context.
        /// </summary>
        public bool IsSet(string name)
        {
            return Context.ContainsProperty(name);
        }

        /// <summary>
        /// Determines if all given properties exist in the context.
        /// </summary>
        public bool AreAllSet(params string[] names)
        {
            foreach (var prop in names)
            {
                if (!IsSet(prop))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
