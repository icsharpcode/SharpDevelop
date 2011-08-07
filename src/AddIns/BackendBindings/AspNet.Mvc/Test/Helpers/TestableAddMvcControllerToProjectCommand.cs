// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AspNet.Mvc;

namespace AspNet.Mvc.Tests.Helpers
{
	public class TestableAddMvcControllerToProjectCommand : AddMvcControllerToProjectCommand
	{
		public FakeAddMvcItemToProjectView FakeAddMvcControllerToProjectView = new FakeAddMvcItemToProjectView();
		
		protected override IAddMvcItemToProjectView CreateView()
		{
			return FakeAddMvcControllerToProjectView;
		}
		
		public object FakeDataContext { get; set; }
		
		protected override object CreateDataContext()
		{
			return FakeDataContext;
		}
	}
}
