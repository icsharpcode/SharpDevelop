// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
using NUnit.Framework;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class PackageManagementServiceProviderTests
	{
		PackageManagementServiceProvider serviceProvider;
		
		public void CreateServiceProvider()
		{
			serviceProvider = new PackageManagementServiceProvider();
		}
		
		[Test]
		public void GetService_TypeOfDTE_ReturnsDTE()
		{
			CreateServiceProvider();
			
			object dte = serviceProvider.GetService(typeof(global::EnvDTE.DTE)) as global::EnvDTE.DTE;
			
			Assert.IsInstanceOf(typeof(global::EnvDTE.DTE), dte);
		}
	}
}
