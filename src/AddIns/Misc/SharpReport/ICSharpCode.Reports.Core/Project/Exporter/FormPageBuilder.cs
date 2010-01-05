/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Forstmeier
 * Datum: 04.05.2007
 * Zeit: 11:34
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using ICSharpCode.Reports.Core.Interfaces;
using System;
using System.Drawing;

namespace ICSharpCode.Reports.Core.Exporter
{
	/// <summary>
	/// Description of FormPageBuilder.
	/// </summary>
	public class FormPageBuilder:BasePager
	{
		private readonly object addLock = new object();
		
		#region Constructure
		
		public static FormPageBuilder CreateInstance(IReportModel reportModel,ILayouter layouter)
		{
			if (reportModel == null) {
				throw new ArgumentNullException("reportModel");
			}
			
			FormPageBuilder instance = new FormPageBuilder(reportModel,layouter);
			return instance;
		}
		
		
		private FormPageBuilder(IReportModel reportModel,ILayouter layouter):base(reportModel,layouter)
		{
		}
		
		#endregion
		
		protected override void BuildNewPage()
		{
			base.BuildNewPage();
			this.BuildReportHeader();
			this.BuildPageHeader();
			this.BuildDetailInternal(base.ReportModel.DetailSection);
			this.BuildPageFooter();
		}
		
		
		protected override void BuildReportHeader()
		{
			base.BuildReportHeader();
			if (base.Pages.Count == 0) {
				this.ReportModel.ReportHeader.SectionOffset = base.SinglePage.SectionBounds.ReportHeaderRectangle.Top;
				base.ConvertSection(this.ReportModel.ReportHeader,
//				                      base.SinglePage.SectionBounds.ReportHeaderRectangle.Top,
				                      1 );                     
			}
		}
		
		protected override void BuildPageHeader()
		{
			base.BuildPageHeader();
			this.ReportModel.PageHeader.SectionOffset = base.SinglePage.SectionBounds.PageHeaderRectangle.Top;
			base.ConvertSection(this.ReportModel.PageHeader,
//				                      base.SinglePage.SectionBounds.PageHeaderRectangle.Top,
				                      1 ); 
		}
		
		protected override void BuildDetailInternal(BaseSection section)
		{
			base.BuildDetailInternal(section);
			section.SectionOffset = base.SinglePage.SectionBounds.DetailStart.Y;
			base.ConvertSection (section,
//			                       base.SinglePage.SectionBounds.DetailStart.Y,
			                       1);
		}

		
		protected override void BuildPageFooter()
		{
			base.BuildPageFooter();
			this.ReportModel.PageFooter.SectionOffset = base.SinglePage.SectionBounds.PageFooterRectangle.Top;
			base.ConvertSection(this.ReportModel.PageFooter,
//				                      base.SinglePage.SectionBounds.PageFooterRectangle.Top,
				                      1 ); 
		}
		
		
		protected override void BuildReportFooter(Rectangle footerRectangle)
		{
			base.BuildReportFooter(footerRectangle);
		}
		
		
		protected override void AddPage (ExporterPage page) {
			if (page == null) {
				throw new ArgumentNullException("page");
			}
			lock (addLock) {
				base.Pages.Add(page);
				
			}
			base.FirePageCreated(page);
		}
	
		
		private void WritePages ()
		{
			this.BuildNewPage();
			this.AddPage(base.SinglePage);
			base.FinishRendering(null);
		}
		
		
		public override void BuildExportList()
		{
			base.BuildExportList();
			WritePages ();
		}
		
	}
}
