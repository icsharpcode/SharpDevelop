// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using Microsoft.VisualStudio.ExtensionManager;
using Microsoft.VisualStudio.Shell;
using NUnit.Framework;

namespace PackageManagement.Tests.VisualStudio
{
	[TestFixture]
	public class PackageTests
	{
		[Test]
		public void GetGlobalService_GetExtensionManagerService_ReturnsExtensionManager()
		{
			object extensionManager = Package.GetGlobalService(typeof(SVsExtensionManager)) as SVsExtensionManager;
			
			Assert.IsInstanceOf(typeof(SVsExtensionManager), extensionManager);
		}
	}
}
