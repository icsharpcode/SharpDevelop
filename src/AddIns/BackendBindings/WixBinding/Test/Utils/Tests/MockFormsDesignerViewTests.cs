// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.Utils.Tests
{
	[TestFixture]
	public class MockFormsDesignerViewTests
	{
		MockFormsDesignerView view;
		
		[SetUp]
		public void Init()
		{
			view = new MockFormsDesignerView();
		}
		
		[Test]
		public void CanRetrievePrimaryOpenedFile()
		{
			MockOpenedFile file = new MockOpenedFile(@"d:\a.txt", false);
			view.PrimaryFile = file;
			Assert.AreSame(file, view.PrimaryFile);
		}
	}
}
