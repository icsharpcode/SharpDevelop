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
