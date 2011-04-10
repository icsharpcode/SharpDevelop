// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class PackageManagementEvents : IPackageManagementEvents
	{
		public event EventHandler PackageOperationsStarting;
		
		public void OnPackageOperationsStarting()
		{
			if (PackageOperationsStarting != null) {
				PackageOperationsStarting(this, new EventArgs());
			}
		}
		
		public event PackageOperationExceptionEventHandler PackageOperationError;
		
		public void OnPackageOperationError(Exception ex)
		{
			if (PackageOperationError != null) {
				PackageOperationError(this, new PackageOperationExceptionEventArgs(ex));
			}
		}
		
		public event AcceptLicensesEventHandler AcceptLicenses;
		
		public bool OnAcceptLicenses(IEnumerable<IPackage> packages)
		{
			if (AcceptLicenses != null) {
				var eventArgs = new AcceptLicensesEventArgs(packages);
				AcceptLicenses(this, eventArgs);
				return eventArgs.IsAccepted;
			}
			return true;
		}
	}
}
