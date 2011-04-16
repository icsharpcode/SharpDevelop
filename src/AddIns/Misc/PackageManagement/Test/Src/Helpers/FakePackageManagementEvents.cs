// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PackageManagement;
using NuGet;

namespace PackageManagement.Tests.Helpers
{
	public class FakePackageManagementEvents : IPackageManagementEvents
	{
		#pragma warning disable 0067
		public event EventHandler PackageOperationsStarting;
		public event EventHandler<AcceptLicensesEventArgs> AcceptLicenses;
		public event EventHandler<PackageOperationExceptionEventArgs> PackageOperationError;
		public event EventHandler<ParentPackageOperationEventArgs> ParentPackageInstalled;
		public event EventHandler<ParentPackageOperationEventArgs> ParentPackageUninstalled;
		public event EventHandler<PackageOperationMessageLoggedEventArgs> PackageOperationMessageLogged;
		#pragma warning restore 0067
		
		public bool IsOnPackageOperationsStartingCalled;
		
		public void OnPackageOperationsStarting()
		{
			IsOnPackageOperationsStartingCalled = true;
		}
		
		public Exception ExceptionPassedToOnPackageOperationError;
		
		public void OnPackageOperationError(Exception ex)
		{
			ExceptionPassedToOnPackageOperationError = ex;
		}
		
		public IEnumerable<IPackage> PackagesPassedToOnAcceptLicenses;
		public bool IsOnAcceptLicensesCalled;
		public bool AcceptLicensesReturnValue;
		
		public bool OnAcceptLicenses(IEnumerable<IPackage> packages)
		{
			PackagesPassedToOnAcceptLicenses = packages;
			IsOnAcceptLicensesCalled = true;
			return AcceptLicensesReturnValue;
		}
		
		public IPackage PackagePassedToOnParentPackageInstalled;
		
		public void OnParentPackageInstalled(IPackage package)
		{
			PackagePassedToOnParentPackageInstalled = package;
		}
		
		public IPackage PackagePassedToOnParentPackageUninstalled;
		
		public void OnParentPackageUninstalled(IPackage package)
		{
			PackagePassedToOnParentPackageUninstalled = package;
		}
		
		public MessageLevel MessageLevelPassedToOnPackageOperationMessageLogged;
		public string FormattedStringPassedToOnPackageOperationMessageLogged;
		
		public void OnPackageOperationMessageLogged(MessageLevel level, string message, params object[] args)
		{
			MessageLevelPassedToOnPackageOperationMessageLogged = level;
			FormattedStringPassedToOnPackageOperationMessageLogged = String.Format(message, args);
		}
	}
}
