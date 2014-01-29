// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NuGet;

namespace ICSharpCode.AddInManager2.Model
{
	/// <summary>
	/// Interface for AddInManager event service.
	/// </summary>
	public interface IAddInManagerEvents
	{
		event EventHandler OperationStarted;
		void OnOperationStarted(EventArgs e);
		void OnOperationStarted();
		
		event EventHandler AddInManagerViewOpened;
		void OnAddInManagerViewOpened(EventArgs e);
		void OnAddInManagerViewOpened();
		
		event EventHandler<PackageListDownloadEndedEventArgs> PackageListDownloadEnded;
		void OnPackageListDownloadEnded(object sender, PackageListDownloadEndedEventArgs e);
		
		event EventHandler<AddInInstallationEventArgs> AddInInstalled;
		void OnAddInInstalled(AddInInstallationEventArgs e);
		
		event EventHandler<AddInInstallationEventArgs> AddInUninstalled;
		void OnAddInUninstalled(AddInInstallationEventArgs e);
		
		event EventHandler<AddInOperationErrorEventArgs> AddInOperationError;
		void OnAddInOperationError(AddInOperationErrorEventArgs e);
		
		event EventHandler<PackageOperationEventArgs> AddInPackageDownloaded;
		void OnAddInPackageDownloaded(PackageOperationEventArgs e);
		
		event EventHandler<PackageOperationEventArgs> AddInPackageRemoved;
		void OnAddInPackageRemoved(PackageOperationEventArgs e);
		
		event EventHandler<AddInInstallationEventArgs> AddInStateChanged;
		void OnAddInStateChanged(AddInInstallationEventArgs e);
		
		event EventHandler<PackageMessageLoggedEventArgs> PackageMessageLogged;
		void OnPackageMessageLogged(PackageMessageLoggedEventArgs e);
		
		event EventHandler<AcceptLicensesEventArgs> AcceptLicenses;
		void OnAcceptLicenses(AcceptLicensesEventArgs e);
		
		event EventHandler<EventArgs> PackageSourcesChanged;
		void OnPackageSourcesChanged(EventArgs e);
	}
}
