// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace PackageManagement.Tests.Helpers
{
	public class ReferenceAndProjectName
	{
		public ReferenceAndProjectName()
		{
		}
		
		public ReferenceAndProjectName(string reference, string project)
		{
			this.Reference = reference;
			this.Project = project;
		}
		
		public string Reference;
		public string Project;
		
		public override bool Equals(object obj)
		{
			ReferenceAndProjectName rhs = obj as ReferenceAndProjectName;
			if (rhs != null) {
				return (rhs.Project == Project) && 
					(rhs.Reference == Reference);
			}
			return false;
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		
		public override string ToString()
		{
			return String.Format("[ReferenceAndProjectName Reference={0}, Project={1}]", Reference, Project);
		}
	}
}
