// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
