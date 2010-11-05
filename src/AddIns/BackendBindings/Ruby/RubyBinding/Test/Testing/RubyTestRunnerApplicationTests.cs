// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.IO;

using ICSharpCode.Core;
using ICSharpCode.RubyBinding;
using ICSharpCode.Scripting.Tests.Utils;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using RubyBinding.Tests.Testing;
using RubyBinding.Tests.Utils;
using UnitTesting.Tests.Utils;

namespace RubyBinding.Tests.Testing
{
	[TestFixture]
	public class RubyTestRunnerApplicationTests
	{
		RubyTestRunnerApplication testRunner;
		RubyAddInOptions options;
		
		[SetUp]
		public void Init()
		{
			string tempFileName = "temp.tmp";
			MockScriptingFileService fileService = new MockScriptingFileService();
			fileService.SetTempFileName(tempFileName);
			fileService.SetTextWriter(new StringWriter());

			Properties properties = new Properties();
			options = new RubyAddInOptions(properties);

			AddInPathHelper helper = new AddInPathHelper("RubyBinding");
			AddIn addin = helper.CreateDummyAddInInsideAddInTree();
			addin.FileName = @"c:\rubybinding\rubybinding.addin";

			string testResultsFileName = "results.txt";
			testRunner = new RubyTestRunnerApplication(testResultsFileName, options, fileService);
		}
		
		[Test]
		public void CreateProcessInfoReturnsCommandLineWithTestClassOption()
		{
			MockClass c = MockClass.CreateMockClassWithoutAnyAttributes();
			c.FullyQualifiedName = "MyTests";
			SelectedTests selectedTests = RubySelectedTestsHelper.CreateSelectedTests(c);
			ProcessStartInfo processStartInfo = GetProcessStartInfoFromTestRunnerApp(selectedTests);
			
			string expectedCommandLine = 
				"--disable-gems " +
				"\"-Ic:\\rubybinding\\TestRunner\" " +
				"\"c:\\rubybinding\\TestRunner\\sdtest.rb\" " +
				"--testcase=MyTests " +
				"-- " +
				"\"results.txt\" " +
				"\"temp.tmp\"";
			
			Assert.AreEqual(expectedCommandLine, processStartInfo.Arguments);
		}
		
		ProcessStartInfo GetProcessStartInfoFromTestRunnerApp(SelectedTests selectedTests)
		{
			testRunner.CreateResponseFile(selectedTests);
			return testRunner.CreateProcessStartInfo(selectedTests);
		}
		
		[Test]
		public void CreateProcessInfoReturnsCommandLineWithTestMethodOption()
		{
			MockClass c = MockClass.CreateMockClassWithoutAnyAttributes();
			MockMethod method = new MockMethod(c, "MyMethod");
			SelectedTests selectedTests = RubySelectedTestsHelper.CreateSelectedTests(method);
			ProcessStartInfo processStartInfo = GetProcessStartInfoFromTestRunnerApp(selectedTests);
			
			string expectedCommandLine = 
				"--disable-gems " +
				"\"-Ic:\\rubybinding\\TestRunner\" " +
				"\"c:\\rubybinding\\TestRunner\\sdtest.rb\" " +
				"--name=MyMethod " +
				"-- " +
				"\"results.txt\" " +
				"\"temp.tmp\"";
			
			Assert.AreEqual(expectedCommandLine, processStartInfo.Arguments);
		}
		
		[Test]
		public void CreateProcessInfoReturnsFixedTestRunnerFilePath()
		{
			AddInPathHelper helper = new AddInPathHelper("RubyBinding");
			AddIn addin = helper.CreateDummyAddInInsideAddInTree();
			addin.FileName = @"c:\rubybinding\bin\..\AddIns\rubybinding.addin";
			
			MockCSharpProject project = new MockCSharpProject();
			SelectedTests selectedTests = RubySelectedTestsHelper.CreateSelectedTests(project);
			ProcessStartInfo processStartInfo = GetProcessStartInfoFromTestRunnerApp(selectedTests);
			
			string expectedCommandLine = 
				"--disable-gems " +
				"\"-Ic:\\rubybinding\\AddIns\\TestRunner\" " +
				"\"c:\\rubybinding\\AddIns\\TestRunner\\sdtest.rb\" " +
				"-- " +
				"\"results.txt\" " +
				"\"temp.tmp\"";
			
			Assert.AreEqual(expectedCommandLine, processStartInfo.Arguments);
		}
		
		[Test]
		public void CreateProcessInfoReturnsCommandLineWithLoadPathSpecifiedInRubyAddInOptions()
		{
			MockCSharpProject project = new MockCSharpProject();
			options.RubyLibraryPath = @"c:\ruby\lib";
			SelectedTests selectedTests = RubySelectedTestsHelper.CreateSelectedTests(project);
			ProcessStartInfo processStartInfo = GetProcessStartInfoFromTestRunnerApp(selectedTests);
			
			string expectedCommandLine = 
				"--disable-gems " +
				"\"-Ic:\\ruby\\lib\" " +
				"\"-Ic:\\rubybinding\\TestRunner\" " +
				"\"c:\\rubybinding\\TestRunner\\sdtest.rb\" " +
				"-- " +
				"\"results.txt\" " +
				"\"temp.tmp\"";
			
			Assert.AreEqual(expectedCommandLine, processStartInfo.Arguments);
		}
		
		[Test]
		public void CreateProcessInfoReturnsCommandLineWithDirectoriesForReferencedProjects()
		{
			MockCSharpProject referencedProject = new MockCSharpProject();
			referencedProject.FileName = @"c:\projects\rbproject\rbproject.rbproj";
			
			MockCSharpProject unitTestProject = new MockCSharpProject();
			ProjectReferenceProjectItem projectRef = new ProjectReferenceProjectItem(unitTestProject, referencedProject);
			projectRef.FileName = @"c:\projects\rbproject\pyproject.rbproj";
			ProjectService.AddProjectItem(unitTestProject, projectRef);
			
			MockMethod method = MockMethod.CreateMockMethodWithoutAnyAttributes();
			method.CompilationUnit.FileName = @"d:\mytest.rb";
			FileProjectItem fileItem = new FileProjectItem(unitTestProject, ItemType.Compile);
			fileItem.FileName = @"d:\mytest.rb";
			ProjectService.AddProjectItem(unitTestProject, fileItem);
			
			SelectedTests tests = RubySelectedTestsHelper.CreateSelectedTests(unitTestProject);
			ProcessStartInfo processStartInfo = GetProcessStartInfoFromTestRunnerApp(tests);
						
			string expectedCommandLine = 
				"--disable-gems " +
				"\"-Ic:\\rubybinding\\TestRunner\" " +
				"\"-Ic:\\projects\\rbproject\" " +
				"\"c:\\rubybinding\\TestRunner\\sdtest.rb\" " +
				"-- " +
				"\"results.txt\" " +
				"\"temp.tmp\"";
			
			Assert.AreEqual(expectedCommandLine, processStartInfo.Arguments);
		}
	}
}
