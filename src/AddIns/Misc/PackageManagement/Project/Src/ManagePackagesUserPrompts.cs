// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class ManagePackagesUserPrompts
	{
		ILicenseAcceptanceService licenseAcceptanceService;
		ISelectProjectsService selectProjectsService;
		IPackageManagementEvents packageManagementEvents;
		IFileConflictResolver fileConflictResolver;
		FileConflictResolution lastFileConflictResolution;
		
		public ManagePackagesUserPrompts(IPackageManagementEvents packageManagementEvents)
			: this(
				packageManagementEvents,
				new LicenseAcceptanceService(),
				new SelectProjectsService(),
				new FileConflictResolver())
		{
		}
		
		public ManagePackagesUserPrompts(
			IPackageManagementEvents packageManagementEvents,
			ILicenseAcceptanceService licenseAcceptanceService,
			ISelectProjectsService selectProjectsService,
			IFileConflictResolver fileConflictResolver)
		{
			this.packageManagementEvents = packageManagementEvents;
			this.licenseAcceptanceService = licenseAcceptanceService;
			this.selectProjectsService = selectProjectsService;
			this.fileConflictResolver = fileConflictResolver;
			
			ResetFileConflictResolution();
			SubscribeToEvents();
		}
		
		void ResetFileConflictResolution()
		{
			lastFileConflictResolution = FileConflictResolution.Overwrite;
		}
		
		void SubscribeToEvents()
		{
			packageManagementEvents.AcceptLicenses += AcceptLicenses;
			packageManagementEvents.SelectProjects += SelectProjects;
			packageManagementEvents.ResolveFileConflict += ResolveFileConflict;
			packageManagementEvents.PackageOperationsStarting += PackageOperationsStarting;
		}
		
		void AcceptLicenses(object sender, AcceptLicensesEventArgs e)
		{
			e.IsAccepted = licenseAcceptanceService.AcceptLicenses(e.Packages);
		}
		
		void SelectProjects(object sender, SelectProjectsEventArgs e)
		{
			e.IsAccepted = selectProjectsService.SelectProjects(e.SelectedProjects);
		}
		
		void ResolveFileConflict(object sender, ResolveFileConflictEventArgs e)
		{
			if (UserPreviouslySelectedOverwriteAllOrIgnoreAll()) {
				e.Resolution = lastFileConflictResolution;
			} else {
				e.Resolution = fileConflictResolver.ResolveFileConflict(e.Message);
				lastFileConflictResolution = e.Resolution;
			}
		}
		
		bool UserPreviouslySelectedOverwriteAllOrIgnoreAll()
		{
			return
				(lastFileConflictResolution == FileConflictResolution.IgnoreAll) ||
				(lastFileConflictResolution == FileConflictResolution.OverwriteAll);
		}
		
		void PackageOperationsStarting(object sender, EventArgs e)
		{
			ResetFileConflictResolution();
		}
		
		public void Dispose()
		{
			UnsubscribeFromEvents();
		}
		
		public void UnsubscribeFromEvents()
		{
			packageManagementEvents.SelectProjects -= SelectProjects;
			packageManagementEvents.AcceptLicenses -= AcceptLicenses;
			packageManagementEvents.ResolveFileConflict -= ResolveFileConflict;
			packageManagementEvents.PackageOperationsStarting -= PackageOperationsStarting;
		}
	}
}
