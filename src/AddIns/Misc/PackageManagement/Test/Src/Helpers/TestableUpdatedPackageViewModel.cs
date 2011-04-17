// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using NuGet;

namespace PackageManagement.Tests.Helpers
{
	public class TestableUpdatedPackageViewModel : UpdatedPackageViewModel
	{
		public FakePackageOperationResolver FakePackageOperationResolver = new FakePackageOperationResolver();
		public FakePackageManagementSolution FakeSolution;
		public FakePackageManagementEvents FakePackageManagementEvents;
		public FakePackage FakePackage;
		public FakeLogger FakeLogger;
		public ILogger LoggerUsedWhenCreatingPackageResolver;
		
		public TestableUpdatedPackageViewModel()
			: this(new FakePackageManagementSolution())
		{
		}
		
		public TestableUpdatedPackageViewModel(FakePackageManagementSolution solution)
			: this(
				new FakePackage(),
				solution,
				new FakePackageManagementEvents(),
				new FakeLogger())
		{		
		}
		
		public TestableUpdatedPackageViewModel(
			FakePackage package,
			FakePackageManagementSolution solution,
			FakePackageManagementEvents packageManagementEvents,
			FakeLogger logger)
			: base(
				package,
				solution,
				packageManagementEvents,
				logger)
		{
			this.FakePackage = package;
			this.FakeSolution = solution;
			this.FakeLogger = logger;
		}
	}
}
