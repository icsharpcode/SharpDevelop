// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.PackageManagement;
using NuGet;

namespace PackageManagement.Tests.Helpers
{
	public class FakePackageManagementEvents : IThreadSafePackageManagementEvents
	{
		#pragma warning disable 0067
		public event EventHandler PackageOperationsStarting;
		public event EventHandler<AcceptLicensesEventArgs> AcceptLicenses;
		public event EventHandler<SelectProjectsEventArgs> SelectProjects;
		public event EventHandler<PackageOperationExceptionEventArgs> PackageOperationError;
		public event EventHandler<ParentPackageOperationEventArgs> ParentPackageInstalled;
		public event EventHandler<ParentPackageOperationEventArgs> ParentPackageUninstalled;
		public event EventHandler<PackageOperationMessageLoggedEventArgs> PackageOperationMessageLogged;
		#pragma warning restore 0067
		
		public bool IsOnPackageOperationsStartingCalled;
		
		public void OnPackageOperationsStarting()
		{
			IsOnPackageOperationsStartingCalled = true;
		}
		
		public Exception ExceptionPassedToOnPackageOperationError;
		
		public void OnPackageOperationError(Exception ex)
		{
			ExceptionPassedToOnPackageOperationError = ex;
		}
		
		public IEnumerable<IPackage> LastPackagesPassedToOnAcceptLicenses;
		public bool IsOnAcceptLicensesCalled;
		public bool OnAcceptLicensesReturnValue;
		public List<IEnumerable<IPackage>> PackagesPassedToAcceptLicenses = 
			new List<IEnumerable<IPackage>>();
		
		public bool OnAcceptLicenses(IEnumerable<IPackage> packages)
		{
			LastPackagesPassedToOnAcceptLicenses = packages;
			PackagesPassedToAcceptLicenses.Add(packages);
			IsOnAcceptLicensesCalled = true;
			return OnAcceptLicensesReturnValue;
		}
		
		public IPackage PackagePassedToOnParentPackageInstalled;
		public bool IsOnParentPackageInstalledCalled;
		
		public void OnParentPackageInstalled(IPackage package)
		{
			PackagePassedToOnParentPackageInstalled = package;
			IsOnParentPackageInstalledCalled = true;
		}
		
		public IPackage PackagePassedToOnParentPackageUninstalled;
		
		public void OnParentPackageUninstalled(IPackage package)
		{
			PackagePassedToOnParentPackageUninstalled = package;
		}
		
		public MessageLevel MessageLevelPassedToOnPackageOperationMessageLogged;
		public string FormattedStringPassedToOnPackageOperationMessageLogged;
		
		public void OnPackageOperationMessageLogged(MessageLevel level, string message, params object[] args)
		{
			MessageLevelPassedToOnPackageOperationMessageLogged = level;
			FormattedStringPassedToOnPackageOperationMessageLogged = String.Format(message, args);
		}
		
		public bool IsDisposed;
		
		public void Dispose()
		{
			IsDisposed = true;
		}
		
		public bool OnSelectProjectsReturnValue;
		public IEnumerable<IPackageManagementSelectedProject> SelectedProjectsPassedToOnSelectProjects;
		public bool IsOnSelectProjectsCalled;
		public List<string> ProjectsToSelect = new List<string>();
		
		public bool OnSelectProjects(IEnumerable<IPackageManagementSelectedProject> selectedProjects)
		{
			IsOnSelectProjectsCalled = true;
			SelectedProjectsPassedToOnSelectProjects = selectedProjects;
			
			SetIsSelectedForSelectedProjects(selectedProjects);
			
			return OnSelectProjectsReturnValue;
		}
		
		void SetIsSelectedForSelectedProjects(IEnumerable<IPackageManagementSelectedProject> selectedProjects)
		{
			if (ProjectsToSelect.Any()) {
				List<IPackageManagementSelectedProject> selectedProjectsAsList = selectedProjects.ToList();
				selectedProjectsAsList.ForEach(p => p.IsSelected = ProjectsToSelect.Contains(p.Name));
			}
		}
		
		public List<IPackage> GetPackagesPassedToOnAcceptLicensesAsList()
		{
			return new List<IPackage>(LastPackagesPassedToOnAcceptLicenses);
		}
	}
}
