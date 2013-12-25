// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class RemovedPackageReferenceMonitor : IDisposable
	{
		ISharpDevelopProjectManager projectManager;
		List<IPackage> packagesRemoved = new List<IPackage>();
		
		public RemovedPackageReferenceMonitor(ISharpDevelopProjectManager projectManager)
		{
			this.projectManager = projectManager;
			projectManager.PackageReferenceRemoved += PackageReferenceRemoved;
		}
		
		void PackageReferenceRemoved(object sender, PackageOperationEventArgs e)
		{
			packagesRemoved.Add(e.Package);
		}
		
		public void Dispose()
		{
			projectManager.PackageReferenceRemoved -= PackageReferenceRemoved;
		}
		
		public List<IPackage> PackagesRemoved {
			get { return packagesRemoved; }
		}
	}
}
