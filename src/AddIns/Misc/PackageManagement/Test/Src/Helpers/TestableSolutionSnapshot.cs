// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;

namespace PackageManagement.Tests.Helpers
{
	public class TestableSolutionSnapshot : DefaultSolutionSnapshot, ISolutionSnapshotWithProjectMapping
	{
		IProject project;
		
		public TestableSolutionSnapshot(IProject project)
		{
			this.project = project;
		}
		
		public IProject GetProject(IAssembly assembly)
		{
			return GetProject(assembly.UnresolvedAssembly as IProjectContent);
		}
		
		IProject GetProject(IProjectContent projectContent)
		{
			if (this.project.ProjectContent == projectContent) {
				return project;
			}
			return null;
		}
		
		public IProjectContent GetProjectContent(IProject project)
		{
			throw new NotImplementedException();
		}
		
		public ICompilation GetCompilation(IProject project)
		{
			throw new NotImplementedException();
		}
	}
}
