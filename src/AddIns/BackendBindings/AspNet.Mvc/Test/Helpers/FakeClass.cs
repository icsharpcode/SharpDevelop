// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace AspNet.Mvc.Tests.Helpers
{
	public class FakeClass : DefaultClass
	{
		public FakeClass(string name)
			: base(new DefaultCompilationUnit(new DefaultProjectContent()), name)
		{
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
