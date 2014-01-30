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
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using NUnit.Framework;
using Rhino.Mocks;
using UnitTesting.Tests.Utils;
using ICSharpCode.SharpDevelop;

namespace UnitTesting.Tests.NUnit
{
	[TestFixture]
	public class NUnitConsoleCommandLineTests : SDTestFixtureBase
	{
		MockCSharpProject project;
		NUnitTestProject testProject;
		
		public override void FixtureSetUp()
		{
			base.FixtureSetUp();
			SD.Services.AddStrictMockService<IProjectService>();
			SD.ProjectService.Stub(p => p.TargetFrameworks).Return(new[] { TargetFramework.Net40Client, TargetFramework.Net40 });
		}
		
		[SetUp]
		public void SetUp()
		{
			project = new MockCSharpProject();
			project.FileName = FileName.Create(@"C:\Projects\MyTests\MyTests.csproj");
			project.AssemblyName = "MyTests";
			project.OutputType = OutputType.Library;
			project.SetProperty("OutputPath", null);
			
			testProject = new NUnitTestProject(project);
		}
		
		[Test]
		public void TestResultsFile()
		{
			NUnitConsoleApplication app = new NUnitConsoleApplication(new[] { testProject });
			app.NoLogo = false;
			app.ShadowCopy = true;
			app.NoXmlOutputFile = false;
			app.ResultsPipe = @"C:\results.txt";
			
			string expectedCommandLine = "\"C:\\Projects\\MyTests\\MyTests.dll\" /pipe=\"C:\\results.txt\"";
			Assert.AreEqual(expectedCommandLine, app.GetArguments());
		}
		
		[Test]
		public void NoLogo()
		{
			NUnitConsoleApplication app = new NUnitConsoleApplication(new[] { testProject });
			app.NoLogo = true;
			app.ShadowCopy = true;
			app.NoXmlOutputFile = false;
			
			string expectedCommandLine = "\"C:\\Projects\\MyTests\\MyTests.dll\" /nologo";
			Assert.AreEqual(expectedCommandLine, app.GetArguments());
		}
		
		[Test]
		public void NoShadowCopy()
		{
			NUnitConsoleApplication app = new NUnitConsoleApplication(new[] { testProject });
			app.NoLogo = false;
			app.ShadowCopy = false;
			app.NoXmlOutputFile = false;
			
			string expectedCommandLine = "\"C:\\Projects\\MyTests\\MyTests.dll\" /noshadow";
			Assert.AreEqual(expectedCommandLine, app.GetArguments());
		}
		
		[Test]
		public void NoThread()
		{
			NUnitConsoleApplication app = new NUnitConsoleApplication(new[] { testProject });
			app.NoLogo = false;
			app.ShadowCopy = true;
			app.NoThread = true;
			app.NoXmlOutputFile = false;
			
			string expectedCommandLine = "\"C:\\Projects\\MyTests\\MyTests.dll\" /nothread";
			Assert.AreEqual(expectedCommandLine, app.GetArguments());
		}
		
		[Test]
		public void NoDots()
		{
			NUnitConsoleApplication app = new NUnitConsoleApplication(new[] { testProject });
			app.NoLogo = false;
			app.ShadowCopy = true;
			app.NoDots = true;
			app.NoXmlOutputFile = false;
			
			string expectedCommandLine = "\"C:\\Projects\\MyTests\\MyTests.dll\" /nodots";
			Assert.AreEqual(expectedCommandLine, app.GetArguments());
		}

		[Test]
		public void Labels()
		{
			NUnitConsoleApplication app = new NUnitConsoleApplication(new[] { testProject });
			app.NoLogo = false;
			app.ShadowCopy = true;
			app.Labels = true;
			app.NoXmlOutputFile = false;
			
			string expectedCommandLine = "\"C:\\Projects\\MyTests\\MyTests.dll\" /labels";
			Assert.AreEqual(expectedCommandLine, app.GetArguments());
		}
		
		[Test]
		public void TestFixture()
		{
			NUnitConsoleApplication app = new NUnitConsoleApplication(new[] { testProject });
			app.NoLogo = false;
			app.ShadowCopy = true;
			app.Fixture = "TestFixture";
			app.NoXmlOutputFile = false;
			
			string expectedCommandLine = "\"C:\\Projects\\MyTests\\MyTests.dll\" /run=\"TestFixture\"";
			Assert.AreEqual(expectedCommandLine, app.GetArguments());
		}

		[Test]
		public void TestNamespace()
		{
			NUnitConsoleApplication app = new NUnitConsoleApplication(new[] { testProject });
			app.NoLogo = false;
			app.ShadowCopy = true;
			app.NamespaceFilter = "TestFixture";
			app.NoXmlOutputFile = false;
			
			string expectedCommandLine = "\"C:\\Projects\\MyTests\\MyTests.dll\" /run=\"TestFixture\"";
			Assert.AreEqual(expectedCommandLine, app.GetArguments());
		}
		
		[Test]
		public void XmlOutputFile()
		{
			NUnitConsoleApplication app = new NUnitConsoleApplication(new[] { testProject });
			app.NoLogo = false;
			app.ShadowCopy = true;
			app.XmlOutputFile = @"C:\NUnit.xml";
			app.NoXmlOutputFile = false;
			
			string expectedCommandLine = "\"C:\\Projects\\MyTests\\MyTests.dll\" /xml=\"C:\\NUnit.xml\"";
			Assert.AreEqual(expectedCommandLine, app.GetArguments());
		}
		
		[Test]
		public void NoXmlWhenXmlOutputFileSpecified()
		{
			NUnitConsoleApplication app = new NUnitConsoleApplication(new[] { testProject });
			app.NoLogo = false;
			app.ShadowCopy = true;
			app.XmlOutputFile = @"C:\NUnit.xml";
			app.NoXmlOutputFile = true;
			
			string expectedCommandLine = "\"C:\\Projects\\MyTests\\MyTests.dll\" /noxml";
			Assert.AreEqual(expectedCommandLine, app.GetArguments());
		}
		
		[Test]
		public void TestMethod()
		{
			NUnitConsoleApplication app = new NUnitConsoleApplication(new[] { testProject });
			app.NoLogo = false;
			app.ShadowCopy = true;
			app.Fixture = "TestFixture";
			app.Test = "Test";
			app.NoXmlOutputFile = false;
			
			string expectedCommandLine = "\"C:\\Projects\\MyTests\\MyTests.dll\" /run=\"TestFixture.Test\"";
			Assert.AreEqual(expectedCommandLine, app.GetArguments());
		}
		
		[Test]
		public void TestMethodSpecifiedInInitialize()
		{
			var method = new DefaultUnresolvedMethod(new DefaultUnresolvedTypeDefinition("TestFixture"), "Test");
			var testMethod = new NUnitTestMethod(testProject, method);
			NUnitConsoleApplication app = new NUnitConsoleApplication(new[] { testMethod });
			app.NoLogo = false;
			app.ShadowCopy = true;
			app.NoXmlOutputFile = false;
			
			string expectedCommandLine = "\"C:\\Projects\\MyTests\\MyTests.dll\" /run=\"TestFixture.Test\"";
			Assert.AreEqual(expectedCommandLine, app.GetArguments());
		}
		
		[Test]
		public void TestNamespaceSpecifiedInInitialize()
		{
			var testNamespace = new TestNamespace(testProject, "Project.MyTests");
			NUnitConsoleApplication app = new NUnitConsoleApplication(new[] { testNamespace });
			app.NoLogo = false;
			app.ShadowCopy = true;
			app.NoXmlOutputFile = false;
			
			string expectedCommandLine = "\"C:\\Projects\\MyTests\\MyTests.dll\" /run=\"Project.MyTests\"";
			Assert.AreEqual(expectedCommandLine, app.GetArguments());
		}
		
		[Test]
		public void FullCommandLine()
		{
			NUnitConsoleApplication app = new NUnitConsoleApplication(new[] { testProject });
			app.NoLogo = true;
			app.ShadowCopy = true;
			app.NoXmlOutputFile = false;
			
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
			NUnitConsoleApplication app = new NUnitConsoleApplication(new[] { testProject });
			app.Assemblies.Add("SecondAssembly.dll");
			app.NoLogo = false;
			app.ShadowCopy = true;
			app.ResultsPipe = @"C:\results.txt";
			app.NoXmlOutputFile = false;
			
			string expectedCommandLine =
				"\"C:\\Projects\\MyTests\\MyTests.dll\" " +
				"\"SecondAssembly.dll\" " +
				"/pipe=\"C:\\results.txt\"";
			Assert.AreEqual(expectedCommandLine, app.GetArguments());
		}
		
		[Test]
		public void GetProject()
		{
			NUnitConsoleApplication app = new NUnitConsoleApplication(new[] { testProject });
			Assert.AreSame(project, app.Project);
		}
		
		const string testProgram = @"
namespace MyTests {
	class TestFixture {
		class InnerTest {
			[NUnit.Framework.Test]
			public void InnerTest() {}
		}
	}
}
";
		
		[Test]
		public void TestInnerClassSpecifiedInInitialize()
		{
			NUnitTestClass innerTestClass = new NUnitTestClass(testProject, new FullTypeName("MyTests.TestFixture+InnerTest"));
			NUnitConsoleApplication app = new NUnitConsoleApplication(new [] { innerTestClass });
			app.NoLogo = false;
			app.ShadowCopy = true;
			app.NoXmlOutputFile = false;
			
			string expectedCommandLine = "\"C:\\Projects\\MyTests\\MyTests.dll\" " +
				"/run=\"MyTests.TestFixture+InnerTest\"";
			Assert.AreEqual(expectedCommandLine, app.GetArguments());
		}
		
		[Test]
		public void XmlOutputFileNameSpecifiedOnCommandLine()
		{
			UnitTestingOptions options = new UnitTestingOptions(new Properties());
			options.CreateXmlOutputFile = true;
			NUnitTestClass testFixture = new NUnitTestClass(testProject, new FullTypeName("MyTests.TestFixture.MyTest"));
			NUnitConsoleApplication app = new NUnitConsoleApplication(new[] { testFixture }, options);
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
