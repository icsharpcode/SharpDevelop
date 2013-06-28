// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using ICSharpCode.SharpDevelop;
using ICSharpCode.AddInManager2.Model;
using ICSharpCode.AddInManager2.ViewModel;

namespace ICSharpCode.AddInManager2
{
	/// <summary>
	/// Checks configured repositories for updates and shows a user notification.
	/// </summary>
	public class UpdateNotifier
	{
		private IAddInManagerServices _services;
		private UpdatedAddInsViewModel _updatedAddInViewModel;
		private bool _isDetached;
		
		public UpdateNotifier()
			: this(AddInManagerServices.Services)
		{
		}

		public UpdateNotifier(IAddInManagerServices services)
		{
			_isDetached = false;
			_services = services;
			_updatedAddInViewModel = new UpdatedAddInsViewModel(services);
			_updatedAddInViewModel.PackageListDownloadEnded += UpdatedAddInViewModel_PackageListDownloadEnded;
		}
		
		public void Detach()
		{
			if (!_isDetached)
			{
				_updatedAddInViewModel.PackageListDownloadEnded -= UpdatedAddInViewModel_PackageListDownloadEnded;
				_isDetached = true;
			}
		}
		
		public void StartUpdateLookup()
		{
			if (!_isDetached)
			{
				// Start getting updates
				_updatedAddInViewModel.ReadPackages();
			}
		}
		
		public void UpdatedAddInViewModel_PackageListDownloadEnded(object sender, EventArgs e)
		{
			// Do we have any new updates? Collect this information from all configured repositories
			var allRepositories = _updatedAddInViewModel.PackageRepositories;
			if (allRepositories != null)
			{
				if (allRepositories.Any(rep => rep.HasHighlightCount))
				{
					// TODO There must be updates, show an update notification
					Detach();
					SD.MessageService.ShowWarning("There are updates!");
					return;
				}
			}
			
			Detach();
		}
	}
}
