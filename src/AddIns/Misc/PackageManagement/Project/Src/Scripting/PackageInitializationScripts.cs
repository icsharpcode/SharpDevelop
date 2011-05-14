// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using NuGet;

namespace ICSharpCode.PackageManagement.Scripting
{
	public class PackageInitializationScripts : IPackageInitializationScripts
	{
		ISolutionPackageRepository solutionPackageRepository;
		IPackageScriptFactory scriptFactory;
		
		public PackageInitializationScripts(
			ISolutionPackageRepository solutionPackageRepository,
			IPackageScriptFactory scriptFactory)
		{
			this.solutionPackageRepository = solutionPackageRepository;
			this.scriptFactory = scriptFactory;
		}
		
		public void Run()
		{
			foreach (IPackage package in GetPackages()) {
				IPackageScript script = CreateInitializeScript(package);
				script.Execute();
			}
		}
		
		IEnumerable<IPackage> GetPackages()
		{
			return solutionPackageRepository.GetPackagesByDependencyOrder();
		}
		
		IPackageScript CreateInitializeScript(IPackage package)
		{
			string packageInstallDirectory = solutionPackageRepository.GetInstallPath(package);
			return scriptFactory.CreatePackageInitializeScript(packageInstallDirectory);
		}
	}
}
