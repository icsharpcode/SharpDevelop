// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.PythonBinding;
using IronPython.Hosting;
using NUnit.Framework;

namespace PythonBinding.Tests.Expressions
{
	[TestFixture]
	public class FromSystemImportTestFixture
	{
		PythonImportExpression expression;
		
		[SetUp]
		public void Init()
		{
			string text = "from System import ";
			expression = new PythonImportExpression(Python.CreateEngine(), text);
		}
		
		[Test]
		public void HasFromImportAndImportReturnsTrue()
		{
			Assert.IsTrue(expression.HasFromAndImport);
		}
		
		[Test]
		public void ModuleNameIsSystem()
		{
			Assert.AreEqual("System", expression.Module);
		}
	}
}
