// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
