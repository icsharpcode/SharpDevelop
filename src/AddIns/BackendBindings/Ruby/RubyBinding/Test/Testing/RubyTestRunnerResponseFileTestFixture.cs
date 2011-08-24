// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Text;
using ICSharpCode.Core.Services;
using ICSharpCode.RubyBinding;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using RubyBinding.Tests.Utils;
using UnitTesting.Tests.Utils;

namespace RubyBinding.Tests.Testing
{
	[TestFixture]
	public class RubyTestRunnerResponseFileTestFixture
	{
		RubyTestRunnerResponseFile responseFile;
		StringBuilder responseFileText;
		StringWriter writer;
		
		[SetUp]
		public void Init()
		{
			responseFileText = new StringBuilder();
			writer = new StringWriter(responseFileText);
			responseFile = new RubyTestRunnerResponseFile(writer);
		}
		
		[Test]
		public void DisposeMethodDisposesTextWriterPassedInConstructor()
		{
			responseFile.Dispose();
			Assert.Throws<ObjectDisposedException>(delegate { writer.WriteLine("test"); });
		}
		
		[Test]
		public void WriteTestsAddsTestClassFileNameToResponseFile()
		{
			MockClass c = MockClass.CreateMockClassWithoutAnyAttributes();
			c.CompilationUnit.FileName = @"d:\mytest.rb";
			SelectedTests selectedTests = RubySelectedTestsHelper.CreateSelectedTests(c);
			responseFile.WriteTests(selectedTests);
			
			string expectedText = "d:\\mytest.rb\r\n";
			Assert.AreEqual(expectedText, responseFileText.ToString());
		}
		
		[Test]
		public void WriteTestsAddsTestMethodFileNameToResponseFile()
		{
			MockClass c = MockClass.CreateMockClassWithoutAnyAttributes();
			MockMethod method = new MockMethod(c, "MyTest");
			method.CompilationUnit.FileName = @"d:\mytest.rb";
			SelectedTests selectedTests = RubySelectedTestsHelper.CreateSelectedTests(method);
			responseFile.WriteTests(selectedTests);
			
			string expectedText = "d:\\mytest.rb\r\n";
			Assert.AreEqual(expectedText, responseFileText.ToString());
		}
		
		[Test]
		public void WriteTestsAddsFileNamesForFileInProject()
		{
			MockCSharpProject project = new MockCSharpProject(new Solution(new MockProjectChangeWatcher()), "mytests");
			
			FileProjectItem item = new FileProjectItem(project, ItemType.Compile);
			item.FileName = @"c:\projects\mytests\myTests.rb";
			ProjectService.AddProjectItem(project, item);
			
			SelectedTests selectedTests = RubySelectedTestsHelper.CreateSelectedTests(project);
			responseFile.WriteTests(selectedTests);
			
			string expectedText = "c:\\projects\\mytests\\myTests.rb\r\n";
			Assert.AreEqual(expectedText, responseFileText.ToString());
		}
		
		[Test]
		public void WriteTestsDoesNotThrowNullReferenceExceptionWhenNonFileProjectItemInProject()
		{
			MockCSharpProject project = new MockCSharpProject(new Solution(new MockProjectChangeWatcher()), "mytests");
			WebReferenceUrl webRef = new WebReferenceUrl(project);
			webRef.Include = "test";
			ProjectService.AddProjectItem(project, webRef);
			
			FileProjectItem item = new FileProjectItem(project, ItemType.Compile);
			item.FileName = @"c:\projects\mytests\myTests.rb";
			ProjectService.AddProjectItem(project, item);
			
			SelectedTests tests = new SelectedTests(project);
			responseFile.WriteTests(tests);
			
			string expectedText = "c:\\projects\\mytests\\myTests.rb\r\n";
			Assert.AreEqual(expectedText, responseFileText.ToString());
		}
		
		[Test]
		public void WriteTestsDoesNotThrowNullReferenceExceptionWhenProjectIsNull()
		{
			SelectedTests tests = new SelectedTests(null);
			responseFile.WriteTests(tests);
			
			Assert.AreEqual(String.Empty, responseFileText.ToString());
		}
	}
}
