// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public interface IPackageManagementEvents
	{
		event EventHandler PackageOperationsStarting;
		event AcceptLicensesEventHandler AcceptLicenses;
		event PackageOperationExceptionEventHandler PackageOperationError;
		
		void OnPackageOperationsStarting();		
		void OnPackageOperationError(Exception ex);
		bool OnAcceptLicenses(IEnumerable<IPackage> packages);
	}
}
