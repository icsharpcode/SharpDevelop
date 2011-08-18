// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.AspNet.Mvc;

namespace AspNet.Mvc.Tests.Helpers
{
	public class FakeMvcProjectContent : IMvcProjectContent
	{
		public List<FakeMvcClass> FakeClasses = new List<FakeMvcClass>();
		
		public IEnumerable<IMvcClass> GetClasses()
		{
			return FakeClasses;
		}
		
		public void AddFakeClass(string fullyQualifiedClassName)
		{
			var fakeClass = new FakeMvcClass(fullyQualifiedClassName);
			FakeClasses.Add(fakeClass);
		}
	}
}
