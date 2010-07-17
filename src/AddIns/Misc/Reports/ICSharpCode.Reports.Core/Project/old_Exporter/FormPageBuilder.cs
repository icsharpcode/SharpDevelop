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
			if (base.Pages.Count == 0) {
				this.ReportModel.ReportHeader.SectionOffset = base.SinglePage.SectionBounds.ReportHeaderRectangle.Top;
				ExporterCollection convertedList =  base.ConvertSection (base.ReportModel.ReportHeader,0);
				base.SinglePage.Items.AddRange(convertedList);
			}
		}
		
		protected override void BuildPageHeader()
		{
			this.ReportModel.PageHeader.SectionOffset = base.SinglePage.SectionBounds.PageHeaderRectangle.Top;
			ExporterCollection convertedList =  base.ConvertSection (base.ReportModel.PageHeader,0);
			base.SinglePage.Items.AddRange(convertedList);	                      
		}
		
		protected override void BuildDetailInternal(BaseSection section)
		{
			base.BuildDetailInternal(section);
			section.SectionOffset = base.SinglePage.SectionBounds.DetailStart.Y;
		}

		
		protected override void BuildPageFooter()
		{
			this.ReportModel.PageFooter.SectionOffset = base.SinglePage.SectionBounds.PageFooterRectangle.Top;
			ExporterCollection convertedList = convertedList = base.ConvertSection (base.ReportModel.PageFooter,0);
			base.SinglePage.Items.AddRange(convertedList);	                      
		}
		
		
		protected override void BuildReportFooter(Rectangle footerRectangle)
		{
			ExporterCollection convertedList = base.ConvertSection (base.ReportModel.ReportFooter,0);
			base.SinglePage.Items.AddRange(convertedList);
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
