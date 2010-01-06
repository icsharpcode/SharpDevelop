// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
	public class MathModuleMembersInPythonContextTestFixture
	{
		[Test]
		public void UnableToGetMathModuleFromBuiltinModuleDictionary()
		{
			ScriptEngine engine = Python.CreateEngine();
			PythonContext context = HostingHelpers.GetLanguageContext(engine) as PythonContext;
			
			object mathModuleFromBuiltinModuleDictionaryObject = null;
			context.BuiltinModuleDict.TryGetValue("math", out mathModuleFromBuiltinModuleDictionaryObject);
			Assert.IsNull(mathModuleFromBuiltinModuleDictionaryObject);
		}
	}
}
