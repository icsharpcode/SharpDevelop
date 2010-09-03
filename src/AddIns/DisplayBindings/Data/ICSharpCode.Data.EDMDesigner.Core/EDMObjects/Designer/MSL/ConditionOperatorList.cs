// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.MSL
{
    public class ConditionOperatorList : List<ConditionOperatorItem>
    {
        private static ConditionOperatorList _instance;
        /// <summary>
        /// It isn't a thread safe singleton but it isn't a problem here
        /// </summary>
        public static ConditionOperatorList Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ConditionOperatorList() 
                    { 
                        new ConditionOperatorItem { Value = ICSharpCode.Data.EDMDesigner.Core.EDMObjects.MSL.Condition.ConditionOperator.IsNull, Text = "== null" }, 
                        new ConditionOperatorItem { Value = ICSharpCode.Data.EDMDesigner.Core.EDMObjects.MSL.Condition.ConditionOperator.IsNotNull, Text = "!= null" }, 
                        new ConditionOperatorItem { Value = ICSharpCode.Data.EDMDesigner.Core.EDMObjects.MSL.Condition.ConditionOperator.Equals, Text="==" }
                    };
                return _instance;
            }
        }
    }
}
