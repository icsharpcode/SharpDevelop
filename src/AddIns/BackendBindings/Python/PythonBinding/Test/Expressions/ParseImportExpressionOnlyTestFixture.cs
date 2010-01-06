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
	public class ParseImportExpressionOnlyTestFixture
	{
		PythonImportExpression importExpression;
		
		[SetUp]
		public void Init()
		{
			string text = "import";
			importExpression = new PythonImportExpression(Python.CreateEngine(), text);
		}
		
		[Test]
		public void ModuleNameIsEmptyString()
		{
			Assert.AreEqual(String.Empty, importExpression.Module);
		}
		
		[Test]
		public void HasFromAndImportIsFalse()
		{
			Assert.IsFalse(importExpression.HasFromAndImport);
		}
	}
}
