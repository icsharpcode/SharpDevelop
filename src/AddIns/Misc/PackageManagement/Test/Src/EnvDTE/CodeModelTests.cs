// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.EnvDTE;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;
using Rhino.Mocks;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class CodeModelTests
	{
		CodeModel codeModel;
		ProjectContentHelper helper;
		
		void CreateCodeModel()
		{
			helper = new ProjectContentHelper();
			codeModel = new CodeModel(helper.FakeProjectContent);
		}
		
		void AddClassToProjectContent(string className)
		{
			helper.AddClassToProjectContent(className);
		}
		
		void AddInterfaceToProjectContent(string interfaceName)
		{
			helper.AddInterfaceToProjectContent(interfaceName);
		}
		
		[Test]
		public void CodeTypeFromFullName_NoSuchTypeInProject_ReturnsNull()
		{
			CreateCodeModel();
			
			CodeType codeType = codeModel.CodeTypeFromFullName("UnknownType");
			
			Assert.IsNull(codeType);
		}
		
		[Test]
		public void CodeTypeFromFullName_ClassExistsInProject_ReturnsCodeClass2()
		{
			CreateCodeModel();
			AddClassToProjectContent("Tests.TestClass");
			
			var codeClass = codeModel.CodeTypeFromFullName("Tests.TestClass") as CodeClass2;
			
			Assert.AreEqual("Tests.TestClass", codeClass.FullName);
			Assert.AreEqual("TestClass", codeClass.Name);
		}
		
		[Test]
		public void CodeTypeFromFullName_ClassWithoutNamespaceExistsInProject_ReturnsCodeClass2()
		{
			CreateCodeModel();
			AddClassToProjectContent("TestClass");
			
			var codeClass = codeModel.CodeTypeFromFullName("TestClass") as CodeClass2;
			
			Assert.AreEqual("TestClass", codeClass.FullName);
		}
		
		[Test]
		public void CodeTypeFromFullName_InterfaceExistsInProject_ReturnsCodeInterface()
		{
			CreateCodeModel();
			AddInterfaceToProjectContent("Interface1");
			
			var codeInterface = codeModel.CodeTypeFromFullName("Interface1") as CodeInterface;
			
			Assert.AreEqual("Interface1", codeInterface.FullName);
		}
	}
}
