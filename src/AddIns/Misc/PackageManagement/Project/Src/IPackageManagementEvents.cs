// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public interface IPackageManagementEvents
	{
		event EventHandler PackageOperationsStarting;
		event EventHandler<AcceptLicensesEventArgs> AcceptLicenses;
		event EventHandler<SelectProjectsEventArgs> SelectProjects;
		event EventHandler<PackageOperationExceptionEventArgs> PackageOperationError;
		event EventHandler<ParentPackageOperationEventArgs> ParentPackageInstalled;
		event EventHandler<ParentPackageOperationEventArgs> ParentPackageUninstalled;
		event EventHandler<PackageOperationMessageLoggedEventArgs> PackageOperationMessageLogged;
	
		void OnPackageOperationsStarting();		
		void OnPackageOperationError(Exception ex);
		bool OnAcceptLicenses(IEnumerable<IPackage> packages);
		void OnParentPackageInstalled(IPackage package);
		void OnParentPackageUninstalled(IPackage package);
		void OnPackageOperationMessageLogged(MessageLevel level, string message, params object[] args);
		bool OnSelectProjects(IEnumerable<IPackageManagementSelectedProject> selectedProjects);
	}
}
