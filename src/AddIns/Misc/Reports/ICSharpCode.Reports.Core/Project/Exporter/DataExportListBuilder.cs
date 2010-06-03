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
			
			this.BuildPageHeader();
			

			this.BuildDetailInternal (base.ReportModel.DetailSection);
//			
			this.BuildReportFooter();
			this.BuildPageFooter();
			

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
			base.FireSectionRenderEvent(ReportModel.PageHeader,0);
			base.BuildPageHeader();
		
		}
		
		protected override void BuildDetailInternal(BaseSection section)
		{
			base.FireSectionRenderEvent(ReportModel.DetailSection,0);
			base.BuildDetailInternal(section);
		} 
		
		protected override void BuildPageFooter()
		{
			base.FireSectionRenderEvent(ReportModel.ReportFooter,0);
			base.BuildPageFooter();
		}
		
		
		protected override void BuildReportFooter()
		{
			base.FireSectionRenderEvent(ReportModel.ReportFooter,0);
			base.BuildReportFooter();
		}
		
		
		public IDataManager DataManager {get; private set;}
		
	}
	
	public class PageDescriptions :Collection<PageDescription>
	{
		
	}
	
}
