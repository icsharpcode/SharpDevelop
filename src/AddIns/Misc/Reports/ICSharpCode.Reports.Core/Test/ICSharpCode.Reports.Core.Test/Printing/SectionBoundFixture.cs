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
			Assert.That (pageHeader.SectionOffset,Is.GreaterThan(Sut.ReportHeaderRectangle.Bottom));
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
			BaseSection baseSection = new BaseSection();
			baseSection.Location = new Point (50,50);
			baseSection.Size = new Size (727,60);
			sectionBounds.MeasurePageFooter(baseSection);
			//sectionBounds.MeasurePageFooter(Sut);
			Assert.AreEqual(baseSection.Size.Height,
			                sectionBounds.PageFooterRectangle.Size.Height);
			                
			               
			Assert.AreEqual(sectionBounds.MarginBounds.Width,
			                sectionBounds.PageFooterRectangle.Width);
			//int i = sectionBounds.PageSize.Height - setting.BottomMargin;
			//Assert.AreEqual(i,sectionBounds.PageFooterRectangle.Location.Y);
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
		public void MeasureReportFooterNoPageFooterCalculated()
		{
			SectionBounds sectionBounds  = new SectionBounds(new ReportSettings(),true);
			BaseSection baseSection = new BaseSection();
			baseSection.Location = new Point (50,50);
			baseSection.Size = new Size (727,60);
			sectionBounds.MeasureReportFooter(baseSection);
			Assert.AreEqual(baseSection.Size.Height ,sectionBounds.ReportFooterRectangle.Size.Height);
			                			                    
			Assert.AreEqual(sectionBounds.MarginBounds.Width,
			                sectionBounds.ReportFooterRectangle.Width);
		}
		
		
		[Test]
		public void MeasureReportFooterPageFooterIsCalculated()
		{
			SectionBounds sectionBounds  = new SectionBounds(new ReportSettings(),true);
			BaseSection baseSection = new BaseSection();
			baseSection.Location = new Point (50,50);
			baseSection.Size = new Size (727,60);
			sectionBounds.MeasurePageFooter(baseSection);
			sectionBounds.MeasureReportFooter(baseSection);
			
			Assert.AreEqual(baseSection.Size.Height , sectionBounds.ReportFooterRectangle.Size.Height,			               
			                "ItemsCollection is empty, so Size.Height should be '0'");
			Assert.AreEqual(sectionBounds.MarginBounds.Width,
			                sectionBounds.ReportFooterRectangle.Width);
		}
		#endregion
		
		[Test]
		public void DetailStart ()
		{
			/*
			SectionBounds sectionBounds  = new SectionBounds(new ReportSettings(),true);
			BaseSection baseSection = new BaseSection();
			baseSection.Location = new Point (50,50);
			baseSection.Size = new Size (727,60);
			sectionBounds.MeasurePageHeader(baseSection);
			Point p = new Point(sectionBounds.PageHeaderRectangle.Left,sectionBounds.PageHeaderRectangle.Bottom + sectionBounds.Gap	);
			Assert.AreEqual(p,sectionBounds.DetailStart);
			*/
		}
			
		[Test]
		public void DetailEnds ()
		{
			/*
			SectionBounds sectionBounds  = new SectionBounds(new ReportSettings(),true);
			BaseSection baseSection = new BaseSection();
			baseSection.Location = new Point (50,50);
			baseSection.Size = new Size (727,60);
			sectionBounds.MeasurePageFooter(baseSection);
			Point p = new Point(sectionBounds.PageFooterRectangle.Left,sectionBounds.PageFooterRectangle.Top - sectionBounds.Gap);
			Assert.AreEqual(p,sectionBounds.DetailEnds);
			*/
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
			/*
			sec.Items.Add (new BaseTextItem()
			               {
			               	Text = "mytext";
			               }
			               */
			return sec;
		}
	}
}
