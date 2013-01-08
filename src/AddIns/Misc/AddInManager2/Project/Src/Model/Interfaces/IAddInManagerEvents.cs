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
		
		event EventHandler<AddInInstallationEventArgs> AddInInstalled;
		void OnAddInInstalled(AddInInstallationEventArgs e);
		
		event EventHandler<AddInInstallationEventArgs> AddInUninstalled;
		void OnAddInUninstalled(AddInInstallationEventArgs e);
		
		event EventHandler<AddInExceptionEventArgs> AddInOperationError;
		void OnAddInOperationError(AddInExceptionEventArgs e);
		
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
	}
}
