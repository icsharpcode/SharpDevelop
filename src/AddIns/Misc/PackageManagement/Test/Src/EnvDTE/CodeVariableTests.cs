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
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.PackageManagement.EnvDTE;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;
using Rhino.Mocks;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class CodeVariableTests : CodeModelTestBase
	{
		CodeVariable codeVariable;
		
		void CreateCodeVariable(string code)
		{
			AddCodeFile("class.cs", code);
			ITypeDefinition typeDefinition = assemblyModel.TopLevelTypeDefinitions.Single().Resolve();
			codeVariable = new CodeVariable(codeModelContext, typeDefinition.Fields.First());
		}
		
		[Test]
		public void Access_PublicVariable_ReturnsPublic()
		{
			CreateCodeVariable(
				"public class MyClass {\r\n" +
				"    public int MyVariable;\r\n" +
				"}");
			
			global::EnvDTE.vsCMAccess access = codeVariable.Access;
			
			Assert.AreEqual(global::EnvDTE.vsCMAccess.vsCMAccessPublic, access);
		}
		
		[Test]
		public void Access_PrivateVariable_ReturnsPrivate()
		{
			CreateCodeVariable(
				"public class MyClass {\r\n" +
				"    private int MyVariable;\r\n" +
				"}");
			
			global::EnvDTE.vsCMAccess access = codeVariable.Access;
			
			Assert.AreEqual(global::EnvDTE.vsCMAccess.vsCMAccessPrivate, access);
		}
		
		[Test]
		public void GetStartPoint_VariableStartsAtColumnOneOnLineTwo_ReturnsTextPointWithLineCharOffsetOneAndLineTwo()
		{
			CreateCodeVariable(
				"public class Foo {\r\n" +
				"int V;\r\n" +
				"}");
			
			global::EnvDTE.TextPoint point = codeVariable.GetStartPoint();
			
			Assert.AreEqual(1, point.LineCharOffset);
			Assert.AreEqual(2, point.Line);
		}
		
		[Test]
		public void GetEndPoint_VariableEndsAtColumnSevenOnLineTwo_ReturnsTextPointWithLineCharOffsetSevenOnLineTwo()
		{
			CreateCodeVariable(
				"public class Foo {\r\n" +
				"int V;\r\n" +
				"}");
			
			global::EnvDTE.TextPoint point = codeVariable.GetEndPoint();
			
			Assert.AreEqual(7, point.LineCharOffset);
			Assert.AreEqual(2, point.Line);
		}
		
		[Test]
		public void Kind_PublicVariable_ReturnsVariable()
		{
			CreateCodeVariable(
				"public class MyClass {\r\n" +
				"    private int MyVariable;\r\n" +
				"}");
			
			global::EnvDTE.vsCMElement kind = codeVariable.Kind;
			
			Assert.AreEqual(global::EnvDTE.vsCMElement.vsCMElementVariable, kind);
		}
		
		[Test]
		public void GetEndPoint_VariableHasSpaceBeforeSemiColon_ReturnsTextPointThatIncludesSemiColon()
		{
			CreateCodeVariable(
				"public class Foo {\r\n" +
				"int V ;\r\n" +
				"}");
			
			global::EnvDTE.TextPoint point = codeVariable.GetEndPoint();
			
			Assert.AreEqual(8, point.LineCharOffset);
		}
	}
}
