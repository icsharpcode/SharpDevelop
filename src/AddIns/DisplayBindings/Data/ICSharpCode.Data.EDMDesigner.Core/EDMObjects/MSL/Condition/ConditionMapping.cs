// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.MSL.Condition
{
    public abstract class ConditionMapping
    {
        private ConditionOperator _operator;
        public ConditionOperator Operator
        {
            get { return _operator; }
            set
            {
                if (value != ConditionOperator.Equals)
                    _value = null;
                _operator = value;
            }
        }
        private string _value;
        public string Value
        {
            get { return _value; }
            set
            {
                if (Operator != ConditionOperator.Equals)
                    return;
                _value = value;
            }
        }

        public SSDL.EntityType.EntityType Table { get; set; }
    }
}
