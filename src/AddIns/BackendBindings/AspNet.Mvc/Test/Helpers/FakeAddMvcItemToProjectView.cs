// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AspNet.Mvc;

namespace AspNet.Mvc.Tests.Helpers
{
	public class FakeAddMvcItemToProjectView : IAddMvcItemToProjectView
	{
		public bool IsShowDialogCalled;
		
		public bool? ShowDialog()
		{
			IsShowDialogCalled = true;
			return true;
		}
		
		public object DataContext { get; set; }
	}
}
