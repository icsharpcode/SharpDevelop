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
	public class CodeInterfaceTests : CodeModelTestBase
	{
		CodeInterface codeInterface;
		
		void CreateInterface(string code)
		{
			AddCodeFile("interface.cs", code);
			ITypeDefinition typeDefinition = assemblyModel.TopLevelTypeDefinitions.First().Resolve();
			codeInterface = new CodeInterface(codeModelContext, typeDefinition);
		}
		
		[Test]
		public void Kind_Interface_ReturnsInterface()
		{
			CreateInterface("interface MyInterface {}");
			
			global::EnvDTE.vsCMElement kind = codeInterface.Kind;
			
			Assert.AreEqual(global::EnvDTE.vsCMElement.vsCMElementInterface, kind);
		}
	}
}
