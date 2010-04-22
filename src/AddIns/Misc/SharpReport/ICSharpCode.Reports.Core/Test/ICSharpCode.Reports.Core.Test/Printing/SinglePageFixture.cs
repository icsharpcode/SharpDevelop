/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 07.07.2009
 * Zeit: 19:59
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using System.Drawing;
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
		public void Calculate_Page_If_Firstpage ()
		{
			Size defSize = new Size (727,60);
			IReportModel model = ReportModel.Create();
			model.ReportHeader.Size = defSize;
			model.ReportHeader.Location = new Point(50,50);
			
			
			model.PageHeader.Size = defSize;
			model.PageHeader.Location = new Point(50,125);
			
			model.DetailSection.Size = defSize;
			model.DetailSection.Location = new Point(50,200);
			
			model.PageFooter.Size = defSize;
			model.ReportFooter.Location = new Point(50,275);
			
			model.ReportFooter.Size = defSize;
			model.ReportFooter.Location = new Point(50,350);
			
			var  s = new SectionBounds(new ReportSettings(),true);
			SinglePage sp = new SinglePage(s,0);
			
			sp.CalculatePageBounds(model);
			Console.WriteLine();
			Console.WriteLine("ReportHeader {0} - {1}",sp.SectionBounds.ReportHeaderRectangle,sp.SectionBounds.ReportHeaderRectangle.Location.Y + sp.SectionBounds.ReportHeaderRectangle.Height);
			Console.WriteLine("PageHeader {0} - {1}",sp.SectionBounds.PageHeaderRectangle,sp.SectionBounds.PageHeaderRectangle.Location.Y +sp.SectionBounds.PageHeaderRectangle.Height );
			Console.WriteLine("DetailSection {0} - {1} ",sp.SectionBounds.DetailSectionRectangle,sp.SectionBounds.DetailSectionRectangle.Location.Y + sp.SectionBounds.DetailSectionRectangle.Height);
			
			Console.WriteLine("\tDetailStart {0} ",sp.SectionBounds.DetailStart);
			Console.WriteLine("\tDetailEnd {0} ",sp.SectionBounds.DetailEnds);
			Console.WriteLine("\tDetailArea {0} ",sp.SectionBounds.DetailArea);
			Console.WriteLine("PageFooter {0} - {1} ",sp.SectionBounds.PageFooterRectangle,sp.SectionBounds.PageFooterRectangle.Location.Y + sp.SectionBounds.PageFooterRectangle.Height);
			Console.WriteLine("ReportFooter {0} - {1}",sp.SectionBounds.ReportFooterRectangle,sp.SectionBounds.ReportFooterRectangle.Location.Y + sp.SectionBounds.ReportFooterRectangle.Height);
	Console.WriteLine();
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
