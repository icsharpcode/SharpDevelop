// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Attributes
{
    public class DisplayVisibleConditionAttribute : Attribute
    {
        public DisplayVisibleConditionAttribute(string conditionPropertyName)
        {
            ConditionPropertyName = conditionPropertyName;
        }

        public string ConditionPropertyName { get; private set; }
    }
}
