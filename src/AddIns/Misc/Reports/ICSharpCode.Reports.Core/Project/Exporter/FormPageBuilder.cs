// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		
		public static FormPageBuilder CreateInstance(IReportModel reportModel)
		{
			if (reportModel == null) {
				throw new ArgumentNullException("reportModel");
			}
			FormPageBuilder instance = new FormPageBuilder(reportModel);
			return instance;
		}
		
		
		private FormPageBuilder(IReportModel reportModel):base(reportModel)
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
			if (base.Pages.Count == 0)
			{	
				SectionBounds.Offset = new Point(base.SectionBounds.MarginBounds.Left,base.SectionBounds.MarginBounds.Top);
				ExporterCollection convertedList =  base.ConvertSection (base.ReportModel.ReportHeader,0);
				base.SinglePage.Items.AddRange(convertedList);
			}
		}
		
		
		protected override void BuildPageHeader()
		{
			if (SectionBounds.Offset.Y < base.ReportModel.PageHeader.SectionOffset) {
				SectionBounds.Offset = new Point(SectionBounds.Offset.X,base.ReportModel.PageHeader.SectionOffset);
			}
			ExporterCollection convertedList =  base.ConvertSection (base.ReportModel.PageHeader,0);
			base.SinglePage.Items.AddRange(convertedList);
			base.SectionBounds.CalculatePageBounds(base.ReportModel);
		}
		
		
		protected override void BuildDetailInternal(BaseSection section)
		{
			base.BuildDetailInternal(section);
			SectionBounds.Offset = new Point(SectionBounds.Offset.X,base.ReportModel.DetailSection.Location.Y);
			ExporterCollection convertedList =  base.ConvertSection (base.ReportModel.DetailSection,0);
			base.SinglePage.Items.AddRange(convertedList);	   
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
