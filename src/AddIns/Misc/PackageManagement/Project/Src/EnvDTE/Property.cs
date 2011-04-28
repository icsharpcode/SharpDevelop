// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class Property
	{
		public Property(Project project, string name)
		{
			this.Project = project;
			this.Name = name;
		}
		
		public string Name { get; private set; }
		
		protected Project Project { get; private set; }
		
		public virtual object Value {
			get { return GetProperty(); }
			set {
				SetProperty(value);
				Project.Save();
			}
		}
		
		protected virtual object GetProperty()
		{
			return null;
		}
		
		protected virtual void SetProperty(object value)
		{
		}
	}
}
