// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class Property : MarshalByRefObject, global::EnvDTE.Property
	{
		public Property(string name)
		{
			this.Name = name;
		}
		
		public Property()
		{
		}
		
		public virtual string Name { get; private set; }
		
		public virtual object Value {
			get { return GetValue(); }
			set { SetValue(value); }
		}
		
		protected virtual object GetValue()
		{
			return null;
		}
		
		protected virtual void SetValue(object value)
		{
		}
		
		public virtual object Object {
			get { return GetObject(); }
			set { }
		}
		
		protected virtual object GetObject()
		{
			return null;
		}
	}
}
