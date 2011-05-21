// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.Scripting;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests.Scripting
{
	[TestFixture]
	public class PackageScriptsToRunTests
	{
		PackageScriptsToRun scriptsToRun;
		
		void CreateScriptsToBeRun()
		{
			scriptsToRun = new PackageScriptsToRun();
		}
		
		FakePackageScript AddScript()
		{
			var script = new FakePackageScript();
			scriptsToRun.AddScript(script);
			return script;
		}
		
		[Test]
		public void GetNextScript_NewInstance_ReturnsNull()
		{
			CreateScriptsToBeRun();
			IPackageScript script = scriptsToRun.GetNextScript();
			
			Assert.IsNull(script);
		}
		
		[Test]
		public void GetNextScript_NewInstance_ReturnsFalse()
		{
			CreateScriptsToBeRun();
			IPackageScript script = null;
			bool result = scriptsToRun.GetNextScript(out script);
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void GetNextScript_OneScriptAdded_ReturnsScript()
		{
			CreateScriptsToBeRun();
			FakePackageScript expectedScript = AddScript();
			
			IPackageScript script = scriptsToRun.GetNextScript();
			
			Assert.AreEqual(expectedScript, script);
		}
		
		[Test]
		public void GetNextScript_OneScriptAdded_ReturnsScriptInOutParameter()
		{
			CreateScriptsToBeRun();
			FakePackageScript expectedScript = AddScript();
			
			IPackageScript script = null;
			scriptsToRun.GetNextScript(out script);
			
			Assert.AreEqual(expectedScript, script);
		}
		
		[Test]
		public void GetNextScript_OneScriptAdded_ReturnsTrue()
		{
			CreateScriptsToBeRun();
			FakePackageScript expectedScript = AddScript();
			
			IPackageScript script = null;
			bool result = scriptsToRun.GetNextScript(out script);
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void GetNextScript_CalledTwiceWithOneScriptAdded_ReturnsNullOnSecondCall()
		{
			CreateScriptsToBeRun();
			FakePackageScript expectedScript = AddScript();
			scriptsToRun.GetNextScript();
			IPackageScript script = scriptsToRun.GetNextScript();
			
			Assert.IsNull(script);
		}
		
		[Test]
		public void GetNextScript_CalledTwiceWithOneScriptAdded_ReturnsNullScriptInOutParameterOnSecondCall()
		{
			CreateScriptsToBeRun();
			FakePackageScript expectedScript = AddScript();
			scriptsToRun.GetNextScript();
			IPackageScript script = null;
			scriptsToRun.GetNextScript(out script);
			
			Assert.IsNull(script);
		}
		
		[Test]
		public void GetNextScript_CalledTwiceWithOneScriptAdded_ReturnsFalseOnSecondCall()
		{
			CreateScriptsToBeRun();
			FakePackageScript expectedScript = AddScript();
			scriptsToRun.GetNextScript();
			
			IPackageScript script = null;
			bool result = scriptsToRun.GetNextScript(out script);
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void GetNextScript_CalledTwiceWithTwoScriptsAdded_ReturnsSecondScriptAdded()
		{
			CreateScriptsToBeRun();
			AddScript();
			FakePackageScript expectedScript = AddScript();
			scriptsToRun.GetNextScript();
			IPackageScript script = scriptsToRun.GetNextScript();
			
			Assert.AreEqual(expectedScript, script);
		}
		
		[Test]
		public void GetNextScript_CalledTwiceWithTwoScriptsAdded_ReturnsSecondScriptAddedInOutParameter()
		{
			CreateScriptsToBeRun();
			AddScript();
			FakePackageScript expectedScript = AddScript();
			scriptsToRun.GetNextScript();
			IPackageScript script = null;
			scriptsToRun.GetNextScript(out script);
			
			Assert.AreEqual(expectedScript, script);
		}
		
		[Test]
		public void GetNextScript_CalledTwiceWithTwoScriptsAdded_ReturnsTrueOnSecondCall()
		{
			CreateScriptsToBeRun();
			AddScript();
			FakePackageScript expectedScript = AddScript();
			scriptsToRun.GetNextScript();
			IPackageScript script = null;
			bool result = scriptsToRun.GetNextScript(out script);
			
			Assert.IsTrue(result);
		}
	}
}
