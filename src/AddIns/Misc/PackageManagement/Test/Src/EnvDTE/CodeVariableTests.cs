// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
