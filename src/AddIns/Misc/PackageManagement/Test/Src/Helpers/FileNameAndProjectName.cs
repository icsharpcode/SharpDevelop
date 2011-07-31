// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace PackageManagement.Tests.Helpers
{
	public class FileNameAndProjectName
	{
		public FileNameAndProjectName()
		{
		}
		
		public FileNameAndProjectName(string fileName, string projectName)
		{
			this.FileName = fileName;
			this.ProjectName = projectName;
		}
		
		public string FileName;
		public string ProjectName;
		
		public override bool Equals(object obj)
		{
			FileNameAndProjectName rhs = obj as FileNameAndProjectName;
			if (rhs != null) {
				return (rhs.FileName == FileName) &&
					(rhs.ProjectName == ProjectName);
			}
			return false;
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		
		public override string ToString()
		{
			return String.Format("[FileNameAndProjectName FileName={0}, ProjectName={1}]", FileName, ProjectName);
		}
	}
}
