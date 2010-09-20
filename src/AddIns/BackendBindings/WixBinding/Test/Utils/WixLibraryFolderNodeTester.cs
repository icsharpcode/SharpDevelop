// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.WixBinding;
using System;

namespace WixBinding.Tests.Utils
{
	/// <summary>
	/// WixLibraryFolderNode derived class that allows us to run the protected
	/// Initialize method.
	/// </summary>
	public class WixLibraryFolderNodeTester : WixLibraryFolderNode
	{
		public WixLibraryFolderNodeTester(IProject project) : base(project)
		{
		}
		
		public void RunInitialize()
		{
			base.Initialize();
		}
	}
}
