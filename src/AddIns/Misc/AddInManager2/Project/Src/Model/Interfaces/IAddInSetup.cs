// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.Core;
using NuGet;

namespace ICSharpCode.AddInManager2.Model
{
	public interface IAddInSetup
	{
		IEnumerable<ManagedAddIn> AddInsWithMarkedForInstallation
		{
			get;
		}
		AddIn InstallAddIn(string archiveFileName);
		AddIn InstallAddIn(IPackage package, string packageDirectory);
		void UninstallAddIn(AddIn addIn);
		void CancelUpdate(AddIn addIn);
		void CancelInstallation(AddIn addIn);
		void CancelUninstallation(AddIn addIn);
		void SwitchAddInActivation(AddIn addIn);
		AddIn GetInstalledAddInByIdentity(string identity);
		bool IsAddInInstalled(AddIn addIn);
		bool IsAddInPreinstalled(AddIn addin);
		int CompareAddInToPackageVersion(AddIn addIn, IPackage nuGetPackage);
		IPackage GetNuGetPackageForAddIn(AddIn addIn, bool getLatest);
		AddIn GetAddInForNuGetPackage(IPackage package);
		AddIn GetAddInForNuGetPackage(IPackage package, bool withAddInsMarkedForInstallation);
		IEnumerable<ManagedAddIn> GetDependentAddIns(AddIn addIn);
		void RemoveUnreferencedNuGetPackages();
	}
}
