// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.Reports.Addin;
using ICSharpCode.Reports.Addin.ReportWizard;
using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Globals;
using ICSharpCode.SharpDevelop;
using NUnit.Framework;

namespace ICSharpCode.Reports.Addin.Test.Designer
{
	[TestFixture]
	public class ReportDesignerDisplayBindingTestFixture
	{
		DerivedReportDesignerDisplayBinding displayBinding;
		MockViewContent viewContent;
		//bool canAttachToDesignableClass;

		
		[SetUp]
		public void SetUp()
		{
			displayBinding = new DerivedReportDesignerDisplayBinding();
			viewContent = new MockViewContent();
			viewContent.PrimaryFileName = FileName.Create("test.srd");
//			viewContent.TextEditorControl.Text = "text content";
//			parseInfo = new ParseInformation();
//			displayBinding.ParseServiceParseInfoToReturn = parseInfo;
//			displayBinding.IsParseInfoDesignable = true;
//			canAttachToDesignableClass = displayBinding.CanAttachTo(viewContent);
		}
		
		[Test]
		public void CanCreateContentForFile()
		{
			Assert.IsTrue(displayBinding.CanCreateContentForFile("test.srd"));
		}
		
		
		[Test]
		public void CanCreateContentFromFile ()
		{
//			ReportModel model = ReportModel.Create();
//			Properties customizer = new Properties();
//			customizer.Set("ReportLayout",GlobalEnums.ReportLayout.ListLayout);
//			IReportGenerator generator = new GeneratePlainReport(model,customizer);
//			generator.GenerateReport();
//			MockOpenedFile mof = new MockOpenedFile(GlobalValues.PlainFileName);
			OpenedFile file = new MockOpenedFile(GlobalValues.PlainFileName);
//			file.SetData(generator.Generated.ToArray());
			
			//ICSharpCode.SharpDevelop.Gui.IViewContent v = displayBinding.CreateContentForFile(new MockOpenedFile("test.srd"));
			//Assert.IsNotNull(v,"View should not be 'null'");
		}
		
		
	}
}