// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.EnvDTE;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class CodeTypeRef2Tests
	{
		CodeTypeRef2 typeRef;
		ReturnTypeHelper helper;
		CodeElement parent;
		ClassHelper classHelper;
		
		[SetUp]
		public void Init()
		{
			helper = new ReturnTypeHelper();
			classHelper = new ClassHelper();
			parent = new CodeElement();
		}
		
		void AddUnderlyingClassToReturnType(string fullyQualifiedName)
		{
			classHelper.CreatePublicClass(fullyQualifiedName);
			helper.AddUnderlyingClass(classHelper.Class);
		}
		
		void CreateCodeTypeRef2()
		{
			typeRef = new CodeTypeRef2(classHelper.ProjectContentHelper.ProjectContent, parent, helper.ReturnType);
		}
		
		void ReturnTypeUsesDifferentProjectContent()
		{
			classHelper = new ClassHelper();
			classHelper.ProjectContentHelper.SetProjectForProjectContent(ProjectHelper.CreateTestProject());
		}
		
		void ReturnTypeSameProjectContent()
		{
			var project = ProjectHelper.CreateTestProject();
			classHelper.ProjectContentHelper.SetProjectForProjectContent(project);
		}
		
		void ProjectContentIsForVisualBasicProject()
		{
			classHelper.ProjectContentHelper.ProjectContentIsForVisualBasicProject();
		}
		
		void ProjectContentIsForCSharpProject()
		{
			classHelper.ProjectContentHelper.ProjectContentIsForCSharpProject();
		}

		[Test]
		public void CodeType_ReturnTypeIsSystemString_ReturnsCodeClass2ForSystemStringType()
		{
			helper.CreateReturnType("System.String");
			AddUnderlyingClassToReturnType("System.String");
			CreateCodeTypeRef2();
			
			CodeClass2 codeClass = typeRef.CodeType as CodeClass2;
			string name = codeClass.FullName;
			
			Assert.AreEqual("System.String", name);
		}
		
		[Test]
		public void CodeType_ReturnTypeFromDifferentProjectContent_CodeTypeLocationIsExternal()
		{
			helper.CreateReturnType("System.String");
			AddUnderlyingClassToReturnType("System.String");
			ReturnTypeUsesDifferentProjectContent();
			CreateCodeTypeRef2();
			
			CodeClass2 codeClass = typeRef.CodeType as CodeClass2;
			vsCMInfoLocation location = codeClass.InfoLocation;
			
			Assert.AreEqual(vsCMInfoLocation.vsCMInfoLocationExternal, location);
		}
		
		[Test]
		public void CodeType_ReturnTypeFromSameProjectContent_CodeTypeLocationIsProject()
		{
			helper.CreateReturnType("MyType");
			AddUnderlyingClassToReturnType("MyType");
			ReturnTypeSameProjectContent();
			CreateCodeTypeRef2();
			
			CodeClass2 codeClass = typeRef.CodeType as CodeClass2;
			vsCMInfoLocation location = codeClass.InfoLocation;
			
			Assert.AreEqual(vsCMInfoLocation.vsCMInfoLocationProject, location);
		}
		
		[Test]
		public void IsGeneric_NotGenericReturnType_ReturnsFalse()
		{
			helper.CreateReturnType("MyType");
			helper.AddDotNetName("MyType");
			CreateCodeTypeRef2();
			
			bool generic = typeRef.IsGeneric;
			
			Assert.IsFalse(generic);
		}
		
		[Test]
		public void IsGeneric_GenericReturnType_ReturnsTrue()
		{
			helper.CreateReturnType("System.Nullable");
			helper.AddDotNetName("System.Nullable{System.String}");
			CreateCodeTypeRef2();
			
			bool generic = typeRef.IsGeneric;
			
			Assert.IsTrue(generic);
		}
		
		[Test]
		public void AsFullName_GenericReturnType_ReturnsDotNetNameWithCurlyBracesReplacedWithAngleBrackets()
		{
			helper.CreateReturnType("System.Nullable");
			helper.AddDotNetName("System.Nullable{System.String}");
			CreateCodeTypeRef2();
			
			string name = typeRef.AsFullName;
			
			Assert.AreEqual("System.Nullable<System.String>", name);
		}
		
		[Test]
		public void AsString_ReturnTypeIsSystemStringInCSharpProject_ReturnsString()
		{
			helper.CreateReturnType("System.String");
			ProjectContentIsForCSharpProject();
			AddUnderlyingClassToReturnType("System.String");
			CreateCodeTypeRef2();
			
			string name = typeRef.AsString;
			
			Assert.AreEqual("string", name);
		}
		
		[Test]
		public void AsString_ReturnTypeIsSystemInt32InCSharpProject_ReturnsInt()
		{
			helper.CreateReturnType("System.Int32");
			AddUnderlyingClassToReturnType("System.Int32");
			ProjectContentIsForCSharpProject();
			CreateCodeTypeRef2();
			
			string name = typeRef.AsString;
			
			Assert.AreEqual("int", name);
		}
		
		[Test]
		public void AsString_ReturnTypeIsSystemInt32InVisualBasicProject_ReturnsInteger()
		{
			helper.CreateReturnType("System.Int32");
			AddUnderlyingClassToReturnType("System.Int32");
			ProjectContentIsForVisualBasicProject();
			CreateCodeTypeRef2();
			
			string name = typeRef.AsString;
			
			Assert.AreEqual("Integer", name);
		}
		
		[Test]
		public void AsString_ReturnTypeIsCustomType_ReturnsFullTypeName()
		{
			helper.CreateReturnType("Test.MyClass");
			AddUnderlyingClassToReturnType("Test.MyClass");
			ProjectContentIsForCSharpProject();
			helper.AddDotNetName("Test.MyClass");
			CreateCodeTypeRef2();
			
			string name = typeRef.AsString;
			
			Assert.AreEqual("Test.MyClass", name);
		}
	}
}
