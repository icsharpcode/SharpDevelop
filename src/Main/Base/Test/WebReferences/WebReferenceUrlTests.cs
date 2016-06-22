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

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using System;
using System.IO;

namespace ICSharpCode.SharpDevelop.Tests.WebReferences
{
	[TestFixture]
	public class WebReferenceUrlTests : SDTestFixtureBase
	{
		[Test]
		public void FileNameTest()
		{
			MSBuildBasedProject project = WebReferenceTestHelper.CreateTestProject("C#");
			project.FileName = FileName.Create("c:\\projects\\test\\foo.csproj");
			WebReferenceUrl url = new WebReferenceUrl(project);
			url.RelPath = "Web References\\localhost";
			url.Include = "http://localhost/test.asmx";
			
			Assert.AreEqual(project.Directory.CombineFile(url.RelPath), url.FileName);
		}
		
		[Test]
		public void RelPathEndsWithSlash()
		{
			MSBuildBasedProject project = WebReferenceTestHelper.CreateTestProject("C#");
			project.FileName = FileName.Create("c:\\projects\\test\\foo.csproj");
			WebReferenceUrl url = new WebReferenceUrl(project);
			url.RelPath = "Web References\\localhost\\";
			url.Include = "http://localhost/test.asmx";
			
			Assert.AreEqual(project.Directory.CombineFile(url.RelPath.Trim('\\')), url.FileName);
		}
		
		[Test]
		public void ChangeFileName()
		{
			MSBuildBasedProject project = WebReferenceTestHelper.CreateTestProject("C#");
			project.FileName = FileName.Create("c:\\projects\\test\\foo.csproj");
			WebReferenceUrl url = new WebReferenceUrl(project);
			url.RelPath = "Web References\\localhost";
			url.Include = "http://localhost/test.asmx";
			
			// Change filename - simulate a folder rename.
			url.FileName = FileName.Create("c:\\projects\\test\\Web References\\mywebservice");
			
			Assert.AreEqual("http://localhost/test.asmx", url.Include);
			Assert.AreEqual("Web References\\mywebservice", url.RelPath);
		}
		
		[Test]
		public void NoNamespaceSpecified()
		{
			MSBuildBasedProject project = WebReferenceTestHelper.CreateTestProject("C#");
			project.FileName = FileName.Create("c:\\projects\\test\\foo.csproj");
			project.RootNamespace = "TestRootNamespace";
			WebReferenceUrl url = new WebReferenceUrl(project);
			
			Assert.AreEqual("TestRootNamespace", url.Namespace);
		}
		
		[Test]
		public void NamespaceSpecified()
		{
			MSBuildBasedProject project = WebReferenceTestHelper.CreateTestProject("C#");
			project.FileName = FileName.Create("c:\\projects\\test\\foo.csproj");
			project.RootNamespace = "TestRootNamespace";
			WebReferenceUrl url = new WebReferenceUrl(project);
			url.Namespace = "WebReferenceNamespace";
			
			Assert.AreEqual("WebReferenceNamespace", url.Namespace);
		}
	}
}
