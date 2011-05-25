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
		List<IPackageScript> scripts;
		
		public PackageInitializationScripts(
			ISolutionPackageRepository solutionPackageRepository,
			IPackageScriptFactory scriptFactory)
		{
			this.solutionPackageRepository = solutionPackageRepository;
			this.scriptFactory = scriptFactory;
		}
		
		public void Run(IPackageScriptSession session)
		{
			foreach (IPackageScript script in GetScripts()) {
				script.Run(session);
			}			
		}
		
		List<IPackageScript> GetScripts()
		{
			if (scripts == null) {
				CreatePackageInitializationScripts();
			}
			return scripts;
		}
		
		void CreatePackageInitializationScripts()
		{
			scripts = new List<IPackageScript>();
			foreach (IPackage package in GetPackages()) {
				IPackageScript script = CreateInitializeScript(package);
				if (script.Exists()) {
					scripts.Add(script);
				}
			}
		}
		
		IEnumerable<IPackage> GetPackages()
		{
			return solutionPackageRepository.GetPackagesByDependencyOrder();
		}
		
		IPackageScript CreateInitializeScript(IPackage package)
		{
			string packageInstallDirectory = solutionPackageRepository.GetInstallPath(package);
			IPackageScript script = scriptFactory.CreatePackageInitializeScript(packageInstallDirectory);
			script.Package = package;
			return script;
		}
		
		public bool Any()
		{
			List<IPackageScript> scripts = GetScripts();
			return scripts.Count > 0;
		}
	}
}
