// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
