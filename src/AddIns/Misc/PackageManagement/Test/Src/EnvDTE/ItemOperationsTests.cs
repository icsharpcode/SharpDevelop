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
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.PackageManagement.EnvDTE;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class ItemOperationsTests
	{
		DTE dte;
		FakePackageManagementProjectService fakeProjectService;
		FakeFileService fakeFileService;
		ItemOperations itemOperations;
		
		void CreateItemOperations()
		{
			fakeProjectService = new FakePackageManagementProjectService();
			fakeFileService = new FakeFileService(null);
			dte = new DTE(fakeProjectService, fakeFileService);
			itemOperations = (ItemOperations)dte.ItemOperations;
		}
		
		[Test]
		public void OpenFile_FileNamePassed_OpensFileInSharpDevelop()
		{
			CreateItemOperations();
			string expectedFileName = @"d:\temp\readme.txt";
			itemOperations.OpenFile(expectedFileName);
			
			string actualFileName = fakeFileService.FileNamePassedToOpenFile;
			
			Assert.AreEqual(expectedFileName, actualFileName);
		}
		
		[Test]
		public void Navigate_UrlPassed_OpensUrlInSharpDevelop()
		{
			CreateItemOperations();
			string expectedUrl = "http://sharpdevelop.com";
			itemOperations.Navigate(expectedUrl);
			
			string url = fakeFileService.FileNamePassedToOpenFile;
			
			Assert.AreEqual(expectedUrl, url);
		}
	}
}
