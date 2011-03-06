// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.TextTemplating;

namespace TextTemplating.Tests.Helpers
{
	public class FakeTextTemplatingCustomToolContext : ITextTemplatingCustomToolContext
	{
		public FileProjectItem BaseItemPassedToEnsureOutputFileIsInProject;
		public string OutputFileNamePassedToEnsureOutputFileIsInProject;
		public TestableFileProjectItem EnsureOutputFileIsInProjectReturnValue = new TestableFileProjectItem(@"d:\Projects\MyProject\template.tt");
		
		public FileProjectItem EnsureOutputFileIsInProject(FileProjectItem baseItem, string outputFileName)
		{
			BaseItemPassedToEnsureOutputFileIsInProject = baseItem;
			OutputFileNamePassedToEnsureOutputFileIsInProject = outputFileName;
			
			return EnsureOutputFileIsInProjectReturnValue;
		}
	}
}
