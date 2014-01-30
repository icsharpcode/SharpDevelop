// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
		
		PythonLocalVariableResolver CreateResolver()
		{
			PythonClassResolver classResolver = new PythonClassResolver();
			return new PythonLocalVariableResolver(classResolver);
		}
		
		void Resolve(string variableName, string code)
		{
			PythonLocalVariableResolver resolver = CreateResolver();
			typeName = resolver.Resolve(variableName, code);
		}
		
		[Test]
		public void Resolve_InstanceCreatedInCode_ReturnsInstanceType()
		{
			string code = "a = Class1()";
			Resolve("a", code);
			string expectedTypeName = "Class1";
			
			Assert.AreEqual(expectedTypeName, typeName);
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
			string expectedTypeName = "Class1";
			
			Assert.AreEqual(expectedTypeName, typeName);
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
			string expectedTypeName = "Test.Class1";
			
			Assert.AreEqual(expectedTypeName, typeName);
		}
		
		[Test]
		public void Resolve_InstanceCreatedWithTwoPartsToNamespace_ReturnsFullyQualifiedTypeName()
		{
			string code = "a = Root.Test.Class1()";
			Resolve("a", code);
			string expectedTypeName = "Root.Test.Class1";
			
			Assert.AreEqual(expectedTypeName, typeName);
		}
		
		[Test]
		public void Resolve_AssignmentToPropertyOnLocalVariableThenMethodCallOnLocalVariable_CorrectLocalVariableTypeReturned()
		{
			string code = 
				"connection = OracleClient.OracleConnection()\r\n" +
				"connection.ConnectionString = connectionString\r\n" +
				"connection.Open()\r\n" + 
				"connection";
			
			Resolve("connection", code);
			
			Assert.AreEqual("OracleClient.OracleConnection", typeName);
		}
		
		[Test]
		public void Resolve_CalledTwiceFirstCallResolvesTypeNameSecondCallDoesNotResolve_ReturnsNull()
		{
			string code = "a = Class1()";
			
			PythonLocalVariableResolver resolver = CreateResolver();
			resolver.Resolve("a", code);
			string typeName = resolver.Resolve("b", code);
			
			Assert.IsNull(typeName);
		}
	}
}
