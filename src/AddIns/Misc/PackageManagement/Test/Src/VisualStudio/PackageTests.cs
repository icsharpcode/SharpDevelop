// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.PackageManagement.EnvDTE;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.ExtensionManager;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using NuGetConsole;
using NUnit.Framework;

namespace PackageManagement.Tests.VisualStudio
{
	[TestFixture]
	public class PackageTests
	{
		[TestFixtureSetUp]
		public void InitFixture()
		{
			if (!PropertyService.Initialized) {
				PropertyService.InitializeService(String.Empty, String.Empty, String.Empty);
			}
		}
		
		[Test]
		public void GetGlobalService_GetExtensionManagerService_ReturnsExtensionManager()
		{
			object extensionManager = Package.GetGlobalService(typeof(SVsExtensionManager)) as SVsExtensionManager;
			
			Assert.IsInstanceOf(typeof(SVsExtensionManager), extensionManager);
		}
		
		[Test]
		public void GetGlobalService_GetDTE_ReturnsDTE()
		{
			object dte = Package.GetGlobalService(typeof(global::EnvDTE.DTE)) as DTE;
			
			Assert.IsInstanceOf(typeof(DTE), dte);
		}
		
		[Test]
		public void GetGlobalService_UnknownType_ReturnsNull()
		{
			object instance = Package.GetGlobalService(typeof(PackageTests));
			
			Assert.IsNull(instance);
		}
		
		[Test]
		public void GetGlobalService_GetIVsSolution_ReturnsIVsSolution()
		{
			object solution = Package.GetGlobalService(typeof(IVsSolution)) as IVsSolution;
			
			Assert.IsInstanceOf(typeof(IVsSolution), solution);
		}
		
		[Test]
		public void GetGlobalService_GetSComponentModel_ReturnsSComponentModel()
		{
			object model = Package.GetGlobalService(typeof(SComponentModel)) as SComponentModel;
			
			Assert.IsInstanceOf(typeof(SComponentModel), model);
		}
		
		[Test]
		public void GetGlobalService_GetSComponentModel_ReturnsIComponentModel()
		{
			object model = Package.GetGlobalService(typeof(SComponentModel)) as IComponentModel;
			
			Assert.IsInstanceOf(typeof(IComponentModel), model);
		}
	}
}
