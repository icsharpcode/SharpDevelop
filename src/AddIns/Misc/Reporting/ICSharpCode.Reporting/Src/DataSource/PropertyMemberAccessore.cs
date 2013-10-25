// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Reflection;

namespace ICSharpCode.Reporting.DataSource
{
	/// <summary>
	/// Description of PropertyMemberAccessore.
	/// </summary>
	public interface  IMemberAccessor
    {
        object GetValue(object target);
        bool IsStatic { get; }
        Type MemberType { get; }
    }
	
	public class PropertyMemberAccessor : IMemberAccessor
    {
        private readonly PropertyInfo _prop;

        public PropertyMemberAccessor(PropertyInfo prop)
        {
            _prop = prop;
        }

        public object GetValue(object target)
        {
            return _prop.GetValue(target, null);
        }

        public bool IsStatic
        {
            get { return _prop.GetGetMethod().IsStatic; }
        }

        public Type MemberType
        {
            get { return _prop.PropertyType;}
        }
    }
}
