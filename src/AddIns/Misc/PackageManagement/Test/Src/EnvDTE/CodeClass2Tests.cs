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
	public class CodeClass2Tests
	{
		CodeClass2 codeClass;
		ProjectContentHelper helper;
		IClass fakeClass;
		
		void CreateProjectContent()
		{
			helper = new ProjectContentHelper();
		}
		
		void CreateClass(string name)
		{
			fakeClass = helper.AddClassToProjectContent(name);
			codeClass = new CodeClass2(helper.FakeProjectContent, fakeClass);
		}
		
		void CreateMSBuildClass(string name)
		{
			fakeClass = MockRepository.GenerateStub<IClass>();
			fakeClass.Stub(c => c.FullyQualifiedName).Return(name);
		}
		
		[Test]
		public void Language_CSharpProject_ReturnsCSharpModelLanguage()
		{
			CreateProjectContent();
			helper.ProjectContentIsForCSharpProject();
			CreateClass("MyClass");
			
			string language = codeClass.Language;
			
			Assert.AreEqual(CodeModelLanguageConstants.vsCMLanguageCSharp, language);
		}
		
		[Test]
		public void Language_VisualBasicProject_ReturnsVisualBasicModelLanguage()
		{
			CreateProjectContent();
			helper.ProjectContentIsForVisualBasicProject();
			CreateClass("MyClass");
			
			string language = codeClass.Language;
			
			Assert.AreEqual(CodeModelLanguageConstants.vsCMLanguageVB, language);
		}
	}
}
