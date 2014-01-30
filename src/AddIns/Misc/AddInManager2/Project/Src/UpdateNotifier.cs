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
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop;
using ICSharpCode.AddInManager2.Model;
using ICSharpCode.AddInManager2.ViewModel;
using ICSharpCode.AddInManager2.View;

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
		private NotifyIcon _notifyIcon;
		private bool _hasNotified;
		private PackageRepository _firstRepositoryWithUpdates;
		
		public UpdateNotifier()
			: this(AddInManagerServices.Services)
		{
		}

		public UpdateNotifier(IAddInManagerServices services)
		{
			_isDetached = false;
			_services = services;
			_updatedAddInViewModel = new UpdatedAddInsViewModel(services);
			_services.Events.PackageListDownloadEnded += Events_PackageListDownloadEnded;
		}
		
		private void NotifyIcon_Click(object sender, EventArgs e)
		{
			// Remove the notify icon
			DestroyIcon();
			
			// Show AddInManager window on click
			using (AddInManagerView view = AddInManagerView.Create())
			{
				var viewModel = view.ViewModel;
				if (viewModel != null)
				{
					// Activate update view explicitly
					viewModel.UpdatedAddInsViewModel.IsExpandedInView = true;
					var firstRepositoryWithUpdates =
						viewModel.UpdatedAddInsViewModel.PackageRepositories.FirstOrDefault(pr => pr.SourceUrl == _firstRepositoryWithUpdates.SourceUrl);
					if (firstRepositoryWithUpdates != null)
					{
						// Directly go to first repository containing an update
						viewModel.UpdatedAddInsViewModel.SelectedPackageSource = firstRepositoryWithUpdates;
					}
				}
				_firstRepositoryWithUpdates = null;
				view.ShowDialog();
			}
		}
		
		private void DestroyIcon()
		{
			if (_notifyIcon != null)
			{
				_notifyIcon.Dispose();
				_notifyIcon = null;
				
				_services.Events.AddInManagerViewOpened -= Events_AddInManagerViewOpened;
			}
		}
		
		private void Detach()
		{
			if (!_isDetached)
			{
				_services.Events.PackageListDownloadEnded -= Events_PackageListDownloadEnded;
				_isDetached = true;
			}
		}
		
		public void StartUpdateLookup()
		{
			if (!_isDetached && !_hasNotified)
			{
				// Start getting updates
				_updatedAddInViewModel.ReadPackages();
			}
		}
		
		private void Events_AddInManagerViewOpened(object sender, EventArgs e)
		{
			// AddInManager dialog has been opened through menu, not through the NotifyIcon -> hide the icon
			DestroyIcon();
		}
		
		private void Events_PackageListDownloadEnded(object sender, PackageListDownloadEndedEventArgs e)
		{
			if (sender != _updatedAddInViewModel)
			{
				return;
			}
			
			if (e.WasCancelled)
			{
				return;
			}
			
			// Do we have any new updates? Collect this information from all configured repositories
			if (e.WasSuccessful)
			{
				_firstRepositoryWithUpdates = _updatedAddInViewModel.PackageRepositories.FirstOrDefault(pr => pr.HasHighlightCount);
				if (_firstRepositoryWithUpdates != null)
				{
					// There must be updates, show an update notification
					_hasNotified = true;
					Detach();
					
					_services.Events.AddInManagerViewOpened += Events_AddInManagerViewOpened;
					
					_notifyIcon = new NotifyIcon();
					_notifyIcon.Icon = Icon.ExtractAssociatedIcon(Assembly.GetEntryAssembly().Location);
					_notifyIcon.Click += NotifyIcon_Click;
					_notifyIcon.BalloonTipClicked += NotifyIcon_Click;
					
					_notifyIcon.Text = SD.ResourceService.GetString("AddInManager2.UpdateNotifier.BubbleTitle");
					_notifyIcon.BalloonTipTitle = _notifyIcon.Text;
					_notifyIcon.BalloonTipText = SD.ResourceService.GetString("AddInManager2.UpdateNotifier.BubbleText");
					
					_notifyIcon.Visible = true;
					_notifyIcon.ShowBalloonTip(40000);
					
					return;
				}
			}
			
			Detach();
		}
	}
}
