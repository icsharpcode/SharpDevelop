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
