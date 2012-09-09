// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using Rhino.Mocks;

namespace ICSharpCode.SharpDevelop.Tests.Utils
{
	public class ProjectHelper
	{
		public IProject Project = MockRepository.GenerateMock<IProject, IBuildable>();
		public List<ProjectItem> ProjectItems = new List<ProjectItem>();
		public Properties ProjectSpecificProperties = new Properties();
		
		public ProjectHelper(string fileName)
		{
			Project.Stub(p => p.FileName).Return(fileName);
			
			Project
				.Stub(p => p.Items)
				.Return(null)
				.WhenCalled(mi => mi.ReturnValue = new ReadOnlyCollection<ProjectItem>(ProjectItems));
			
			Project.Stub(p => p.SyncRoot).Return(new Object());
		}
		
		public void AddProjectSpecificProperties()
		{
			Project.Stub(p => p.ProjectSpecificProperties).Return(ProjectSpecificProperties);
		}
		
		public void AddProjectItem(ProjectItem projectItem)
		{
			ProjectItems.Add(projectItem);
		}
	}
}
