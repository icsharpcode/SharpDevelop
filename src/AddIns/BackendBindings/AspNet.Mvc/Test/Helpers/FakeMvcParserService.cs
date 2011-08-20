// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AspNet.Mvc;

namespace AspNet.Mvc.Tests.Helpers
{
	public class FakeMvcParserService : IMvcParserService
	{
		public IMvcProject ProjectPassedToGetProjectContent;
		public FakeMvcProjectContent FakeMvcProjectContent = new FakeMvcProjectContent();
		
		public IMvcProjectContent GetProjectContent(IMvcProject project)
		{
			ProjectPassedToGetProjectContent = project;
			return FakeMvcProjectContent;
		}
		
		public FakeMvcClass AddModelClassToProjectContent(string fullyQualifiedClassName)
		{
			return FakeMvcProjectContent.AddFakeClass(fullyQualifiedClassName);
		}
		
		public FakeMvcClass AddModelClassWithBaseClassToProjectContent(string baseClassName, string className)
		{
			return FakeMvcProjectContent.AddFakeClassWithBaseClass(baseClassName, className);
		}
	}
}
