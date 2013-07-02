// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop;
using NuGet;

namespace ICSharpCode.AddInManager2.Model
{
	/// <summary>
	/// Catches and broadcasts package-related events.
	/// </summary>
	public class AddInManagerEvents : IAddInManagerEvents
	{
		public event EventHandler OperationStarted;
		
		public void OnOperationStarted()
		{
			if (OperationStarted != null)
			{
				SD.Log.DebugFormatted("[AddInManager2.Events] Operation started.");
				OperationStarted(this, new EventArgs());
			}
		}
		
		public void OnOperationStarted(EventArgs e)
		{
			if (OperationStarted != null)
			{
				SD.Log.DebugFormatted("[AddInManager2.Events] Operation started.");
				OperationStarted(this, e);
			}
		}
		
		public event EventHandler AddInManagerViewOpened;
		
		public void OnAddInManagerViewOpened(EventArgs e)
		{
			if (AddInManagerViewOpened != null)
			{
				SD.Log.DebugFormatted("[AddInManager2.Events] AddInManagerView opened.");
				AddInManagerViewOpened(this, e);
			}
		}
		
		public void OnAddInManagerViewOpened()
		{
			if (AddInManagerViewOpened != null)
			{
				SD.Log.DebugFormatted("[AddInManager2.Events] AddInManagerView opened.");
				AddInManagerViewOpened(this, new EventArgs());
			}
		}
		
		public event EventHandler<PackageListDownloadEndedEventArgs> PackageListDownloadEnded;
		
		public void OnPackageListDownloadEnded(object sender, PackageListDownloadEndedEventArgs e)
		{
			if (PackageListDownloadEnded != null)
			{
				SD.Log.DebugFormatted("[AddInManager2.Events] Package list download ended (success: {0}).", e.WasSuccessful);
				PackageListDownloadEnded(sender, e);
			}
		}
		
		public event EventHandler<AddInInstallationEventArgs> AddInInstalled;
		
		public void OnAddInInstalled(AddInInstallationEventArgs e)
		{
			if (AddInInstalled != null)
			{
				SD.Log.DebugFormatted("[AddInManager2.Events] AddIn installed: {0}", e.AddIn.Name);
				AddInInstalled(this, e);
			}
		}
		
		public event EventHandler<AddInInstallationEventArgs> AddInUninstalled;
		
		public void OnAddInUninstalled(AddInInstallationEventArgs e)
		{
			if (AddInUninstalled != null)
			{
				SD.Log.DebugFormatted("[AddInManager2.Events] AddIn uninstalled: {0}", e.AddIn.Name);
				AddInUninstalled(this, e);
			}
		}
		
		public event EventHandler<AddInOperationErrorEventArgs> AddInOperationError;
		
		public void OnAddInOperationError(AddInOperationErrorEventArgs e)
		{
			if (AddInOperationError != null)
			{
				SD.Log.DebugFormatted("[AddInManager2.Events] Operation error: {0}", e.Message);
				AddInOperationError(this, e);
			}
		}
		
		public event EventHandler<PackageOperationEventArgs> AddInPackageDownloaded;
		
		public void OnAddInPackageDownloaded(PackageOperationEventArgs e)
		{
			if (AddInPackageDownloaded != null)
			{
				SD.Log.DebugFormatted("[AddInManager2.Events] Package download finished: {0} {1}", e.Package.Id, e.Package.Version.ToString());
				AddInPackageDownloaded(this, e);
			}
		}
		
		public event EventHandler<PackageOperationEventArgs> AddInPackageRemoved;
		
		public void OnAddInPackageRemoved(PackageOperationEventArgs e)
		{
			if (AddInPackageRemoved != null)
			{
				SD.Log.DebugFormatted("[AddInManager2.Events] Package removed: {0} {1}", e.Package.Id, e.Package.Version.ToString());
				AddInPackageRemoved(this, e);
			}
		}
		
		public event EventHandler<AddInInstallationEventArgs> AddInStateChanged;
		
		public void OnAddInStateChanged(AddInInstallationEventArgs e)
		{
			if (AddInStateChanged != null)
			{
				SD.Log.DebugFormatted("[AddInManager2.Events] AddIn state changed: {0}", e.AddIn.Name);
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
				SD.Log.DebugFormatted("[AddInManager2.Events] Accept license.");
				AcceptLicenses(this, e);
			}
		}
		
		public event EventHandler<EventArgs> PackageSourcesChanged;
		
		public void OnPackageSourcesChanged(EventArgs e)
		{
			if (PackageSourcesChanged != null)
			{
				SD.Log.DebugFormatted("[AddInManager2.Events] Package sources changed.");
				PackageSourcesChanged(this, e);
			}
		}
	}
}
