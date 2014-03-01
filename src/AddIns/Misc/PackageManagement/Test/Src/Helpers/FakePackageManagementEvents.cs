// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
		public event EventHandler<ParentPackagesOperationEventArgs> ParentPackagesUpdated;
		public event EventHandler<PackageOperationMessageLoggedEventArgs> PackageOperationMessageLogged;
		public event EventHandler<ResolveFileConflictEventArgs> ResolveFileConflict;
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
		
		public PackageViewModel PackageViewModel;
		
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
		
		public string MessagePassedToOnResolveFileConflict;
		public FileConflictResolution FileConflictResolutionToReturn = FileConflictResolution.Ignore;
		
		public FileConflictResolution OnResolveFileConflict(string message)
		{
			MessagePassedToOnResolveFileConflict = message;
			return FileConflictResolutionToReturn;
		}
		
		public IEnumerable<IPackage> PackagesPassedToOnParentPackageUpdated;
		
		public void OnParentPackagesUpdated(IEnumerable<IPackage> packages)
		{
			PackagesPassedToOnParentPackageUpdated = packages;
		}
	}
}
