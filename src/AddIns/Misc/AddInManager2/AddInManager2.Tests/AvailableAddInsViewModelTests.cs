// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.AddInManager2.Tests
{
	public class AvailableAddInsViewModelTests
	{
		FakeAddInManagerServices _services;
		
		public AvailableAddInsViewModelTests()
		{
		}
		
		[SetUp]
		public void SetUp()
		{
			_services = new FakeAddInManagerServices();
			_services.Events = new AddInManagerEvents();
			_services.Setup = new FakeAddInSetup();
			_services.Settings = new FakeAddInManagerSettings();
			_services.SDAddInManagement = new FakeSDAddInManagement();
			_services.Repositories = new FakePackageRepositories();
			_services.NuGet = new FakeNuGetPackageManager();
		}
	}
}
