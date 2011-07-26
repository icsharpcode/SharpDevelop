// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.PackageManagement.Scripting;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class UpdateAllPackagesInProject : IUpdatePackageActions
	{
		IPackageManagementProject project;
		
		public UpdateAllPackagesInProject(IPackageManagementProject project)
		{
			this.project = project;
			this.UpdateDependencies = true;
		}
		
		public bool UpdateDependencies { get; set; }
		public IPackageScriptRunner PackageScriptRunner { get; set; }
		
		public IEnumerable<UpdatePackageAction> CreateActions()
		{
			foreach (IPackage package in GetPackages()) {
				yield return CreateUpdatePackageAction(package);
			}
		}
		
		IEnumerable<IPackage> GetPackages()
		{
			return project.GetPackagesInReverseDependencyOrder();
		}
		
		UpdatePackageAction CreateUpdatePackageAction(IPackage package)
		{
			UpdatePackageAction action = CreateUpdatePackageAction();
			action.PackageId = package.Id;
			return action;
		}
		
		UpdatePackageAction CreateUpdatePackageAction()
		{
			UpdatePackageAction action = project.CreateUpdatePackageAction();
			action.UpdateIfPackageDoesNotExistInProject = false;
			action.UpdateDependencies = UpdateDependencies;
			action.PackageScriptRunner = PackageScriptRunner;
			return action;
		}
	}
}
