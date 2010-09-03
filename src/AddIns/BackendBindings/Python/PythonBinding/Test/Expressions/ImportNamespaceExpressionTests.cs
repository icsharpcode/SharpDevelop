// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PythonBinding;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using NUnit.Framework;

namespace PythonBinding.Tests.Expressions
{
	[TestFixture]
	public class ImportNamespaceExpressionTests
	{
		ScriptEngine engine;
		
		[SetUp]
		public void Init()
		{
			engine = Python.CreateEngine();
		}
		
		[Test]
		public void ModuleNameReturnedIsSystemForImportSystemExpression()
		{
			PythonImportExpression expression = new PythonImportExpression(engine, "import System");
			Assert.AreEqual("System", expression.Module);
		}
		
		[Test]
		public void ModuleNameReturnedIsSystemForImportSystemExpressionWithWhitespaceBetweenImportAndSystem()
		{
			string code = "import  \t  System";
			PythonImportExpression expression = new PythonImportExpression(engine, code);
			Assert.AreEqual("System", expression.Module);
		}
		
		[Test]
		public void ModuleNameIsEmptyStringWhenExpressionIsEmptyString()
		{
			PythonImportExpression expression = new PythonImportExpression(engine, String.Empty);
			Assert.AreEqual(String.Empty, expression.Module);
		}
	}
}
