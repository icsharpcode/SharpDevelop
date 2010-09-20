// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
