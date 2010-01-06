// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
