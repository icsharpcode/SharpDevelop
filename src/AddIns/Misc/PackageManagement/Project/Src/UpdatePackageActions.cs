// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PackageManagement.Scripting;

namespace ICSharpCode.PackageManagement
{
	public abstract class UpdatePackageActions : IUpdatePackageActions
	{
		public bool UpdateDependencies { get; set; }		
		public bool AllowPrereleaseVersions { get; set; }
		public IPackageScriptRunner PackageScriptRunner { get; set; }
		
		public abstract IEnumerable<UpdatePackageAction> CreateActions();
		
		protected UpdatePackageAction CreateDefaultUpdatePackageAction(IPackageManagementProject project)
		{
			UpdatePackageAction action = project.CreateUpdatePackageAction();
			SetUpdatePackageActionProperties(action);
			return action;
		}
		
		void SetUpdatePackageActionProperties(UpdatePackageAction action)
		{
			action.PackageScriptRunner = PackageScriptRunner;
			action.UpdateDependencies = UpdateDependencies;
			action.UpdateIfPackageDoesNotExistInProject = false;
			action.AllowPrereleaseVersions = AllowPrereleaseVersions;
		}
	}
}
