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
