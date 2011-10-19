// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using ICSharpCode.Reports.Core.BaseClasses;
using ICSharpCode.Reports.Core.Globals;
using ICSharpCode.Reports.Core.Test.TestHelpers;
using NUnit.Framework;

namespace ICSharpCode.Reports.Core.Test.Printing
{
	[TestFixture]
	public class SectionBoundFixture:ConcernOf<SectionBounds>
	{

		[Test]
		public void Can_Read_SectionBounds ()
		{
			SectionBounds sb = new SectionBounds(new ReportSettings(),false);
			Assert.IsNotNull(sb);
		}
		
		
		[Test]
		public void CanReadMargins ()
		{
			ReportSettings rs = new ReportSettings();
			rs.LeftMargin = 20;
			rs.RightMargin = 20;
			rs.TopMargin = 20;
			rs.BottomMargin = 20;
			SectionBounds sb = new SectionBounds(rs,false);
			SinglePage singlePage = new SinglePage(sb,15);
			Assert.AreEqual(20,sb.MarginBounds.Left);
			Assert.AreEqual(20,sb.MarginBounds.Top);
			Assert.AreEqual(827 - 40,sb.MarginBounds.Width);
			Assert.AreEqual(1169 - 40,sb.MarginBounds.Height);
		}
		
		
		#region MeasureReportHeader
		
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void MeasureReportHeaderThrowsIfSectionIsNull()
		{
			Sut.MeasureReportHeader(null);
		}
		
		
		[Test]
		public void MeasureReportHeaderForFirstPageWithEmptyItems()
		{
			SectionBounds sectionBounds  = new SectionBounds(new ReportSettings(),true);
			var baseSection = CreateSection();
			sectionBounds.MeasureReportHeader(baseSection);
			Size size = new Size(baseSection.Size.Width,0);
			Assert.That(sectionBounds.ReportHeaderRectangle.Size,Is.EqualTo(size));
		}
		
		
		[Test]
		public void ReportHeader_For_FirstPage_WithItems()
		{
			SectionBounds sectionBounds  = new SectionBounds(new ReportSettings(),true);
			BaseSection reportHeader = CreateSection();
			reportHeader.Items.Add(new BaseTextItem());
			
			sectionBounds.MeasureReportHeader(reportHeader);
		
			Assert.That(sectionBounds.ReportHeaderRectangle.Size,Is.EqualTo(reportHeader.Size));            
		}
		
		
		[Test]
		public void MeasureReportHeaderForAnyPage ()
		{
			var baseSection = CreateSection();
			Sut.MeasureReportHeader(baseSection);
			Assert.AreEqual(0,Sut.ReportHeaderRectangle.Size.Height);
		}
		
		#endregion
		
		#region MeasurePageHeader
		
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void MeasurePageHeaderThrowsIfSectionIsNull()
		{
			Sut.MeasurePageHeader(null);
		}
		
		
		[Test]
		public void PageHeader_No_Items()
		{
			BaseSection baseSection = CreateSection();
			
			Sut.MeasurePageHeader(baseSection);
			Size size = new Size (baseSection.Size.Width,0);

			Assert.That(Sut.PageHeaderRectangle.Size,Is.EqualTo(size));
		}
		
	
		[Test]
		public void PageHeader_Location_One_Point_Under_ReportHeader()
		{
			BaseSection reportHeader = CreateSection();
			BaseSection pageHeader = CreateSection();
			Sut.MeasureReportHeader(reportHeader);
			Sut.MeasurePageHeader(pageHeader);
			
			Assert.That (pageHeader.SectionOffset,Is.EqualTo(Sut.ReportHeaderRectangle.Bottom + 1));
		}
	
		#endregion
		
		#region MeasurePageFooter
		
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void MeasurePageFooterThrowsIfSectionIsNull()
		{
			Sut.MeasurePageFooter(null);
		}
		
		
		[Test]
		public void MeasurePageFooter()
		{
			var setting = new ReportSettings();
			SectionBounds sectionBounds  = new SectionBounds(new ReportSettings(),true);
			BaseSection pageFootter = CreateSection();

			sectionBounds.MeasurePageFooter(pageFootter);
			Size expectedSize = pageFootter.Size;
			
			Assert.That(expectedSize,Is.EqualTo(sectionBounds.PageFooterRectangle.Size));
		}
		
		
		[Test]
		public void PageFooter_From_PageEnd_Uppward()
		{
			SectionBounds sectionBounds  = new SectionBounds(new ReportSettings(),true);
			BaseSection pageFootter = CreateSection();

			sectionBounds.MeasurePageFooter(pageFootter);
			int top = sectionBounds.MarginBounds.Bottom - pageFootter.Size.Height;
			
			Assert.That(sectionBounds.PageFooterRectangle.Top,Is.EqualTo(top));
		}
		#endregion
		
		#region MeasureReportFooter
		
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void MeasureReportFooterThrowsIfSectionIsNull()
		{
			Sut.MeasureReportFooter(null);
		}
		
		
		[Test]
		public void ReportFooter_No_PageFooter_Is_Calculated()
		{
			SectionBounds sectionBounds  = new SectionBounds(new ReportSettings(),true);
			BaseSection reportFootter = CreateSection();
			sectionBounds.MeasureReportFooter(reportFootter);
			Size expectedSize = reportFootter.Size;
			Assert.That(sectionBounds.ReportFooterRectangle.Size,Is.EqualTo(expectedSize));
		}
		
	
		[Test]
		public void MeasureReportFooterPageFooterIsCalculated()
		{
			SectionBounds sectionBounds  = new SectionBounds(new ReportSettings(),true);
			BaseSection pageFootter = CreateSection();
			BaseSection reportFootter = CreateSection();
			
			sectionBounds.MeasurePageFooter(pageFootter);			
			sectionBounds.MeasureReportFooter(reportFootter);
			
			int top = sectionBounds.MarginBounds.Bottom - pageFootter.Size.Height - reportFootter.Size.Height -1;
			Assert.That(sectionBounds.ReportFooterRectangle.Top,Is.EqualTo(top));
			
		}
		#endregion
		
		[Test]
		public void DetailArea_Start_One_Below_PageHeader ()
		{
			SectionBounds sectionBounds  = new SectionBounds(new ReportSettings(),true);
			BaseSection pageHeader = new BaseSection();
			sectionBounds.MeasurePageHeader(pageHeader);
			Point p = new Point(sectionBounds.PageHeaderRectangle.Left,sectionBounds.PageHeaderRectangle.Bottom +1	);
			Assert.That(sectionBounds.DetailArea.Location,Is.EqualTo(p));
		}
			
		[Test]
		public void DetailArea_Ends_One_Above_PageFooter()
		{
			SectionBounds sectionBounds  = new SectionBounds(new ReportSettings(),true);
			BaseSection pageFooter = new BaseSection();
			sectionBounds.MeasurePageFooter(pageFooter);
			Point p = new Point(sectionBounds.PageFooterRectangle.Left,sectionBounds.PageFooterRectangle.Y );
			Assert.That(sectionBounds.DetailArea.Top + sectionBounds.DetailArea.Height,Is.EqualTo(p.Y));
		}
		
		
		public override void Setup()
		{
			ReportSettings rs = new ReportSettings();
			Sut = new SectionBounds(rs,false);
		}
		
		BaseSection CreateSection()
		{
			var sec = new BaseSection();
			sec.Location = new Point (50,50);
			sec.Size = new Size (727,60);
			return sec;
		}
	}
}
