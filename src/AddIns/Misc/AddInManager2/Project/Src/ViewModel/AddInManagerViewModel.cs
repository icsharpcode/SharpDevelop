// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using ICSharpCode.AddInManager2.Model;
using ICSharpCode.AddInManager2.View;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using NuGet;

namespace ICSharpCode.AddInManager2.ViewModel
{
	public class AddInManagerViewModel : Model<AddInManagerViewModel>, IDisposable
	{
		private string _message;
		private bool _hasError;
		
		public AddInManagerViewModel()
			: base()
		{
			Initialize();
		}
		
		public AddInManagerViewModel(IAddInManagerServices services)
			: base(services)
		{
			Initialize();
		}
		
		private void Initialize()
		{
			// Visuals
			this.Title = SD.ResourceService.GetString("AddInManager.Title");
			
			// Add event handlers
			AddInManager.Events.OperationStarted += AddInManager_Events_OperationStarted;
			AddInManager.Events.AddInOperationError += AddInManager_Events_NuGetPackageOperationError;
			AddInManager.Events.AcceptLicenses += AddInManager_Events_AcceptLicenses;
			
			AvailableAddInsViewModel = new AvailableAddInsViewModel();
			InstalledAddInsViewModel = new InstalledAddInsViewModel();
			UpdatedAddInsViewModel = new UpdatedAddInsViewModel();
			
			// Read the packages
			AvailableAddInsViewModel.ReadPackages();
			InstalledAddInsViewModel.ReadPackages();
			UpdatedAddInsViewModel.ReadPackages();
		}
		
		public AvailableAddInsViewModel AvailableAddInsViewModel
		{
			get;
			private set;
		}
		
		public InstalledAddInsViewModel InstalledAddInsViewModel
		{
			get;
			private set;
		}
		
		public UpdatedAddInsViewModel UpdatedAddInsViewModel
		{
			get;
			private set;
		}
		
		public string Title
		{
//			get { return viewTitle.Title; }
			get;
			private set;
		}
		
		public void Dispose()
		{
			AddInManager.Events.AddInOperationError -= AddInManager_Events_NuGetPackageOperationError;
			AddInManager.Events.AcceptLicenses -= AddInManager_Events_AcceptLicenses;
		}
		
		private void ShowErrorMessage(string message)
		{
			this.Message = message;
			this.HasError = true;
		}
		
		public string Message
		{
			get
			{
				return _message;
			}
			set
			{
				_message = value;
				OnPropertyChanged(model => model.Message);
			}
		}
		
		public bool HasError
		{
			get
			{
				return _hasError;
			}
			set
			{
				_hasError = value;
				OnPropertyChanged(model => model.HasError);
			}
		}
		
		private void AddInManager_Events_OperationStarted(object sender, EventArgs e)
		{
			ClearMessage();
		}
		
		private void ClearMessage()
		{
			this.Message = null;
			this.HasError = false;
		}
		
		private void AddInManager_Events_NuGetPackageOperationError(object sender, AddInExceptionEventArgs e)
		{
			ShowErrorMessage(e.Exception.Message);
		}
		
		private void AddInManager_Events_AcceptLicenses(object sender, AcceptLicensesEventArgs e)
		{
			// Show a license acceptance prompt to the user
			e.IsAccepted = ShowLicenseAcceptancePrompt(e.Packages);
		}
		
		private bool ShowLicenseAcceptancePrompt(IEnumerable<IPackage> packages)
		{
			if (packages == null)
			{
				// No package -> nothing to accept
				return true;
			}
			
			// Create a license acceptance view
			var viewModel = new LicenseAcceptanceViewModel(packages);
			var view = new LicenseAcceptanceView();
			view.DataContext = viewModel;
			view.Owner = SD.Workbench.MainWindow;
			return view.ShowDialog() ?? false;
		}
	}
}
