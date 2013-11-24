//// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
//// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
//
//using System;
//using ICSharpCode.PackageManagement.EnvDTE;
//using ICSharpCode.SharpDevelop.Dom;
//using NUnit.Framework;
//using PackageManagement.Tests.Helpers;
//
//namespace PackageManagement.Tests.EnvDTE
//{
//	[TestFixture]
//	public class CodeInterfaceTests : CodeModelTestBase
//	{
//		CodeInterface codeInterface;
//		
//		void CreateInterface(string code)
//		{
//			AddCodeFile("interface.cs", code);
//			ITypeDefinitionModel typeModel = assemblyModel.TopLevelTypeDefinitions.Single();
//			codeInterface = new CodeInterface(codeModelContext, typeModel);
//		}
//		
//		[Test]
//		public void Kind_Interface_ReturnsInterface()
//		{
//			CreateInterface("interface MyInterface {}");
//			
//			global::EnvDTE.vsCMElement kind = codeInterface.Kind;
//			
//			Assert.AreEqual(global::EnvDTE.vsCMElement.vsCMElementInterface, kind);
//		}
//	}
//}
