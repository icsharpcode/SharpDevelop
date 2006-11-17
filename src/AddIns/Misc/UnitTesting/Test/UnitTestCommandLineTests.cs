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
		public void Threaded()
		{
			helper.Initialize(project, null, null);
			helper.NoLogo = false;
			helper.ShadowCopy = true;
			helper.Threaded = true;
			
			string expectedCommandLine = "\"C:\\Projects\\MyTests\\MyTests.dll\" /thread";
			Assert.AreEqual(expectedCommandLine, helper.GetArguments());
		}
		
		[Test]
		public void TestFixture()
		{
			helper.Initialize(project, null, null);
			helper.NoLogo = false;
			helper.ShadowCopy = true;
			helper.Fixture = "TestFixture";
			
			string expectedCommandLine = "\"C:\\Projects\\MyTests\\MyTests.dll\" /fixture=\"TestFixture\"";
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
			
			string expectedCommandLine = "\"C:\\Projects\\MyTests\\MyTests.dll\" /fixture=\"TestFixture\" /testMethodName=\"TestFixture.Test\"";
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
			
			string expectedCommandLine = "\"C:\\Projects\\MyTests\\MyTests.dll\" /fixture=\"TestFixture\" /testMethodName=\"TestFixture.Test\"";
			Assert.AreEqual(expectedCommandLine, helper.GetArguments());
		}
		
		[Test]
		public void FullCommandLine()
		{
			helper.Initialize(project, null, null);
			helper.NoLogo = true;
			helper.ShadowCopy = true;
			
			FileUtility.ApplicationRootPath = @"C:\SharpDevelop";
			
			string expectedFullCommandLine = "\"C:\\SharpDevelop\\bin\\Tools\\NUnit\\nunit-console.exe\" \"C:\\Projects\\MyTests\\MyTests.dll\" /nologo";
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
	}
}
