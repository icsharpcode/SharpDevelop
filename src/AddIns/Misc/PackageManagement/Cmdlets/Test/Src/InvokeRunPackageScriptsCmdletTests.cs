// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.Scripting;
using NUnit.Framework;
using PackageManagement.Cmdlets.Tests.Helpers;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Cmdlets.Tests
{
	[TestFixture]
	public class InvokeRunPackageScriptsCmdletTests
	{
		TestableInvokeRunPackageScriptsCmdlet cmdlet;
		FakeCmdletTerminatingError fakeTerminatingError;
		PackageScriptsToRun scriptsToRun;
		
		void CreateCmdlet()
		{
			cmdlet = new TestableInvokeRunPackageScriptsCmdlet();
			fakeTerminatingError = cmdlet.FakeCmdletTerminatingError;
			scriptsToRun = cmdlet.ScriptsToBeRun;
		}
		
		void RunCmdlet()
		{
			cmdlet.CallProcessRecord();
		}
		
		FakePackageScript AddScript()
		{
			var script = new FakePackageScript();
			scriptsToRun.AddScript(script);
			return script;
		}
		
		[Test]
		public void ProcessRecord_OnePackageScriptToRun_PackageScriptIsRun()
		{
			CreateCmdlet();
			FakePackageScript script = AddScript();
			RunCmdlet();
			
			bool run = script.IsRun;
			
			Assert.IsTrue(run);
		}
		
		[Test]
		public void ProcessRecord_TwoPackageScriptsToRun_SecondPackageScriptIsRun()
		{
			CreateCmdlet();
			AddScript();
			FakePackageScript script = AddScript();
			RunCmdlet();
			
			bool run = script.IsRun;
			
			Assert.IsTrue(run);
		}
		
		[Test]
		public void ProcessRecord_OnePackageScriptToRun_PackageScriptSessionIsCmdlet()
		{
			CreateCmdlet();
			FakePackageScript script = AddScript();
			RunCmdlet();
			
			IPackageScriptSession session = script.SessionPassedToRun;
			
			Assert.AreEqual(cmdlet, session);
		}
		
		[Test]
		public void ProcessRecord_TwoPackageScriptsToRun_TwoPackageScriptSessionIsCmdlet()
		{
			CreateCmdlet();
			AddScript();
			FakePackageScript script = AddScript();
			RunCmdlet();
			
			IPackageScriptSession session = script.SessionPassedToRun;
			
			Assert.AreEqual(cmdlet, session);
		}
		
		[Test]
		public void ProcessRecord_NoPackageScriptsToRun_NullReferenceExceptionIsNotThrown()
		{
			CreateCmdlet();
			Assert.DoesNotThrow(() => RunCmdlet());
		}
	}
}
