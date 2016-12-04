//
// Copyright (c) 2013-2016 Ancient Light Studios
// All Rights Reserved
// 
// http://www.ancientlightstudios.com
//

namespace AncientLightStudios.uTomate
{
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEditor;

    public class UTMemberListResult
    {
        private string[] nicifiedMemberNames;
        private MemberInfo[] memberInfos;

        public UTMemberListResult(IEnumerable<MemberInfo> memberInfos)
        {
            SortedDictionary<string, MemberInfo> sortedNames = new SortedDictionary<string, MemberInfo>();

            foreach (var memberInfo in memberInfos)
            {
                sortedNames.Add(ObjectNames.NicifyVariableName(memberInfo.Name), memberInfo);
            }

            nicifiedMemberNames = new string[sortedNames.Count];
            this.memberInfos = new MemberInfo[sortedNames.Count];

            var i = 0;
            foreach (var key in sortedNames.Keys)
            {
                nicifiedMemberNames[i] = key;
                this.memberInfos[i] = sortedNames[key];
                i++;
            }
        }

        public MemberInfo[] MemberInfos
        {
            get
            {
                return memberInfos;
            }
        }

        public string[] NicifiedMemberNames
        {
            get
            {
                return nicifiedMemberNames;
            }
        }
    }
}
