// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.AspNet.Mvc
{
	public interface IAddMvcItemToProjectView
	{
		bool? ShowDialog();
		object DataContext { get; set; }
	}
}
