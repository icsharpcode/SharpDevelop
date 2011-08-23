// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.ObjectModel;
using System.Linq;

using ICSharpCode.PackageManagement.Scripting;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class ManagePackagesViewModel : ViewModelBase<ManagePackagesViewModel>, IDisposable
	{
		IThreadSafePackageManagementEvents packageManagementEvents;
		ManagePackagesUserPrompts userPrompts;
		PackagesViewModels packagesViewModels;
		ManagePackagesViewTitle viewTitle;
		string message;
		bool hasError;
		
		public ManagePackagesViewModel(
			PackagesViewModels packagesViewModels,
			ManagePackagesViewTitle viewTitle,
			IThreadSafePackageManagementEvents packageManagementEvents,
			ManagePackagesUserPrompts userPrompts)
		{
			this.packagesViewModels = packagesViewModels;
			this.viewTitle = viewTitle;
			this.packageManagementEvents = packageManagementEvents;
			this.userPrompts = userPrompts;
			
			packageManagementEvents.PackageOperationError += PackageOperationError;
			packageManagementEvents.PackageOperationsStarting += PackageOperationsStarting;
			
			packagesViewModels.ReadPackages();
		}
		
		public AvailablePackagesViewModel AvailablePackagesViewModel {
			get { return packagesViewModels.AvailablePackagesViewModel; }
		}
		
		public InstalledPackagesViewModel InstalledPackagesViewModel {
			get { return packagesViewModels.InstalledPackagesViewModel; }
		}
		
		public RecentPackagesViewModel RecentPackagesViewModel {
			get { return packagesViewModels.RecentPackagesViewModel; }
		}
		
		public UpdatedPackagesViewModel UpdatedPackagesViewModel {
			get { return packagesViewModels.UpdatedPackagesViewModel; }
		}
		
		public string Title {
			get { return viewTitle.Title; }
		}
		
		public void Dispose()
		{
			packagesViewModels.Dispose();
			userPrompts.Dispose();
			
			packageManagementEvents.PackageOperationError -= PackageOperationError;
			packageManagementEvents.PackageOperationsStarting -= PackageOperationsStarting;
			packageManagementEvents.Dispose();
		}
		
		void PackageOperationError(object sender, PackageOperationExceptionEventArgs e)
		{
			ShowErrorMessage(e.Exception.Message);
		}
		
		void ShowErrorMessage(string message)
		{
			this.Message = message;
			this.HasError = true;
		}
		
		public string Message {
			get { return message; }
			set {
				message = value;
				OnPropertyChanged(model => model.Message);
			}
		}
		
		public bool HasError {
			get { return hasError; }
			set {
				hasError = value;
				OnPropertyChanged(model => model.HasError);
			}
		}
		
		void PackageOperationsStarting(object sender, EventArgs e)
		{
			ClearMessage();
		}
		
		void ClearMessage()
		{
			this.Message = null;
			this.HasError = false;
		}
	}
}
