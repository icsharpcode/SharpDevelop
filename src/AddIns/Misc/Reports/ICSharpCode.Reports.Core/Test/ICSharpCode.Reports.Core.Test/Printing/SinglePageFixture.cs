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
using System.Drawing;
using ICSharpCode.Reports.Core.BaseClasses;
using ICSharpCode.Reports.Core.Interfaces;
using NUnit.Framework;

namespace ICSharpCode.Reports.Core.Test.Printing
{
	[TestFixture]
	public class SinglePageFixture
	{
		
		private SectionBounds sectionBounds;
		
		[Test]
		public void Can_Create_SinglePage()
		{
			SinglePage p = new SinglePage(sectionBounds,15);
			Assert.IsNotNull(p);
		}
		
		
		[Test]
		public void SinglePage_Should_Return_PageNumber()
		{
			SinglePage p = new SinglePage(sectionBounds,15);
			Assert.AreEqual(15,p.PageNumber);
		}
		
		
		[Test]
		[ExpectedExceptionAttribute(typeof(ArgumentNullException))]
		public void ConstrThrowIfNullSectionBounds()
		{
			SinglePage p = new SinglePage(null,15);
		}
		
		
		[Test]
		[ExpectedExceptionAttribute(typeof(ArgumentNullException))]
		public void ConstrThrowIfPageNumberLessThenZero()
		{
			SinglePage p = new SinglePage(this.sectionBounds,-1);
		}
		
		
		[Test]
		public void Calculate_ReportHeader ()
		{
			
			IReportModel model = ReportModel.Create();
			ISinglePage singlePage = CreateSinglePage(model,0);
			singlePage.CalculatePageBounds(model);

			Point expectedLocation = new Point(model.ReportSettings.LeftMargin,model.ReportSettings.TopMargin);
			Assert.That(singlePage.SectionBounds.ReportHeaderRectangle.Location,Is.EqualTo(expectedLocation));
		}
		
		
		[Test]
		public void Calculate_PageHeader ()
		{
			
			IReportModel model = ReportModel.Create();
			ISinglePage singlePage = CreateSinglePage(model,0);
			singlePage.CalculatePageBounds(model);

			Point expectedLocation = new Point(model.ReportSettings.LeftMargin,model.ReportSettings.TopMargin + singlePage.SectionBounds.ReportHeaderRectangle.Size.Height + 1);
			Assert.That(singlePage.SectionBounds.PageHeaderRectangle.Location,Is.EqualTo(expectedLocation));
		}
		
		
		[Test]
		public void Calculate_PageFooter ()
		{
			
			IReportModel model = ReportModel.Create();
			ISinglePage singlePage = CreateSinglePage(model,0);
			singlePage.CalculatePageBounds(model);

			
			Point expectedLocation = new Point(model.ReportSettings.LeftMargin,
			                                    model.ReportSettings.PageSize.Height - model.ReportSettings.BottomMargin - singlePage.SectionBounds.PageFooterRectangle.Height);
			Assert.That(singlePage.SectionBounds.PageFooterRectangle.Location,Is.EqualTo(expectedLocation));
		}
		
		
		IReportModel CreateModel ()
		{
			Size defaultSectionSize = new Size (727,60);
			IReportModel model = ReportModel.Create();
			model.ReportHeader.Size = defaultSectionSize;
			model.ReportHeader.Location = new Point(50,50);
			
			
			model.PageHeader.Size = defaultSectionSize;
			model.PageHeader.Location = new Point(50,125);
			
			model.DetailSection.Size = defaultSectionSize;
			model.DetailSection.Location = new Point(50,200);
			
			model.PageFooter.Size = defaultSectionSize;
			model.ReportFooter.Location = new Point(50,275);
			
			model.ReportFooter.Size = defaultSectionSize;
			model.ReportFooter.Location = new Point(50,350);
			return model;
		}
		
		
		ISinglePage CreateSinglePage(IReportModel model,int pageNumber)
		{
			Size defaultSectionSize = new Size (727,60);
			var  s = new SectionBounds(model.ReportSettings,true);
			
			SinglePage sp = new SinglePage(s,0);
			sp.CalculatePageBounds(model);
			return sp;
		}
		
		#region Setup/Teardown
		
		[TestFixtureSetUp]
		public void Init()
		{
			this.sectionBounds = new SectionBounds(new ReportSettings(),false);
		}
		
		#endregion
	}
}
