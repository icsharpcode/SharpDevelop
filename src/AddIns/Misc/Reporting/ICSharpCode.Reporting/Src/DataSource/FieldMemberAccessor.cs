// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Reflection;

namespace ICSharpCode.Reporting.DataSource
{
	/// <summary>
	/// Description of FieldMemberAccessor.
	/// </summary>
	 public class FieldMemberAccessor : IMemberAccessor
    {
        private readonly FieldInfo _field;

        public FieldMemberAccessor(FieldInfo field)
        {
            _field = field;
        }

        public object GetValue(object target)
        {
            return _field.GetValue(target);
        }

        public bool IsStatic
        {
            get { return _field.IsStatic; }
        }

        public Type MemberType
        {
            get { return _field.FieldType;}
        }
    }
}
