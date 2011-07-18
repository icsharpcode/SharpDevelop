// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class PackageManagementEvents : IPackageManagementEvents
	{
		public event EventHandler PackageOperationsStarting;
		
		public void OnPackageOperationsStarting()
		{
			if (PackageOperationsStarting != null) {
				PackageOperationsStarting(this, new EventArgs());
			}
		}
		
		public event EventHandler<PackageOperationExceptionEventArgs> PackageOperationError;
		
		public void OnPackageOperationError(Exception ex)
		{
			if (PackageOperationError != null) {
				PackageOperationError(this, new PackageOperationExceptionEventArgs(ex));
			}
		}
		
		public event EventHandler<AcceptLicensesEventArgs> AcceptLicenses;
		
		public bool OnAcceptLicenses(IEnumerable<IPackage> packages)
		{
			if (AcceptLicenses != null) {
				var eventArgs = new AcceptLicensesEventArgs(packages);
				AcceptLicenses(this, eventArgs);
				return eventArgs.IsAccepted;
			}
			return true;
		}
		
		public event EventHandler<ParentPackageOperationEventArgs> ParentPackageInstalled;
		
		public void OnParentPackageInstalled(IPackage package)
		{
			if (ParentPackageInstalled != null) {
				ParentPackageInstalled(this, new ParentPackageOperationEventArgs(package));
			}
		}
		
		public event EventHandler<ParentPackageOperationEventArgs> ParentPackageUninstalled;
		
		public void OnParentPackageUninstalled(IPackage package)
		{
			if (ParentPackageUninstalled != null) {
				ParentPackageUninstalled(this, new ParentPackageOperationEventArgs(package));
			}
		}
		
		public event EventHandler<PackageOperationMessageLoggedEventArgs> PackageOperationMessageLogged;
		
		public void OnPackageOperationMessageLogged(MessageLevel level, string message, params object[] args)
		{
			if (PackageOperationMessageLogged != null) {
				var eventArgs = new PackageOperationMessageLoggedEventArgs(level, message, args);
				PackageOperationMessageLogged(this, eventArgs);
			}
		}
		
		public event EventHandler<SelectProjectsEventArgs> SelectProjects;
		
		public bool OnSelectProjects(IEnumerable<IPackageManagementSelectedProject> projects)
		{
			if (SelectProjects != null) {
				var eventArgs = new SelectProjectsEventArgs(projects);
				SelectProjects(this, eventArgs);
				return eventArgs.IsAccepted;
			}
			return true;
		}
	}
}
