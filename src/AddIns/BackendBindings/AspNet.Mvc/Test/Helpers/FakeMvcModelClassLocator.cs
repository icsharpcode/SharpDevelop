// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.AspNet.Mvc;

namespace AspNet.Mvc.Tests.Helpers
{
	public class FakeMvcModelClassLocator : IMvcModelClassLocator
	{
		public List<FakeMvcClass> FakeModelClasses = new List<FakeMvcClass>();
		
		public void AddModelClass(string fullyQualifiedClassName)
		{
			var fakeClass = new FakeMvcClass(fullyQualifiedClassName);
			FakeModelClasses.Add(fakeClass);
		}
		
		public IMvcProject ProjectPassedToGetModelClasses;
		
		public IEnumerable<IMvcClass> GetModelClasses(IMvcProject project)
		{
			ProjectPassedToGetModelClasses = project;
			return FakeModelClasses;
		}
	}
}
