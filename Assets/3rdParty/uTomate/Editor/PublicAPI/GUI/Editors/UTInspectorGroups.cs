//
// Copyright (c) 2013-2016 Ancient Light Studios
// All Rights Reserved
// 
// http://www.ancientlightstudios.com
//

namespace AncientLightStudios.uTomate.API
{
    using System;

    /// <summary>
    /// Annotation which allows ordering of inspector groups. Must be placed a the class level.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class UTInspectorGroups : Attribute
    {
        /// <summary>
        /// The names of the groups in the order in which they should appear in the inspector.
        /// </summary>
        public string[] groups = { };

        private static readonly UTInspectorGroups Default = new UTInspectorGroups();

        /// <summary>
        /// Gets the UTInspectorGroups attribute attached to the given type. If no such attribute is attached to the given
        /// type, a default UTInspectorGroups object will be returned.
        /// </summary>
        /// <returns>
        /// The UTInspectorGroups attribute.
        /// </returns>
        /// <param name='type'>
        /// The type to read the attribute from.
        /// </param>
        public static UTInspectorGroups GetFor(Type type)
        {
            var groups = type.GetCustomAttributes(typeof(UTInspectorGroups), true);

            if (groups.Length <= 0)
            {
                return Default;
            }

            var hint = (UTInspectorGroups)groups[0];
            return hint;
        }
    }
}
