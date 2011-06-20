// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public abstract class ProcessPackageOperationsAction : ProcessPackageAction
	{
		public ProcessPackageOperationsAction(
			IPackageManagementProject project,
			IPackageManagementEvents packageManagementEvents)
			: base(project, packageManagementEvents)
		{
		}
		
		public IEnumerable<PackageOperation> Operations { get; set; }

		public override bool HasPackageScriptsToRun()
		{
			BeforeExecute();
			var files = new PackageFilesForOperations(Operations);
			return files.HasAnyPackageScripts();
		}
		
		protected override void BeforeExecute()
		{
			base.BeforeExecute();
			GetPackageOperationsIfMissing();
		}
				
		void GetPackageOperationsIfMissing()
		{
			if (Operations == null) {
				Operations = GetPackageOperations();
			}
		}
		
		protected virtual IEnumerable<PackageOperation> GetPackageOperations()
		{
			return null;
		}
	}
}
