// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.Reports.Addin;
using ICSharpCode.Reports.Addin.ReportWizard;
using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Globals;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Workbench;
using NUnit.Framework;

namespace ICSharpCode.Reports.Addin.Test.Designer
{
	
	[TestFixture]

	public class ReportDesignerDisplayBindingTestFixture
	{
		ReportDesignerDisplayBinding displayBinding;
//		MockViewContent viewContent;
		//bool canAttachToDesignableClass;

		
		[SetUp]
		public void SetUp()
		{
			displayBinding = new ReportDesignerDisplayBinding();
//			viewContent = new MockViewContent();
//			viewContent.PrimaryFileName = FileName.Create("test.srd");
//			viewContent.TextEditorControl.Text = "text content";
//			parseInfo = new ParseInformation();
//			displayBinding.ParseServiceParseInfoToReturn = parseInfo;
//			displayBinding.IsParseInfoDesignable = true;
//			canAttachToDesignableClass = displayBinding.CanAttachTo(viewContent);
		}
		
		[Test]
		public void CanCreateContentForFile()
		{
			ICSharpCode.Core.FileName filename = new FileName("test.srd");
			Assert.IsTrue(displayBinding.CanCreateContentForFile(filename));
		}
		
		
		[Test]
		public void IsPreferredBindingForFile()
		{
			ICSharpCode.Core.FileName filename = new FileName("test.srd");
			Assert.IsTrue(displayBinding.IsPreferredBindingForFile(filename));
		}
		
	}
}