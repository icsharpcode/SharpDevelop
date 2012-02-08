// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace AspNet.Mvc.Tests.Helpers
{
	public class TestableProjectContent : DefaultProjectContent
	{
		public TestableProject TestableProject = TestableProject.CreateProject();
		
		public override object Project {
			get { return TestableProject; }
		}
	}
}
