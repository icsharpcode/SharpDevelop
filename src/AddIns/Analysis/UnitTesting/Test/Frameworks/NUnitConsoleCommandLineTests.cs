// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Frameworks
{
	[TestFixture]
	public class NUnitConsoleCommandLineTests
	{
		CompilableProject project;
		
		[SetUp]
		public void SetUp()
		{
			project = new MockCSharpProject();
			project.FileName = @"C:\Projects\MyTests\MyTests.csproj";
			project.AssemblyName = "MyTests";
			project.OutputType = OutputType.Library;
			project.SetProperty("OutputPath", null);
		}
		
		[Test]
		public void TestResultsFile()
		{
			SelectedTests selectedTests = new SelectedTests(project);
			NUnitConsoleApplication app = new NUnitConsoleApplication(selectedTests);
			app.NoLogo = false;
			app.ShadowCopy = true;
			app.Results = @"C:\results.txt";
			
			string expectedCommandLine = "\"C:\\Projects\\MyTests\\MyTests.dll\" /results=\"C:\\results.txt\"";
			Assert.AreEqual(expectedCommandLine, app.GetArguments());
		}
		
		[Test]
		public void NoLogo()
		{
			SelectedTests selectedTests = new SelectedTests(project);
			NUnitConsoleApplication app = new NUnitConsoleApplication(selectedTests);
			app.NoLogo = true;
			app.ShadowCopy = true;
			
			string expectedCommandLine = "\"C:\\Projects\\MyTests\\MyTests.dll\" /nologo";
			Assert.AreEqual(expectedCommandLine, app.GetArguments());
		}
		
		[Test]
		public void NoShadowCopy()
		{
			SelectedTests selectedTests = new SelectedTests(project);
			NUnitConsoleApplication app = new NUnitConsoleApplication(selectedTests);
			app.NoLogo = false;
			app.ShadowCopy = false;
			
			string expectedCommandLine = "\"C:\\Projects\\MyTests\\MyTests.dll\" /noshadow";
			Assert.AreEqual(expectedCommandLine, app.GetArguments());
		}
		
		[Test]
		public void NoThread()
		{
			SelectedTests selectedTests = new SelectedTests(project);
			NUnitConsoleApplication app = new NUnitConsoleApplication(selectedTests);
			app.NoLogo = false;
			app.ShadowCopy = true;
			app.NoThread = true;
			
			string expectedCommandLine = "\"C:\\Projects\\MyTests\\MyTests.dll\" /nothread";
			Assert.AreEqual(expectedCommandLine, app.GetArguments());
		}
		
		[Test]
		public void NoDots()
		{
			SelectedTests selectedTests = new SelectedTests(project);
			NUnitConsoleApplication app = new NUnitConsoleApplication(selectedTests);
			app.NoLogo = false;
			app.ShadowCopy = true;
			app.NoDots = true;
			
			string expectedCommandLine = "\"C:\\Projects\\MyTests\\MyTests.dll\" /nodots";
			Assert.AreEqual(expectedCommandLine, app.GetArguments());
		}		

		[Test]
		public void Labels()
		{
			SelectedTests selectedTests = new SelectedTests(project);
			NUnitConsoleApplication app = new NUnitConsoleApplication(selectedTests);
			app.NoLogo = false;
			app.ShadowCopy = true;
			app.Labels = true;
			
			string expectedCommandLine = "\"C:\\Projects\\MyTests\\MyTests.dll\" /labels";
			Assert.AreEqual(expectedCommandLine, app.GetArguments());
		}
		
		[Test]
		public void TestFixture()
		{
			SelectedTests selectedTests = new SelectedTests(project);
			NUnitConsoleApplication app = new NUnitConsoleApplication(selectedTests);
			app.NoLogo = false;
			app.ShadowCopy = true;
			app.Fixture = "TestFixture";
			
			string expectedCommandLine = "\"C:\\Projects\\MyTests\\MyTests.dll\" /run=\"TestFixture\"";
			Assert.AreEqual(expectedCommandLine, app.GetArguments());
		}

		[Test]
		public void TestNamespace()
		{
			SelectedTests selectedTests = new SelectedTests(project);
			NUnitConsoleApplication app = new NUnitConsoleApplication(selectedTests);
			app.NoLogo = false;
			app.ShadowCopy = true;
			app.NamespaceFilter = "TestFixture";
			
			string expectedCommandLine = "\"C:\\Projects\\MyTests\\MyTests.dll\" /run=\"TestFixture\"";
			Assert.AreEqual(expectedCommandLine, app.GetArguments());
		}
		
		[Test]
		public void XmlOutputFile()
		{
			SelectedTests selectedTests = new SelectedTests(project);
			NUnitConsoleApplication app = new NUnitConsoleApplication(selectedTests);
			app.NoLogo = false;
			app.ShadowCopy = true;
			app.XmlOutputFile = @"C:\NUnit.xml";
			
			string expectedCommandLine = "\"C:\\Projects\\MyTests\\MyTests.dll\" /xml=\"C:\\NUnit.xml\"";
			Assert.AreEqual(expectedCommandLine, app.GetArguments());
		}
		
		[Test]
		public void TestMethod()
		{
			SelectedTests selectedTests = new SelectedTests(project);
			NUnitConsoleApplication app = new NUnitConsoleApplication(selectedTests);
			app.NoLogo = false;
			app.ShadowCopy = true;
			app.Fixture = "TestFixture";
			app.Test = "Test";
			
			string expectedCommandLine = "\"C:\\Projects\\MyTests\\MyTests.dll\" /run=\"TestFixture.Test\"";
			Assert.AreEqual(expectedCommandLine, app.GetArguments());
		}
		
		[Test]
		public void TestMethodSpecifiedInInitialize()
		{
			MockClass testFixture = new MockClass("TestFixture");
			MockMethod testMethod = new MockMethod(testFixture, "Test");
			SelectedTests selectedTests = new SelectedTests(project, null, testFixture, testMethod);
			NUnitConsoleApplication app = new NUnitConsoleApplication(selectedTests);
			app.NoLogo = false;
			app.ShadowCopy = true;
			
			string expectedCommandLine = "\"C:\\Projects\\MyTests\\MyTests.dll\" /run=\"TestFixture.Test\"";
			Assert.AreEqual(expectedCommandLine, app.GetArguments());
		}
		
		[Test]
		public void TestNamespaceSpecifiedInInitialize()
		{
			SelectedTests selectedTests = new SelectedTests(project, "Project.MyTests", null, null);
			NUnitConsoleApplication app = new NUnitConsoleApplication(selectedTests);
			app.NoLogo = false;
			app.ShadowCopy = true;
			
			string expectedCommandLine = "\"C:\\Projects\\MyTests\\MyTests.dll\" /run=\"Project.MyTests\"";
			Assert.AreEqual(expectedCommandLine, app.GetArguments());
		}
		
		[Test]
		public void FullCommandLine()
		{
			SelectedTests selectedTests = new SelectedTests(project);
			NUnitConsoleApplication app = new NUnitConsoleApplication(selectedTests);
			app.NoLogo = true;
			app.ShadowCopy = true;
			
			FileUtility.ApplicationRootPath = @"C:\SharpDevelop";
			
			string expectedFullCommandLine =
				"\"C:\\SharpDevelop\\bin\\Tools\\NUnit\\nunit-console-x86.exe\" " +
				"\"C:\\Projects\\MyTests\\MyTests.dll\" " +
				"/nologo";
			Assert.AreEqual(expectedFullCommandLine, app.GetCommandLine());
		}
		
		/// <summary>
		/// Tests that a space is appended between the items added
		/// to the UnitTestApplicationStartapp.Assemblies
		/// when the command line is generated.
		/// </summary>
		[Test]
		public void SecondAssemblySpecified()
		{
			SelectedTests selectedTests = new SelectedTests(project);
			NUnitConsoleApplication app = new NUnitConsoleApplication(selectedTests);
			app.Assemblies.Add("SecondAssembly.dll");
			app.NoLogo = false;
			app.ShadowCopy = true;
			app.Results = @"C:\results.txt";
			
			string expectedCommandLine = 
				"\"C:\\Projects\\MyTests\\MyTests.dll\" " +
				"\"SecondAssembly.dll\" " +
				"/results=\"C:\\results.txt\"";
			Assert.AreEqual(expectedCommandLine, app.GetArguments());
		}
		
		[Test]
		public void GetProject()
		{
			SelectedTests selectedTests = new SelectedTests(project);
			NUnitConsoleApplication app = new NUnitConsoleApplication(selectedTests);
			Assert.AreSame(project, app.Project);
		}
		
		[Test]
		public void TestInnerClassSpecifiedInInitialize()
		{
			MockClass testFixture = new MockClass("MyTests.TestFixture.InnerTest", "MyTests.TestFixture+InnerTest");
			SelectedTests selectedTests = new SelectedTests(project, null, testFixture, null);
			NUnitConsoleApplication app = new NUnitConsoleApplication(selectedTests);
			app.NoLogo = false;
			app.ShadowCopy = true;
			
			string expectedCommandLine = "\"C:\\Projects\\MyTests\\MyTests.dll\" " +
				"/run=\"MyTests.TestFixture+InnerTest\"";
			Assert.AreEqual(expectedCommandLine, app.GetArguments());
		}
		
		[Test]
		public void XmlOutputFileNameSpecifiedOnCommandLine()
		{
			UnitTestingOptions options = new UnitTestingOptions(new Properties());
			options.CreateXmlOutputFile = true;
			MockClass testFixture = new MockClass("MyTests.TestFixture.MyTest");
			SelectedTests selectedTests = new SelectedTests(project, null, testFixture, null);
			NUnitConsoleApplication app = new NUnitConsoleApplication(selectedTests, options);
			app.NoLogo = false;
			app.ShadowCopy = true;
			
			string expectedCommandLine = 
				"\"C:\\Projects\\MyTests\\MyTests.dll\" " +
				"/xml=\"C:\\Projects\\MyTests\\MyTests-TestResult.xml\" " +
				"/run=\"MyTests.TestFixture.MyTest\"";
			Assert.AreEqual(expectedCommandLine, app.GetArguments());
		}
	}
}
