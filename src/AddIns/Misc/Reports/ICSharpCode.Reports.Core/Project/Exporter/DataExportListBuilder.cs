/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 15.05.2010
 * Time: 18:21
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using ICSharpCode.Reports.Core.Exporter.Converter;
using ICSharpCode.Reports.Core.Interfaces;

namespace ICSharpCode.Reports.Core.Exporter
{
	/// <summary>
	/// Description of BuildExportList.
	/// </summary>
	public class DataExportListBuilder:AbstractExportListBuilder
	{
		
		
		public DataExportListBuilder(IReportModel reportModel,IDataManager dataManager):base(reportModel,new ItemsConverter())
		{
			this.DataManager = dataManager;
			this.DataManager.GetNavigator.MoveNext();
			
		}
		
		/// <summary>
		/// This constructor os ONLY for testing reasons
		/// </summary>
		
		public DataExportListBuilder (IReportModel reportModel,IDataManager dataManager,IItemsConverter itemsConverter):base(reportModel,itemsConverter)
		{
		}
		
		
		public override void WritePages ()
		{
			base.CreateNewPage();
			
			base.PositionAfterRenderSection = this.BuildReportHeader(SinglePage.SectionBounds.ReportHeaderRectangle.Location);
			
//			this.BuildPageHeader();
			
//			BaseSection section = base.ReportModel.DetailSection;
//
//			section.SectionOffset = base.SinglePage.SectionBounds.DetailStart.Y;
//			this.BuildDetail (section,dataNavigator);
//			
//			this.BuildReportFooter(SectionBounds.ReportFooterRectangle);
//			this.BuildPageFooter();
//			//this is the last Page
			base.AddPage(base.SinglePage);
			//base.FinishRendering(this.dataNavigator);
		}
		
		
		protected override System.Drawing.Point BuildReportHeader(System.Drawing.Point reportHeaderStart)
		{
			
			Point p =  base.BuildReportHeader(reportHeaderStart);
			base.FireSectionRenderEvent(ReportModel.ReportHeader,0);
			return p;
		}
		
		
		protected override void BuildPageHeader()
		{
			base.BuildPageHeader();
		
		}
		
		
		public IDataManager DataManager {get; private set;}
		
	}
	
	public class PageDescriptions :Collection<PageDescription>
	{
		
	}
	
}
