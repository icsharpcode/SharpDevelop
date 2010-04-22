/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 03.09.2009
 * Zeit: 20:15
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using System.Drawing;
using NUnit.Framework;

namespace ICSharpCode.Reports.Core.Test.Printing
{
	[TestFixture]
	public class SectionBoundFixture
	{
		private SectionBounds sectionBounds;
		
	
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
		
		
		[Test]
		public void CheckForGap ()
		{
			Assert.AreEqual(1,this.sectionBounds.Gap);
		}
		
		#region MeasureReportHeader
		
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void MeasureReportHeaderThrowsIfSectionIsNull()
		{
			this.sectionBounds.MeasureReportHeader(null);
		}
		
		
		[Test]
		public void MeasureReportHeaderForFirstPageWithEmptyItems()
		{
			SectionBounds sectionBounds  = new SectionBounds(new ReportSettings(),true);
			BaseSection baseSection = new BaseSection();
			baseSection.Location = new Point (50,50);
			baseSection.Size = new Size (727,60);
			sectionBounds.MeasureReportHeader(baseSection);
			Assert.AreEqual(0,sectionBounds.ReportHeaderRectangle.Size.Height,
			                "ItemsCollection is empty, so Size.Height should be '0'");
			                
			Assert.AreEqual(sectionBounds.MarginBounds.Width,
			                sectionBounds.ReportHeaderRectangle.Width);
		}
		
		
		[Test]
		public void MeasureReportHeaderForFirstPageWithItems()
		{
			SectionBounds sectionBounds  = new SectionBounds(new ReportSettings(),true);
			
			BaseSection baseSection = new BaseSection();
			baseSection.Location = new Point (50,50);
			baseSection.Size = new Size (727,60);
		
			baseSection.Items.Add(new BaseTextItem());
			sectionBounds.MeasureReportHeader(baseSection);
			Assert.AreEqual(baseSection.Size.Height + sectionBounds.Gap,
			                sectionBounds.ReportHeaderRectangle.Size.Height,
			                "ItemsCollection is not empty, so Size.Height should NOT be '0'");
			Assert.AreEqual(sectionBounds.MarginBounds.Width,
			                sectionBounds.ReportHeaderRectangle.Width);
		}
		
		
		[Test]
		public void MeasureReportHeaderForAnyPage ()
		{
			BaseSection bs = new BaseSection();
			bs.Location = new Point (50,50);
			bs.Size = new Size (727,60);
			this.sectionBounds.MeasureReportHeader(bs);
			Assert.AreEqual(0,this.sectionBounds.ReportHeaderRectangle.Size.Height);
			int a = this.sectionBounds.MarginBounds.Width;
			Assert.AreEqual(this.sectionBounds.MarginBounds.Width,this.sectionBounds.ReportHeaderRectangle.Width);
		}
		
		#endregion
		
		#region MeasurePageHeader
		
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void MeasurePageHeaderThrowsIfSectionIsNull()
		{
			this.sectionBounds.MeasurePageHeader(null);
		}
		
		
		[Test]
		public void MeasurePageHeader()
		{
			SectionBounds sectionBounds  = new SectionBounds(new ReportSettings(),true);
			BaseSection baseSection = new BaseSection();
			baseSection.Location = new Point (50,50);
			baseSection.Size = new Size (727,60);
			sectionBounds.MeasurePageHeader(baseSection);
			Assert.AreEqual(baseSection.Size.Height + sectionBounds.Gap, sectionBounds.PageHeaderRectangle.Size.Height,
			                "ItemsCollection is empty, so Size.Height should be '0'");
			               
			Assert.AreEqual(sectionBounds.MarginBounds.Width,
			                sectionBounds.PageHeaderRectangle.Width);
		}
		
		
		#endregion
		
		#region MeasurePageFooter
		
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void MeasurePageFooterThrowsIfSectionIsNull()
		{
			this.sectionBounds.MeasurePageFooter(null);
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
			this.sectionBounds.MeasureReportFooter(null);
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
			SectionBounds sectionBounds  = new SectionBounds(new ReportSettings(),true);
			BaseSection baseSection = new BaseSection();
			baseSection.Location = new Point (50,50);
			baseSection.Size = new Size (727,60);
			sectionBounds.MeasurePageHeader(baseSection);
			Point p = new Point(sectionBounds.PageHeaderRectangle.Left,sectionBounds.PageHeaderRectangle.Bottom + sectionBounds.Gap	);
			Assert.AreEqual(p,sectionBounds.DetailStart);
		}
			
		[Test]
		public void DetailEnds ()
		{
			SectionBounds sectionBounds  = new SectionBounds(new ReportSettings(),true);
			BaseSection baseSection = new BaseSection();
			baseSection.Location = new Point (50,50);
			baseSection.Size = new Size (727,60);
			sectionBounds.MeasurePageFooter(baseSection);
			Point p = new Point(sectionBounds.PageFooterRectangle.Left,sectionBounds.PageFooterRectangle.Top - sectionBounds.Gap);
			Assert.AreEqual(p,sectionBounds.DetailEnds);
		}
		
		
		#region Setup/Teardown		
				
		[TestFixtureSetUp]
		public void Init()
		{
			ReportSettings rs = new ReportSettings();
			this.sectionBounds = new SectionBounds(rs,false);
		}
		
		[TestFixtureTearDown]
		public void Dispose()
		{
			// TODO: Add tear down code.
		}
		
		#endregion
	}
}
