// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace AspNet.Mvc.Tests.Helpers
{
	public class FakeClass : DefaultClass
	{
		public TestableProject TestableProject;
		
		public FakeClass(string name)
			: this(name, new TestableProjectContent())
		{
			
		}
		
		public FakeClass(string name, TestableProjectContent projectContent)
			: base(new DefaultCompilationUnit(projectContent), name)
		{
			this.TestableProject = projectContent.TestableProject;
		}
		
		public FakeClass AddBaseClass(string name)
		{
			var baseClass = new FakeClass(name);
			var returnType = new DefaultReturnType(baseClass);
			BaseTypes.Add(returnType);
			return baseClass;
		}
	}
}
