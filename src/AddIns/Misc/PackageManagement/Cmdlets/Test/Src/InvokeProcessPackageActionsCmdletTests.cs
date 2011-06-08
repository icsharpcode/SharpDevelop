// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.PackageManagement.Scripting;
using NUnit.Framework;
using PackageManagement.Cmdlets.Tests.Helpers;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Cmdlets.Tests
{
	[TestFixture]
	public class InvokeProcessPackageActionsCmdletTests
	{
		TestableInvokeProcessPackageActionsCmdlet cmdlet;
		PackageActionsToRun actionsToRun;		
		
		void CreateCmdlet()
		{
			cmdlet = new TestableInvokeProcessPackageActionsCmdlet();
			actionsToRun = cmdlet.ActionsToRun;
		}
		
		void RunCmdlet()
		{
			cmdlet.CallProcessRecord();
		}
		
		FakeInstallPackageAction AddInstallAction()
		{
			var project = new FakePackageManagementProject();
			var action = new FakeInstallPackageAction(project);
			actionsToRun.AddAction(action);
			return action;
		}
		
		[Test]
		public void ProcessRecord_OnePackageActionToRun_PackageActionIsRun()
		{
			CreateCmdlet();
			FakeInstallPackageAction action = AddInstallAction();
			RunCmdlet();
			
			bool run = action.IsExecuteCalled;
			
			Assert.IsTrue(run);
		}
		
		[Test]
		public void ProcessRecord_TwoPackageActionsToRun_SecondPackageActionIsRun()
		{
			CreateCmdlet();
			AddInstallAction();
			FakeInstallPackageAction action = AddInstallAction();
			RunCmdlet();
			
			bool run = action.IsExecuteCalled;
			
			Assert.IsTrue(run);
		}
		
		[Test]
		public void ProcessRecord_OnePackageActionToRun_PackageActionScriptRunnerIsCmdlet()
		{
			CreateCmdlet();
			FakeInstallPackageAction action = AddInstallAction();
			RunCmdlet();
			
			IPackageScriptRunner runner = action.PackageScriptRunner;
			
			Assert.AreEqual(cmdlet, runner);
		}
		
		[Test]
		public void ProcessRecord_TwoPackageActionsToRun_TwoPackageActionScriptRunnerIsCmdlet()
		{
			CreateCmdlet();
			AddInstallAction();
			FakeInstallPackageAction action = AddInstallAction();
			RunCmdlet();
			
			IPackageScriptRunner runner = action.PackageScriptRunner;
			
			Assert.AreEqual(cmdlet, runner);
		}
		
		[Test]
		public void ProcessRecord_NoPackageActionsToRun_NullReferenceExceptionIsNotThrown()
		{
			CreateCmdlet();
			Assert.DoesNotThrow(() => RunCmdlet());
		}
	}
}
