// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.PackageManagement
{
	public class ManagePackagesUserPrompts
	{
		ILicenseAcceptanceService licenseAcceptanceService;
		ISelectProjectsService selectProjectsService;
		IPackageManagementEvents packageManagementEvents;
		
		public ManagePackagesUserPrompts(IPackageManagementEvents packageManagementEvents)
			: this(
				packageManagementEvents,
				new LicenseAcceptanceService(),
				new SelectProjectsService())
		{
		}
		
		public ManagePackagesUserPrompts(
			IPackageManagementEvents packageManagementEvents,
			ILicenseAcceptanceService licenseAcceptanceService,
			ISelectProjectsService selectProjectsService)
		{
			this.packageManagementEvents = packageManagementEvents;
			this.licenseAcceptanceService = licenseAcceptanceService;
			this.selectProjectsService = selectProjectsService;
			
			SubscribeToEvents();
		}
		
		void SubscribeToEvents()
		{
			packageManagementEvents.AcceptLicenses += AcceptLicenses;			
			packageManagementEvents.SelectProjects += SelectProjects;
		}
		
		void AcceptLicenses(object sender, AcceptLicensesEventArgs e)
		{
			e.IsAccepted = licenseAcceptanceService.AcceptLicenses(e.Packages);
		}
		
		void SelectProjects(object sender, SelectProjectsEventArgs e)
		{
			e.IsAccepted = selectProjectsService.SelectProjects(e.SelectedProjects);
		}
		
		public void Dispose()
		{
			UnsubscribeFromEvents();
		}
		
		public void UnsubscribeFromEvents()
		{
			packageManagementEvents.SelectProjects -= SelectProjects;
			packageManagementEvents.AcceptLicenses -= AcceptLicenses;
		}
	}
}
