// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PythonBinding;
using IronPython.Hosting;
using NUnit.Framework;

namespace PythonBinding.Tests.Expressions
{
	[TestFixture]
	public class ParseImportSystemConsoleExpressionTestFixture
	{
		PythonImportExpression importExpression;
		
		[SetUp]
		public void Init()
		{
			string text = "import System.Console";
			importExpression = new PythonImportExpression(Python.CreateEngine(), text);
		}
		
		[Test]
		public void ModuleNameIsSystem()
		{
			Assert.AreEqual("System.Console", importExpression.Module);
		}
		
		[Test]
		public void HasFromAndImportIsFalse()
		{
			Assert.IsFalse(importExpression.HasFromAndImport);
		}
	}
}
