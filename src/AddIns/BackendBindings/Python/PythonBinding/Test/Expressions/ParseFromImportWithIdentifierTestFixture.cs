// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Expressions
{
	[TestFixture]
	public class ParseFromImportWithIdentifierTestFixture
	{
		PythonImportExpression importExpression;
		
		[SetUp]
		public void Init()
		{
			string code = "from System import Console";
			importExpression = new PythonImportExpression(code);
		}
		
		[Test]
		public void HasImportAndFromReturnsTrue()
		{
			Assert.IsTrue(importExpression.HasFromAndImport);
		}
		
		[Test]
		public void ImportIdentifierIsConsole()
		{
			Assert.AreEqual("Console", importExpression.Identifier);
		}
		
		[Test]
		public void HasIdentifierReturnsTrue()
		{
			Assert.IsTrue(importExpression.HasIdentifier);
		}
	}
}
