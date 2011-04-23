// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.PackageManagement.Design
{
	public class FakeUpdatePackageAction : UpdatePackageAction
	{
		public bool IsExecuted;
		
		public FakeUpdatePackageAction()
			: base((IPackageManagementProject)null, null)
		{
		}
		
		protected override void ExecuteCore()
		{
			IsExecuted = true;
		}
		
		protected override void BeforeExecute()
		{
		}
	}
}
