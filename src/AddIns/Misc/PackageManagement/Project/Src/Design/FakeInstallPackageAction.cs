// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.PackageManagement.Design
{
	public class FakeInstallPackageAction : InstallPackageAction
	{
		public FakeInstallPackageAction()
			: this(null)
		{
		}
		
		public FakeInstallPackageAction(IPackageManagementProject project)
			: base(project, null)
		{
		}
		
		public bool IsExecuteCalled;
		
		protected override void ExecuteCore()
		{
			IsExecuteCalled = true;
		}
		
		protected override void BeforeExecute()
		{
		}
	}
}
