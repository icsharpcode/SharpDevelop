// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
