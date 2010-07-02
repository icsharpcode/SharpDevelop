/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 24.05.2010
 * Time: 19:53
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;

using ICSharpCode.Reports.Core.BaseClasses.Printing;
using ICSharpCode.Reports.Core.Exporter.Converter;
using ICSharpCode.Reports.Core.Interfaces;

namespace ICSharpCode.Reports.Core.Exporter
{
	/// <summary>
	/// Description of AbstractExportListBuilder.
	/// </summary>
	public class AbstractExportListBuilder
	{
		
		private readonly object pageLock = new object();
		private readonly IItemsConverter itemsConverter;
		
		private PageDescriptions pages;
		
		public event EventHandler<NewPageCreatedEventArgs> PageCreated;
		public event EventHandler<SectionRenderEventArgs> SectionRendering;
		
		
		public AbstractExportListBuilder (IReportModel reportModel,IItemsConverter itemsConverter)
		{
			this.ReportModel = reportModel;
			this.itemsConverter = itemsConverter;
		}
		
		
		public virtual void WritePages ()
		{
			
		}
		
		
		
		protected virtual Point BuildReportHeader (Point reportHeaderStart)
		{
			System.Console.WriteLine("BuildReportHeader at {0} with {1} items ",reportHeaderStart,ReportModel.ReportHeader.Items.Count);
			
			BaseSection section = ReportModel.ReportHeader;
			Size size = section.Size;
			Point retval = Point.Empty;
			
			if ((!CanGrow(section.Items)) | (!CanShrink(section.Items))) {
				ReportItemCollection result = itemsConverter.Convert(section,section.Items);
				this.SinglePage.Items.AddRange(result);
				
				retval = itemsConverter.LocationAfterConvert;
			} else {
				retval = new Point (reportHeaderStart.X , 150);
			}
			
			return retval;
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
		
		
		protected virtual void BuildPageHeader ()
		{
			System.Diagnostics.Trace.WriteLine(" BuildPageHeader");
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
		
		
		protected virtual void FinishRendering ()
		{
			
		}
		
		
		protected virtual void CreateNewPage ()
		{
			this.SinglePage = this.InitNewPage();
			
			
			PrintHelper.InitPage(this.SinglePage,this.ReportModel.ReportSettings);
			this.SinglePage.CalculatePageBounds(this.ReportModel);
			
//			this.pageFull = false;
		}
		
		
		
		
		private  PageDescription InitNewPage ()
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
			FirePageCreated(page);
		}
		
		
		#region Event's
		
		
		protected void FirePageCreated(PageDescription page)
		{
			EventHelper.Raise<NewPageCreatedEventArgs>(PageCreated,this,
			                                        new NewPageCreatedEventArgs(page));
		}
		
		
		protected void FireSectionRenderEvent (BaseSection section,int currentRow)
		{
			SectionRenderEventArgs ea =
				new SectionRenderEventArgs(section,
				                           pages.Count,
				                           currentRow,
				                           section);
			
			EventHelper.Raise<SectionRenderEventArgs>(SectionRendering,this,ea);
		}
		
		
		#endregion
		
		
		public PageDescription SinglePage {get;private set;}
		
		public IReportModel ReportModel {get;private set;}
		
		public	Point PositionAfterRenderSection {get;set;}
		
		
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
