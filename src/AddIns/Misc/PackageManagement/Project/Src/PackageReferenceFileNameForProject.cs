// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.SharpDevelop.Project;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class PackageReferenceFileNameForProject
	{
		public PackageReferenceFileNameForProject(IProject project)
		{
			GetFileNameForProject(project);
		}
		
		void GetFileNameForProject(IProject project)
		{
			FileName = Path.Combine(project.Directory, Constants.PackageReferenceFile);
		}
		
		public string FileName { get; private set; }
		
		public override string ToString()
		{
			return FileName;
		}
	}
}
