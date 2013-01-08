// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NuGet;

namespace ICSharpCode.AddInManager2.Model
{
	/// <summary>
	/// Catches and broadcasts package-related events.
	/// </summary>
	public class AddInManagerEvents : IAddInManagerEvents
	{
		public event EventHandler OperationStarted;
		
		public void OnOperationStarted(EventArgs e)
		{
			if (OperationStarted != null)
			{
				OperationStarted(this, e);
			}
		}
		
		public event EventHandler<AddInInstallationEventArgs> AddInInstalled;
		
		public void OnAddInInstalled(AddInInstallationEventArgs e)
		{
			if (AddInInstalled != null)
			{
				AddInInstalled(this, e);
			}
		}
		
		public event EventHandler<AddInInstallationEventArgs> AddInUninstalled;
		
		public void OnAddInUninstalled(AddInInstallationEventArgs e)
		{
			if (AddInUninstalled != null)
			{
				AddInUninstalled(this, e);
			}
		}
		
		public event EventHandler<AddInExceptionEventArgs> AddInOperationError;
		
		public void OnAddInOperationError(AddInExceptionEventArgs e)
		{
			if (AddInOperationError != null)
			{
				AddInOperationError(this, e);
			}
		}
		
		public event EventHandler<PackageOperationEventArgs> AddInPackageDownloaded;
		
		public void OnAddInPackageDownloaded(PackageOperationEventArgs e)
		{
			if (AddInPackageDownloaded != null)
			{
				AddInPackageDownloaded(this, e);
			}
		}
		
		public event EventHandler<PackageOperationEventArgs> AddInPackageRemoved;
		
		public void OnAddInPackageRemoved(PackageOperationEventArgs e)
		{
			if (AddInPackageRemoved != null)
			{
				AddInPackageRemoved(this, e);
			}
		}
		
		public event EventHandler<AddInInstallationEventArgs> AddInStateChanged;
		
		public void OnAddInStateChanged(AddInInstallationEventArgs e)
		{
			if (AddInStateChanged != null)
			{
				AddInStateChanged(this, e);
			}
		}
		
		public event EventHandler<PackageMessageLoggedEventArgs> PackageMessageLogged;
		
		public void OnPackageMessageLogged(PackageMessageLoggedEventArgs e)
		{
			if (PackageMessageLogged != null)
			{
				PackageMessageLogged(this, e);
			}
		}
		
		public event EventHandler<AcceptLicensesEventArgs> AcceptLicenses;
		
		public void OnAcceptLicenses(AcceptLicensesEventArgs e)
		{
			if (AcceptLicenses != null)
			{
				AcceptLicenses(this, e);
			}
		}
	}
}
