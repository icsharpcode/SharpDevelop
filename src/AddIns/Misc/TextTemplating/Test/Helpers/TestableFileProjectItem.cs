// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;

namespace TextTemplating.Tests.Helpers
{
	public class TestableFileProjectItem : FileProjectItem
	{
		string fileName;
		
		public TestableProject TestableProject;
		
		public TestableFileProjectItem(string fileName)
			: this(ProjectHelper.CreateProject(), fileName)
		{
		}
		
		TestableFileProjectItem(TestableProject project, string fileName)
			: base(project, ItemType.None)
		{
			this.TestableProject = project;
			this.fileName = fileName;
		}
		
		public override string FileName {
			get { return fileName; }
			set { fileName = value; }
		}
		
		public string CustomToolNamespace {
			get { return GetEvaluatedMetadata("CustomToolNamespace"); }
			set { SetMetadata("CustomToolNamespace", value); }
		}
	}
}
