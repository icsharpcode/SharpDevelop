// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using Microsoft.VisualStudio.ExtensionManager;
using NUnit.Framework;

namespace PackageManagement.Tests.VisualStudio
{
	[TestFixture]
	public class SVsExtensionManagerTests
	{
		SVsExtensionManager extensionManager;
		
		void CreateExtensionManager()
		{
			extensionManager = new SVsExtensionManager();
		}
		
		[Test]
		public void GetInstalledExtension_UnknownExtensionIdPassed_ThrowsNotInstalledException()
		{
			CreateExtensionManager();
			
			Assert.Throws<NotInstalledException>(() => extensionManager.GetInstalledExtension("UnknownExtensionId"));
		}
	}
}
