// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
