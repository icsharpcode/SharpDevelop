// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.PackageManagement.EnvDTE;
using NUnit.Framework;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class CodeStructTests : CodeModelTestBase
	{
		CodeStruct codeStruct;
		
		void CreateStruct(string code)
		{
			AddCodeFile("class.cs", code);
			ITypeDefinition typeDefinition = assemblyModel
				.TopLevelTypeDefinitions
				.First()
				.Resolve();
			
			codeStruct = new CodeStruct(codeModelContext, typeDefinition);
		}
		
		[Test]
		public void Access_PublicStruct_ReturnsPublic()
		{
			CreateStruct("public struct MyStruct {}");
			
			global::EnvDTE.vsCMAccess access = codeStruct.Access;
			
			Assert.AreEqual(global::EnvDTE.vsCMAccess.vsCMAccessPublic, access);
		}
		
		
		[Test]
		public void Access_PrivateStruct_ReturnsPrivate()
		{
			CreateStruct("struct MyStruct {}");
			
			global::EnvDTE.vsCMAccess access = codeStruct.Access;
			
			Assert.AreEqual(global::EnvDTE.vsCMAccess.vsCMAccessPrivate, access);
		}
		
		[Test]
		public void Kind_PublicStruct_ReturnsStruct()
		{
			CreateStruct("public struct MyStruct {}");
			
			global::EnvDTE.vsCMElement kind = codeStruct.Kind;
			
			Assert.AreEqual(global::EnvDTE.vsCMElement.vsCMElementStruct, kind);
		}
	}
}
