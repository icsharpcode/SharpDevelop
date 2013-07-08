// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.Scripting;

namespace ICSharpCode.PackageManagement.Design
{
	public class FakeUninstallPackageAction : UninstallPackageAction
	{
		public bool IsExecuted;
		
		public FakeUninstallPackageAction(IPackageManagementProject project)
			: base(project, null)
		{
		}
		
		protected override void ExecuteCore()
		{
			IsExecuted = true;
		}
		
		protected override void BeforeExecute()
		{
		}
		
		protected override RunPackageScriptsAction CreateRunPackageScriptsAction(
			IPackageScriptRunner scriptRunner,
			IPackageManagementProject project)
		{
			return new RunPackageScriptsAction(
				project,
				scriptRunner,
				new PackageScriptFactory(),
				new NullGlobalMSBuildProjectCollection());
		}
	}
}
