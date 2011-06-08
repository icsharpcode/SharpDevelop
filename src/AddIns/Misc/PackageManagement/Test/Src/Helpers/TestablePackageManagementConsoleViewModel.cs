// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.PackageManagement.Scripting;
using ICSharpCode.Scripting;
using ICSharpCode.Scripting.Tests.Utils;
using NuGet;

namespace PackageManagement.Tests.Helpers
{
	public class TestablePackageManagementConsoleViewModel : PackageManagementConsoleViewModel
	{
		public PackageManagementConsole FakeConsole = 
			new PackageManagementConsole(new FakeScriptingConsole(), new FakeControlDispatcher());
		
		public RegisteredPackageSources RegisteredPackageSources;
		public FakePackageManagementProjectService FakeProjectService;
		
		public TestablePackageManagementConsoleViewModel(IPackageManagementConsoleHost consoleHost)
			: this(new RegisteredPackageSources(new PackageSource[0]), consoleHost)
		{
		}
		
		public TestablePackageManagementConsoleViewModel(
			IEnumerable<PackageSource> packageSources,
			IPackageManagementConsoleHost consoleHost)
			: this(new RegisteredPackageSources(packageSources), consoleHost, new FakePackageManagementProjectService())
		{
		}
		
		public TestablePackageManagementConsoleViewModel(
			IPackageManagementConsoleHost consoleHost,
			FakePackageManagementProjectService projectService)
			: this(new RegisteredPackageSources(new PackageSource[0]), consoleHost, projectService)
		{
		}
		
		public TestablePackageManagementConsoleViewModel(
			RegisteredPackageSources registeredPackageSources,
			IPackageManagementConsoleHost consoleHost,
			FakePackageManagementProjectService projectService)
			: base(registeredPackageSources, projectService, consoleHost)
		{
			this.RegisteredPackageSources = registeredPackageSources;
			this.FakeProjectService = projectService;
		}
		
		protected override PackageManagementConsole CreateConsole()
		{
			return FakeConsole;
		}
	}
}
