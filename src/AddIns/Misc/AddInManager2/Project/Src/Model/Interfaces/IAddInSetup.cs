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
