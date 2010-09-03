// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NUnit.Framework;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.Utils.Tests
{
	[TestFixture]
	public class MockViewContentTests
	{
		MockViewContent view;
		
		[SetUp]
		public void Init()
		{
			view = new MockViewContent();
		}
		
		[Test]
		public void SetFileNameMethodCreatesNewPrimaryFile()
		{
			string fileName = @"d:\projects\a.txt";
			view.SetFileName(fileName);
			Assert.AreEqual(fileName, view.PrimaryFile.FileName.ToString());
		}
		
		[Test]
		public void SetFileNameMethodCreatesNewPrimaryFileName()
		{
			string fileName = @"d:\projects\a.txt";
			view.SetFileName(fileName);
			Assert.AreEqual(fileName, view.PrimaryFileName.ToString());
		}
			
		[Test]
		public void SetFileNameMethodCreatesNewPrimaryFileThatIsNotUntitled()
		{
			view.SetFileName(@"d:\test.txt");
			Assert.IsFalse(view.PrimaryFile.IsUntitled);
		}
		
		[Test]
		public void SetUntitledFileNameMethodCreatesNewPrimaryFile()
		{
			string fileName = @"d:\projects\a.txt";
			view.SetUntitledFileName(fileName);
			Assert.AreEqual(fileName, view.PrimaryFile.FileName.ToString());
		}
		
		[Test]
		public void SetUntitledFileNameMethodCreatesNewPrimaryFileName()
		{
			string fileName = @"d:\projects\a.txt";
			view.SetUntitledFileName(fileName);
			Assert.AreEqual(fileName, view.PrimaryFileName.ToString());
		}	
		
		[Test]
		public void SetUntitledFileNameMethodCreatesNewPrimaryFileThatIsUntitled()
		{
			view.SetUntitledFileName(@"d:\test.txt");
			Assert.IsTrue(view.PrimaryFile.IsUntitled);
		}
		
		[Test]
		public void DefaultPrimaryFileNameIsDummyName()
		{
			Assert.AreEqual("dummy.name", view.PrimaryFile.FileName.ToString());
		}
	}
}
