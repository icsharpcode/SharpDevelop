// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PythonBinding;
using IronPython.Compiler.Ast;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Resolver
{
	[TestFixture]
	public class PythonLocalVariableResolverTests
	{
		string typeName;
		
		void Resolve(string variableName, string code)
		{
			PythonLocalVariableResolver resolver = new PythonLocalVariableResolver();
			typeName = resolver.Resolve(variableName, code);
		}
		
		[Test]
		public void Resolve_InstanceCreatedInCode_ReturnsInstanceType()
		{
			string code = "a = Class1()";
			Resolve("a", code);
			
			Assert.AreEqual("Class1", typeName);
		}

		/// <summary>
		/// Tests that the NameExpression in the resolver is reset so the second assignment
		/// does not override the first.
		/// </summary>
		[Test]
		public void Resolve_TwoInstancesCreatedInCode_ReturnsFirstInstanceType()
		{
			string code = 
				"a = Class1()\r\n" +
				"b = Class2()";
			
			Resolve("a", code);
			
			Assert.AreEqual("Class1", typeName);
		}
		
		[Test]
		public void Resolve_VariableIsAssignedToString_ReturnsNull()
		{
			string code = "a = \"test\"";
			Resolve("a", code);
			
			Assert.IsNull(typeName);
		}
		
		[Test]
		public void Resolve_CodeIsNull_ReturnsNull()
		{
			Resolve("a", null);
			Assert.IsNull(typeName);
		}
		
		[Test]
		public void Resolve_InstanceCreatedWithNamespace_ReturnsFullyQualifiedTypeName()
		{
			string code = "a = Test.Class1()";
			Resolve("a", code);
			Assert.AreEqual("Test.Class1", typeName);
		}
		
		[Test]
		public void Resolve_InstanceCreatedWithTwoPartsToNamespace_ReturnsFullyQualifiedTypeName()
		{
			string code = "a = Root.Test.Class1()";
			Resolve("a", code);
			Assert.AreEqual("Root.Test.Class1", typeName);
		}
		
		[Test]
		public void GetTypeName_ExpressionIsNotNameOrMemberExpression_ReturnsEmptyStringAndDoesNotGetStuckInInfiniteLoop()
		{
			AssignmentStatement statement = PythonParserHelper.GetAssignmentStatement("a = 2");
			Expression expression = statement.Right;
			string typeName = PythonLocalVariableResolver.GetTypeName(expression);
			Assert.AreEqual(String.Empty, typeName);
		}
	}
}
