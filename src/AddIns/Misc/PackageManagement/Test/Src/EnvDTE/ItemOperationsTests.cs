// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
