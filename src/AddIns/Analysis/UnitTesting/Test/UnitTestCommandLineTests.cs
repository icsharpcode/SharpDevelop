// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests
{
	[TestFixture]
	public class UnitTestCommandLineTests
	{
		CompilableProject project;
		UnitTestApplicationStartHelper helper;
		
		[SetUp]
		public void SetUp()
		{
			project = new MockCSharpProject();
			project.FileName = @"C:\Projects\MyTests\MyTests.csproj";
			project.AssemblyName = "MyTests";
			project.OutputType = OutputType.Library;
			project.SetProperty("OutputPath", "."); // don't create bin/Debug
			project.SetProperty("TargetFrameworkVersion", "v4.0");
			helper = new UnitTestApplicationStartHelper();
		}
		
		[Test]
		public void TestResultsFile()
		{
			helper.Initialize(project, null, null);
			helper.NoLogo = false;
			helper.ShadowCopy = true;
			helper.Results = @"C:\results.txt";
			
			string expectedCommandLine = "\"C:\\Projects\\MyTests\\MyTests.dll\" /results=\"C:\\results.txt\"";
			Assert.AreEqual(expectedCommandLine, helper.GetArguments());
		}
		
		[Test]
		public void NoLogo()
		{
			helper.Initialize(project, null, null);
			helper.NoLogo = true;
			helper.ShadowCopy = true;
			
			string expectedCommandLine = "\"C:\\Projects\\MyTests\\MyTests.dll\" /nologo";
			Assert.AreEqual(expectedCommandLine, helper.GetArguments());
		}
		
		[Test]
		public void NoShadowCopy()
		{
			helper.Initialize(project, null, null);
			helper.NoLogo = false;
			helper.ShadowCopy = false;
			
			string expectedCommandLine = "\"C:\\Projects\\MyTests\\MyTests.dll\" /noshadow";
			Assert.AreEqual(expectedCommandLine, helper.GetArguments());
		}
		
		[Test]
		public void NoThread()
		{
			helper.Initialize(project, null, null);
			helper.NoLogo = false;
			helper.ShadowCopy = true;
			helper.NoThread = true;
			
			string expectedCommandLine = "\"C:\\Projects\\MyTests\\MyTests.dll\" /nothread";
			Assert.AreEqual(expectedCommandLine, helper.GetArguments());
		}
		
		[Test]
		public void NoDots()
		{
			helper.Initialize(project, null, null);
			helper.NoLogo = false;
			helper.ShadowCopy = true;
			helper.NoDots = true;
			
			string expectedCommandLine = "\"C:\\Projects\\MyTests\\MyTests.dll\" /nodots";
			Assert.AreEqual(expectedCommandLine, helper.GetArguments());
		}		

		[Test]
		public void Labels()
		{
			helper.Initialize(project, null, null);
			helper.NoLogo = false;
			helper.ShadowCopy = true;
			helper.Labels = true;
			
			string expectedCommandLine = "\"C:\\Projects\\MyTests\\MyTests.dll\" /labels";
			Assert.AreEqual(expectedCommandLine, helper.GetArguments());
		}
		
		[Test]
		public void TestFixture()
		{
			helper.Initialize(project, null, null);
			helper.NoLogo = false;
			helper.ShadowCopy = true;
			helper.Fixture = "TestFixture";
			
			string expectedCommandLine = "\"C:\\Projects\\MyTests\\MyTests.dll\" /run=\"TestFixture\"";
			Assert.AreEqual(expectedCommandLine, helper.GetArguments());
		}

		[Test]
		public void TestNamespace()
		{
			helper.Initialize(project, null, null);
			helper.NoLogo = false;
			helper.ShadowCopy = true;
			helper.NamespaceFilter = "TestFixture";
			
			string expectedCommandLine = "\"C:\\Projects\\MyTests\\MyTests.dll\" /run=\"TestFixture\"";
			Assert.AreEqual(expectedCommandLine, helper.GetArguments());
		}
		
		[Test]
		public void XmlOutputFile()
		{
			helper.Initialize(project, null, null);
			helper.NoLogo = false;
			helper.ShadowCopy = true;
			helper.XmlOutputFile = @"C:\NUnit.xml";
			
			string expectedCommandLine = "\"C:\\Projects\\MyTests\\MyTests.dll\" /xml=\"C:\\NUnit.xml\"";
			Assert.AreEqual(expectedCommandLine, helper.GetArguments());
		}
		
		[Test]
		public void TestMethod()
		{
			helper.Initialize(project, null, null);
			helper.NoLogo = false;
			helper.ShadowCopy = true;
			helper.Fixture = "TestFixture";
			helper.Test = "Test";
			
			string expectedCommandLine = "\"C:\\Projects\\MyTests\\MyTests.dll\" /run=\"TestFixture.Test\"";
			Assert.AreEqual(expectedCommandLine, helper.GetArguments());
		}
		
		[Test]
		public void TestMethodSpecifiedInInitialize()
		{
			MockClass testFixture = new MockClass("TestFixture");
			MockMethod testMethod = new MockMethod("Test");
			helper.Initialize(project, testFixture, testMethod);
			helper.NoLogo = false;
			helper.ShadowCopy = true;
			
			string expectedCommandLine = "\"C:\\Projects\\MyTests\\MyTests.dll\" /run=\"TestFixture.Test\"";
			Assert.AreEqual(expectedCommandLine, helper.GetArguments());
		}
		
		[Test]
		public void TestNamespaceSpecifiedInInitialize()
		{
			helper.Initialize(project, "Project.MyTests");
			helper.NoLogo = false;
			helper.ShadowCopy = true;
			
			string expectedCommandLine = "\"C:\\Projects\\MyTests\\MyTests.dll\" /run=\"Project.MyTests\"";
			Assert.AreEqual(expectedCommandLine, helper.GetArguments());
		}
		
		[Test]
		public void FullCommandLine()
		{
			helper.Initialize(project, null, null);
			helper.NoLogo = true;
			helper.ShadowCopy = true;
			
			FileUtility.ApplicationRootPath = @"C:\SharpDevelop";
			
			string expectedFullCommandLine = "\"C:\\SharpDevelop\\bin\\Tools\\NUnit\\nunit-console-x86.exe\" \"C:\\Projects\\MyTests\\MyTests.dll\" /nologo";
			Assert.AreEqual(expectedFullCommandLine, helper.GetCommandLine());
		}
		
		/// <summary>
		/// Tests that a space is appended between the items added
		/// to the UnitTestApplicationStartHelper.Assemblies
		/// when the command line is generated.
		/// </summary>
		[Test]
		public void SecondAssemblySpecified()
		{
			helper.Initialize(project, null, null);
			helper.Assemblies.Add("SecondAssembly.dll");
			helper.NoLogo = false;
			helper.ShadowCopy = true;
			helper.Results = @"C:\results.txt";
			
			string expectedCommandLine = "\"C:\\Projects\\MyTests\\MyTests.dll\" \"SecondAssembly.dll\" /results=\"C:\\results.txt\"";
			Assert.AreEqual(expectedCommandLine, helper.GetArguments());
		}
		
		[Test]
		public void GetProject()
		{
			helper.Initialize(project, null, null);
			Assert.AreSame(project, helper.Project);
		}
		
		[Test]
		public void TestInnerClassSpecifiedInInitialize()
		{
			MockClass testFixture = new MockClass("MyTests.TestFixture.InnerTest", "MyTests.TestFixture+InnerTest");
			helper.Initialize(project, testFixture, null);
			helper.NoLogo = false;
			helper.ShadowCopy = true;
			
			string expectedCommandLine = "\"C:\\Projects\\MyTests\\MyTests.dll\" /run=\"MyTests.TestFixture+InnerTest\"";
			Assert.AreEqual(expectedCommandLine, helper.GetArguments());
		}
	}
}
