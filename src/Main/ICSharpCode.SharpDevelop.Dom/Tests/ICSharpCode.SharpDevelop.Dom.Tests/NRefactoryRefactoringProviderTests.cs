// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Alpert" email="david@spinthemoose.com"/>
//     <version>$Revision:  $</version>
// </file>

using System;
using System.Collections.Generic;

using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using NR = ICSharpCode.NRefactory;

using ICSharpCode.SharpDevelop.Tests;

namespace ICSharpCode.SharpDevelop.Dom.Tests
{


	[TestFixture]
	public class NRefactoryRefactoringProviderTests
	{
		NRefactoryResolverTests helper;
		NRefactoryRefactoringProvider csharpRefactoringProvider;
		
		// TODO: write VBNet tests to ensure that Extract Interface works on VB.NET syntax
		NRefactoryRefactoringProvider vbnetRefactoringProvider;
		
		#region Extract Interface
		
		private class ITestClass {
			public const string FileName = "ITestCase.cs";
			public const string FileContent = @"// <file>
//     <copyright see=""prj:///doc/copyright.txt""/>
//     <license see=""prj:///doc/license.txt""/>
//     <owner name=""David Alpert"" email=""david@spinthemoose.com""/>
//     <version>$Revision:  $</version>
// </file>

using System;
using System.Collections.Generics;

namespace ExtractInterface.Tests {
	
	public interface ITestCase {
		string Name {get;}
		string Property {get; set;}
		string Configure {set;}

		IList<int> GetRange(string subject);
		int MultiplyBy5(int x,
                        out bool success);

		event EventHandler<UnhandledExceptionEventArgs> ConfigurationChanged;
	}
}
";
		}
		
		private class TestClass {
			public const string FileName = "TestCase.cs";
			
			// TODO: write TestClass.FileContent to refactor the string literal code blocks
			//       from the following tests.
			public const string FileContent = @"";
		}
		
		#region Sanity Test
		
		[Test]
		/// <summary>
		/// ensures that the custom assertions in this test fixture are working properly
		/// </summary>
		public void SanityCheckTest() {
			ICompilationUnit cu = helper.Parse(ITestClass.FileName, ITestClass.FileContent);
			Assert.That(cu.Classes, Has.Count.EqualTo(1));
			
			IClass c = cu.Classes[0];
			Assert.That(c.ClassType, Is.EqualTo(ClassType.Interface));
			Assert.That(c.BaseTypes, Has.Count.EqualTo(0));
			Assert.That(c.Attributes, Has.Count.EqualTo(0));
			Assert.That(c.Events, Has.Count.EqualTo(1));
			Assert.That(c.Methods, Has.Count.EqualTo(2));
			{
				IMethod m = c.Methods[0];
				Assert.That(m.Name, Is.EqualTo("GetRange"));
				Assert.That(m.Parameters, Has.Count.EqualTo(1));
				Assert.That(m, HasEmpty.MethodBody);
				
				m = c.Methods[1];
				Assert.That(m.Name, Is.EqualTo("MultiplyBy5"));
				Assert.That(m.Parameters, Has.Count.EqualTo(2));
				Assert.That(m, HasEmpty.MethodBody);
			}
			Assert.That(c.Properties, Has.Count.EqualTo(3));
			{
				IProperty p = c.Properties[0];
				Assert.That(p.Name, Is.EqualTo("Name"));
				Assert.That(p.CanGet, Is.True);
				Assert.That(p, HasEmpty.GetRegion);
				Assert.That(p.CanSet, Is.False);
				
				p = c.Properties[1];
				Assert.That(p.Name, Is.EqualTo("Property"));
				Assert.That(p.CanGet, Is.True);
				Assert.That(p, HasEmpty.GetRegion);
				Assert.That(p.CanSet, Is.True);
				Assert.That(p, HasEmpty.SetRegion);
				
				p = c.Properties[2];
				Assert.That(p.Name, Is.EqualTo("Configure"));
				Assert.That(p.CanGet, Is.False);
				Assert.That(p.CanSet, Is.True);
				Assert.That(p, HasEmpty.SetRegion);
			}
		}
		
		#endregion
		
		
		[Test]
		public void GenerateInterfaceFromImplicitPropertyTest() {
			string fileContent = @"
using System;

namespace ExtractInterfaceImplicitPropertyTest {
	public class ClassA {
		string myName;

		public ClassA() {
			string myName = String.Empty;
		}
		
		public string Category {
			get; set;
		}
	}
}
";
			IList<IMember> membersToExtract = new List<IMember>();

			ICompilationUnit cu = helper.Parse(TestClass.FileName, fileContent);
			IClass c = cu.Classes[0];
			IProperty originalProperty = c.Properties[0];
			membersToExtract.Add(originalProperty);
			string interfaceName = "IA";
			string sourceClassName = c.Name;
			string sourceNamespace = c.Namespace;
			
			string interfaceCode = csharpRefactoringProvider.GenerateInterfaceForClass(
				interfaceName, fileContent, membersToExtract, c, false);

			ICompilationUnit icu = helper.Parse(TestClass.FileName, interfaceCode);
			IClass i = icu.Classes[0];
			
			Assert.That(i.ClassType, Is.EqualTo(ClassType.Interface));
			Assert.That(i.Name, Is.EqualTo(interfaceName));
			Assert.That(i.Namespace, Is.EqualTo(c.Namespace));
			Assert.That(i.Methods.Count, Is.EqualTo(0));
			Assert.That(i.Properties.Count, Is.EqualTo(1));
			
			IProperty extractedProperty = i.Properties[0];
			
			Assert.That(extractedProperty.Name, Is.EqualTo(originalProperty.Name));
			Assert.That(extractedProperty.CanGet, Is.True);
			Assert.That(extractedProperty, HasEmpty.GetRegion);
			Assert.That(extractedProperty.CanSet, Is.True);
			Assert.That(extractedProperty, HasEmpty.SetRegion);
		}
		
		[Test]
		[Ignore] // TODO: C# parser seems to have trouble with an implicit property with a private setter?
		public void GenerateInterfaceFromImplicitPropertyWithPrivateSetterTest() {
			string fileContent = @"
using System;

namespace ExtractInterfaceImplicitPropertyTest {
	public class ClassA {
		string myName;

		public ClassA() {
			string myName = String.Empty;
		}
		
		public string Category {
			get; private set;
		}
	}
}
";
			IList<IMember> membersToExtract = new List<IMember>();

			ICompilationUnit cu = helper.Parse(TestClass.FileName, fileContent);
			IClass c = cu.Classes[0];
			IProperty originalProperty = c.Properties[0];
			membersToExtract.Add(originalProperty);
			string interfaceName = "IA";
			string sourceClassName = c.Name;
			string sourceNamespace = c.Namespace;
			
			string interfaceCode = csharpRefactoringProvider.GenerateInterfaceForClass(
				interfaceName, fileContent, membersToExtract, c, false);

			ICompilationUnit icu = helper.Parse(TestClass.FileName, interfaceCode);
			IClass i = icu.Classes[0];
			
			Assert.That(i.ClassType, Is.EqualTo(ClassType.Interface));
			Assert.That(i.Name, Is.EqualTo(interfaceName));
			Assert.That(i.Namespace, Is.EqualTo(c.Namespace));
			Assert.That(i.Methods.Count, Is.EqualTo(0));
			Assert.That(i.Properties.Count, Is.EqualTo(1));
			
			IProperty extractedProperty = i.Properties[0];
			
			Assert.That(extractedProperty.Name, Is.EqualTo(originalProperty.Name));
			Assert.That(extractedProperty.CanGet, Is.True);
			Assert.That(extractedProperty, HasEmpty.GetRegion);
			Assert.That(extractedProperty.CanSet, Is.False);
			Assert.That(extractedProperty, HasEmpty.SetRegion);
		}
		
		[Test]
		public void GenerateInterfaceFromExplicitPropertyTest() {
			string fileContent = @"
using System;

namespace ExtractInterfaceImplicitPropertyTest {
	public class ClassA {
		string myName;

		public ClassA() {
			string myName = String.Empty;
		}
		
		public string Name {
			get{
				return myName;
			}
			set {
				myName = value;
			}
		}
	}
}
";
			IList<IMember> membersToExtract = new List<IMember>();

			ICompilationUnit cu = helper.Parse(TestClass.FileName, fileContent);
			IClass c = cu.Classes[0];
			IProperty originalProperty = c.Properties[0];
			membersToExtract.Add(originalProperty);
			string interfaceName = "IA";
			string sourceClassName = c.Name;
			string sourceNamespace = c.Namespace;
			
			string interfaceCode = csharpRefactoringProvider.GenerateInterfaceForClass(
				interfaceName, fileContent, membersToExtract, c, false);

			ICompilationUnit icu = helper.Parse(ITestClass.FileName, interfaceCode);
			IClass i = icu.Classes[0];
			
			Assert.That(i.ClassType, Is.EqualTo(ClassType.Interface));
			Assert.That(i.Name, Is.EqualTo(interfaceName));
			Assert.That(i.Namespace, Is.EqualTo(c.Namespace));
			Assert.That(i.Methods.Count, Is.EqualTo(0));
			IProperty extractedProperty = i.Properties[0];
			
			Assert.That(extractedProperty.Name, Is.EqualTo(originalProperty.Name));
			Assert.That(extractedProperty.CanGet, Is.True);
			Assert.That(extractedProperty, HasEmpty.GetRegion);
			Assert.That(extractedProperty.CanSet, Is.True);
			Assert.That(extractedProperty, HasEmpty.SetRegion);
		}
		
		[Test]
		public void GenerateInterfaceFromMethodsTest() {
			string fileContent = @"
using System;

namespace ExtractInterfaceImplicitPropertyTest {
	public class ClassA {
		string myName;

		public ClassA() {
			string myName = String.Empty;
		}
		
		public string GetName() {
			return myName;
		}
		
		static public int GetNumber() {
			return -4;
		}
	}
	public interface ITestInterface {
		string GetName();
		int Number {
			get; set;
		}
		int AnotherNumber {
			get;
		}
		int AnInlineNumber {get;set; }
	}
}
";
			IList<IMember> membersToExtract = new List<IMember>();

			ICompilationUnit cu = helper.Parse(TestClass.FileName, fileContent);
			IClass c = cu.Classes[0];
			IMethod originalMethod = c.Methods[1];
			membersToExtract.Add(originalMethod);
			string interfaceName = "IClassA";
			string sourceClassName = c.Name;
			string sourceNamespace = c.Namespace;
			
			string interfaceCode = csharpRefactoringProvider.GenerateInterfaceForClass(
				interfaceName, fileContent, membersToExtract, c, false);

			ICompilationUnit icu = helper.Parse(ITestClass.FileName, interfaceCode);
			IClass i = icu.Classes[0];
			
			Assert.That(i.ClassType, Is.EqualTo(ClassType.Interface));
			Assert.That(i.Name, Is.EqualTo(interfaceName));
			Assert.That(i.Namespace, Is.EqualTo(c.Namespace));
			Assert.That(i.Properties.Count, Is.EqualTo(0));
			Assert.That(i.Methods.Count, Is.EqualTo(1));

			IMethod extractedMethod = i.Methods[0];
			Assert.That(extractedMethod.Name, Is.EqualTo(originalMethod.Name));
			Assert.That(extractedMethod, HasEmpty.MethodBody);
			
		}
		
		[Test]
		[Ignore("This test is not necessary: we shouldn't require that GenerateInterfaceForClass ignores static methods, " +
		        "they cannot be selected as memberToExtract in the UI anyways.")]
		public void GenerateInterfaceWithStaticMethodsTest() {
			string fileContent = @"
using System;

namespace ExtractInterfaceImplicitPropertyTest {
	public class ClassA {
		string myName;

		public ClassA() {
			string myName = String.Empty;
		}
		
		public string GetName() {
			return myName;
		}
		
		static public int GetNumber() {
			return -4;
		}
	}
}
";
			IList<IMember> membersToExtract = new List<IMember>();

			ICompilationUnit cu = helper.Parse(TestClass.FileName, fileContent);
			IClass c = cu.Classes[0];
			
			IMethod originalMethod = c.Methods[1]; // GetName()
			membersToExtract.Add(originalMethod);
			IMethod originalStaticMethod = c.Methods[2]; // GetNumber()
			membersToExtract.Add(originalStaticMethod);
			
			string interfaceName = "IClassA";
			string sourceClassName = c.Name;
			string sourceNamespace = c.Namespace;
			
			string interfaceCode = csharpRefactoringProvider.GenerateInterfaceForClass(
				interfaceName, fileContent, membersToExtract, c, false);

			ICompilationUnit icu = helper.Parse(ITestClass.FileName, interfaceCode);
			IClass i = icu.Classes[0];
			
			Assert.That(i.ClassType, Is.EqualTo(ClassType.Interface));
			Assert.That(i.Name, Is.EqualTo(interfaceName));
			Assert.That(i.Namespace, Is.EqualTo(c.Namespace));
			Assert.That(i.Properties.Count, Is.EqualTo(0));
			Assert.That(i.Methods.Count, Is.EqualTo(1));

			IMethod extractedMethod = i.Methods[0];
			Assert.That(extractedMethod.Name, Is.EqualTo(originalMethod.Name));
			Assert.That(extractedMethod, HasEmpty.MethodBody);
			
		}
		
		
		#endregion
		
		#region FindUnusedUsingDeclarations
		
		[Test]
		public void FindUnusedUsingDeclarationsTest()
		{
			string fileName = "a.cs";
			string fileContent = @"
using System;
using System.Collections;
using System.Collections.Generic;

namespace UnusedUsingDeclarationsTest {
	public class A {
		public A() {
			string testString = String.Empty;
		}
	}
}
";
			ICompilationUnit cu = helper.Parse(TestClass.FileName, fileContent);
			IList<IUsing> unusedUsings = csharpRefactoringProvider.FindUnusedUsingDeclarations(null, fileName, fileContent, cu);
			Assert.That(unusedUsings.Count, Is.EqualTo(2));
			Assert.That(unusedUsings[0].Usings.Count, Is.EqualTo(1));
			Assert.That(unusedUsings[0].Usings[0], Is.EqualTo("System.Collections"));
			Assert.That(unusedUsings[1].Usings.Count, Is.EqualTo(1));
			Assert.That(unusedUsings[1].Usings[0], Is.EqualTo("System.Collections.Generic"));
		}
		
		[Test]
		public void NoUnusedUsingDeclarationsToFindTest()
		{
			string fileName = "a.cs";
			string fileContent = @"
using System;

namespace UnusedUsingDeclarationsTest {
	public class A {
		public A() {
			string testString = String.Empty;
		}
	}
}
";
			ICompilationUnit cu = helper.Parse(fileName, fileContent);
			IList<IUsing> unusedUsings = csharpRefactoringProvider.FindUnusedUsingDeclarations(null, fileName, fileContent, cu);
			Assert.That(unusedUsings.Count, Is.EqualTo(0));
		}
		
		#endregion
		
		#region Test-specific setup/tear down
		
		[SetUp]
		public void TestInit()
		{
			// TODO: Add Test-specific Init code.
		}
		
		[TearDown]
		public void TestDispose()
		{
			// TODO: Add Test-specific tear down code.
		}
		#endregion

		#region Fixture setup/tear down
		
		[TestFixtureSetUp]
		public void Init()
		{
			helper = new NRefactoryResolverTests();
			csharpRefactoringProvider = NRefactoryRefactoringProvider.NRefactoryCSharpProviderInstance;
			vbnetRefactoringProvider  = NRefactoryRefactoringProvider.NRefactoryVBNetProviderInstance;
		}
		
		[TestFixtureTearDown]
		public void Dispose()
		{
			// TODO: Add Fixture tear down code.
		}
		#endregion
	}
}
