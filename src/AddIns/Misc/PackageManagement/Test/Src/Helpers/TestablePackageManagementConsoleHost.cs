// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Threading;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.PackageManagement.Scripting;
using ICSharpCode.Scripting.Tests.Utils;

namespace PackageManagement.Tests.Helpers
{
	public class TestablePackageManagementConsoleHost : PackageManagementConsoleHost
	{
		public FakeScriptingConsoleWithLinesToRead FakeScriptingConsole;
		public FakeThread FakeThread = new FakeThread();
		public ThreadStart ThreadStartPassedToCreateThread;
		public FakePowerShellHostFactory FakePowerShellHostFactory;
		public FakePackageManagementAddInPath FakePackageManagementAddInPath;
		public FakePackageManagementProjectService FakeProjectService;
		
		public TestablePackageManagementConsoleHost()
			: this(
				new FakeScriptingConsoleWithLinesToRead(),
				new FakePowerShellHostFactory(),
				new FakePackageManagementProjectService(),
				new FakePackageManagementAddInPath())
		{
		}
		
		public TestablePackageManagementConsoleHost(
			FakeScriptingConsoleWithLinesToRead scriptingConsole,
			FakePowerShellHostFactory powerShellHostFactory,
			FakePackageManagementProjectService projectService,
			FakePackageManagementAddInPath addinPath)
			: base(powerShellHostFactory, projectService, addinPath)
		{
			this.FakeScriptingConsole = scriptingConsole;
			this.ScriptingConsole = scriptingConsole;
			this.FakePowerShellHostFactory = powerShellHostFactory;
			this.FakeProjectService = projectService;
			this.FakePackageManagementAddInPath = addinPath;
		}
		
		protected override IThread CreateThread(ThreadStart threadStart)
		{
			ThreadStartPassedToCreateThread = threadStart;
			return FakeThread;
		}
		
		public Version VersionToReturnFromGetNuGetVersion = new Version("1.2");
		
		protected override Version GetNuGetVersion()
		{
			return VersionToReturnFromGetNuGetVersion;
		}
		
		public string TextToReturnFromGetHelpInfo = String.Empty;
		
		protected override string GetHelpInfo()
		{
			return TextToReturnFromGetHelpInfo;
		}
	}
}
