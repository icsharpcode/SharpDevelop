// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AspNet.Mvc;

namespace AspNet.Mvc.Tests.Helpers
{
	public class TestableAddMvcViewToProjectCommand : AddMvcViewToProjectCommand
	{
		public FakeAddMvcItemToProjectView FakeAddMvcViewToProjectView = new FakeAddMvcItemToProjectView();
		
		protected override IAddMvcItemToProjectView CreateView()
		{
			return FakeAddMvcViewToProjectView;
		}
		
		public object FakeDataContext { get; set; }
		
		protected override object CreateDataContext()
		{
			return FakeDataContext;
		}
	}
}
