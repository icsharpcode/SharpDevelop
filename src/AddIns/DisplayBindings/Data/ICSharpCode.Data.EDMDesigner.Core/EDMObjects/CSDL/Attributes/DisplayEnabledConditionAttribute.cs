#region Usings

using System;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Attributes
{
    public class DisplayEnabledConditionAttribute : Attribute
    {
        public DisplayEnabledConditionAttribute(string conditionPropertyName)
        {
            ConditionPropertyName = conditionPropertyName;
        }

        public string ConditionPropertyName { get; private set; }
    }
}
