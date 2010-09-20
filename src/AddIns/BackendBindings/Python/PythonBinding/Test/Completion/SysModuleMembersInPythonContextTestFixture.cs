// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using IronPython.Hosting;
using IronPython.Modules;
using IronPython.Runtime;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Hosting.Providers;
using Microsoft.Scripting.Runtime;
using NUnit.Framework;

namespace PythonBinding.Tests.Completion
{
	[TestFixture]
	public class SysModuleMembersInPythonContextTestFixture
	{
		IList<string> membersFromSystemState;
		object sysModuleFromBuiltinModuleDictionaryObject;
		
		[SetUp]
		public void Init()
		{
			ScriptEngine engine = Python.CreateEngine();
			PythonContext context = HostingHelpers.GetLanguageContext(engine) as PythonContext;
			
			membersFromSystemState = context.GetMemberNames(context.SystemState);
			
			sysModuleFromBuiltinModuleDictionaryObject = null;
			context.BuiltinModuleDict.TryGetValue("sys", out sysModuleFromBuiltinModuleDictionaryObject);
		}
		
		[Test]
		public void ExitMethodIsMemberOfSystemState()
		{
			Assert.IsTrue(membersFromSystemState.Contains("exit"));
		}
		
		[Test]
		public void SysModuleFromBuiltinModuleDictionary()
		{
			Assert.IsNull(sysModuleFromBuiltinModuleDictionaryObject);
		}
		
		[Test]
		public void PathIsMemberOfSystemState()
		{
			Assert.IsTrue(membersFromSystemState.Contains("path"));
		}
	}
}
