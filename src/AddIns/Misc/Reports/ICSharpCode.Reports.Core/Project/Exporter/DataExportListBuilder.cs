/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 15.05.2010
 * Time: 18:21
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;

using ICSharpCode.Reports.Core.Interfaces;

namespace ICSharpCode.Reports.Core.Exporter
{
	/// <summary>
	/// Description of BuildExportList.
	/// </summary>
	public class DataExportListBuilder:AbstractExportListBuilder
	{
		
		
		public DataExportListBuilder(IReportModel reportModel,IDataManager dataManager):base(reportModel)
		{
			this.DataManager = dataManager;
			this.DataManager.GetNavigator.MoveNext();
		}
		
		
		public override void WritePages ()
		{
			base.CreateNewPage();
			
			base.PositionAfterRenderSection = this.BuildReportHeader(SinglePage.SectionBounds.ReportHeaderRectangle.Location);
			
			this.BuildPageHeader();
			
//			BaseSection section = base.ReportModel.DetailSection;
//
//			section.SectionOffset = base.SinglePage.SectionBounds.DetailStart.Y;
//			this.BuildDetail (section,dataNavigator);
//			
//			this.BuildReportFooter(SectionBounds.ReportFooterRectangle);
//			this.BuildPageFooter();
//			//this is the last Page
			this.AddPage(base.SinglePage);
			//base.FinishRendering(this.dataNavigator);
		}
		
		
		
		
		public IDataManager DataManager {get; private set;}
		
	}
	
	public class PageDescriptions :Collection<PageDescription>
	{
		
	}
		
	
	public class AbstractExportListBuilder
	{
		
		private PageDescriptions pages;
		private readonly object pageLock = new object();
		
		
		public AbstractExportListBuilder (IReportModel reportModel)
		{
			this.ReportModel = reportModel;
		}
		
		
		public virtual void WritePages ()
		{
			
		}
		
		
		private ReportItemCollection ConvertContainer (ISimpleContainer container)
		{
			var col = new ReportItemCollection();
			foreach (BaseReportItem element in container.Items)
			{
				col.Add(element);
			}
			return col;
		}
		
		
		
		private void ConvertSimpleItems (IEnumerable<BaseReportItem> items)
		{
			foreach (BaseReportItem element in items) {
				var container = element as ISimpleContainer;
				
				if (container != null) {
					Console.WriteLine(" is recursive");
					container.Items.AddRange(ConvertContainer (container));
				}
				
				this.SinglePage.Items.Add(element);
			}
		}
		
		
		private bool CanGrow (IEnumerable<BaseReportItem> collection)
		{
			IEnumerable<BaseReportItem> canGrowShrinkCollection = from bt in collection where bt.CanGrow == true select bt;
			return (canGrowShrinkCollection.Count() > 0);
		}
		
		
		private bool CanShrink (IEnumerable<BaseReportItem> collection)
		{
			IEnumerable<BaseReportItem> canGrowShrinkCollection = from bt in collection where bt.CanShrink == true select bt;
			return (canGrowShrinkCollection.Count() > 0);
		}
		
		
		
		protected virtual Point BuildReportHeader (Point reportHeaderStart)
		{
			System.Console.WriteLine("BuildReportHeader at {0} with {1} items ",reportHeaderStart,ReportModel.ReportHeader.Items.Count);
			
			BaseSection section = ReportModel.ReportHeader;
			Size size = section.Size;
			Point retval = Point.Empty;
			
			if ((!CanGrow(section.Items)) | (!CanShrink(section.Items))) {
				ConvertSimpleItems (section.Items);
				retval = new Point (reportHeaderStart.X , reportHeaderStart.Y + size.Height);
			} else {
				retval = new Point (reportHeaderStart.X , 150);
			}
			
			return retval;
		}
		
		
		
		protected virtual void BuildPageHeader ()
		{
//			System.Diagnostics.Trace.WriteLine(" BuildPageHeader");
//			PositionAfterRenderSection = new Point(PositionAfterRenderSection.X,PositionAfterRenderSection.Y + 20);
		}
		
		
		protected virtual void BuildDetailInternal (BaseSection section)
		{
			System.Diagnostics.Trace.WriteLine("BuildDetailInterna ");
		}
		
		
		protected virtual void BuildPageFooter ()
		{
			System.Diagnostics.Trace.WriteLine("BuildPageFooter ");
		}
		
		protected virtual void BuildReportFooter ()
		{
			System.Diagnostics.Trace.WriteLine("BuildReportFooter ");
		}
		
		
		public virtual void CreateNewPage ()
		{
			this.SinglePage = this.InitNewPage();
			PrintHelper.InitPage(this.SinglePage,this.ReportModel.ReportSettings);	
			this.SinglePage.CalculatePageBounds(this.ReportModel);
//			this.pageFull = false;
		}
		
		public	Point PositionAfterRenderSection {get;set;}
		
		
		protected PageDescription InitNewPage ()
		{
			SectionBounds sectionBounds  = new SectionBounds (ReportModel.ReportSettings,(this.Pages.Count == 0));
			return new PageDescription(sectionBounds,Pages.Count + 1);
		}
		
		
		
		protected  void AddPage (PageDescription page)
		{
			if (page == null) {
				throw new ArgumentNullException("page");
			}
//			lock (addLock) {
				Pages.Add(page);
				
//			}
			//FirePageCreated(page);
		}
		
		
		
		
		
		public PageDescription SinglePage {get;private set;}
		
		public IReportModel ReportModel {get;private set;}
		
		
		public PageDescriptions Pages
		{
			get {
				lock(pageLock) {
					if (this.pages == null) {
						pages = new PageDescriptions();
						
					}
					return pages;
				}
			}
		}
		
	}
}
