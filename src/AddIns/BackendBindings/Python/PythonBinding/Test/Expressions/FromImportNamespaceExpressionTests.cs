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
	public class FromImportNamespaceExpressionTests
	{
		ScriptEngine engine;
		
		[SetUp]
		public void Init()
		{
			engine = Python.CreateEngine();
		}
		
		[Test]
		public void FromModuleNameIsSystemForFromSystemExpression()
		{
			PythonImportExpression expression = new PythonImportExpression(engine, "from System");
			Assert.AreEqual("System", expression.Module);
			Assert.IsFalse(expression.HasFromAndImport);
		}
		
		[Test]
		public void FromModuleNameIsEmptyWhenOnlyFromIsInExpression()
		{
			PythonImportExpression expression = new PythonImportExpression(engine, "from");
			Assert.AreEqual(String.Empty, expression.Module);
			Assert.IsFalse(expression.HasFromAndImport);
		}
		
		[Test]
		public void FromModuleNameIsSystemForFromSystemExpressionWithWhitespaceBetweenImportAndSystem()
		{
			PythonImportExpression expression = new PythonImportExpression(engine, "from  \t  System");
			Assert.AreEqual("System", expression.Module);
			Assert.IsFalse(expression.HasFromAndImport);
		}
		
		[Test]
		public void HasIdentifierReturnsFalseForFromMathImportWithoutIdentfier()
		{
			PythonImportExpression expression = new PythonImportExpression(engine, "from math import");
			Assert.IsFalse(expression.HasIdentifier);
		}
	}
}
