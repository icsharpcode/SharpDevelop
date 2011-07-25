// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class SharpDevelopProjectManager : ProjectManager, ISharpDevelopProjectManager
	{
		public SharpDevelopProjectManager(
			IPackageRepository sourceRepository,
			IPackagePathResolver pathResolver,
			IProjectSystem project,
			IPackageRepository localRepository)
			: base(sourceRepository, pathResolver, project, localRepository)
		{
		}
		
		public bool IsInstalled(string packageId)
		{
			return LocalRepository.Exists(packageId);
		}
	}
}
