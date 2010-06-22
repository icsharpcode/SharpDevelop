// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Drawing.Printing;
using ICSharpCode.Reports.Core.Interfaces;

namespace ICSharpCode.Reports.Core
{
	/// <summary>
	/// Description of AbstractDataRenderer.
	/// </summary>
	public class AbstractDataRenderer : AbstractRenderer
	{
		IDataManager dataManager;
		DataNavigator dataNavigator;
		
		public AbstractDataRenderer(IReportModel model,
		                            IDataManager dataManager,
		                            ReportDocument reportDocument,
		                            ILayouter layout):base(model,reportDocument,layout)
		                           
		{
			if (dataManager == null) {
				throw new ArgumentNullException("dataManager");
			}
			this.dataManager = dataManager;
		}
		
		#region overrides
		
		internal override void ReportBegin(object sender, PrintEventArgs pea)
		{
			base.ReportBegin(sender, pea);
		}
		
		
		internal override void ReportQueryPage(object sender, QueryPageSettingsEventArgs qpea)
		{
			base.ReportQueryPage(sender, qpea);
			base.SinglePage.IDataNavigator = this.dataNavigator;
		}
		
		
		protected override Point RenderSection(ReportPageEventArgs rpea)
		{
			return new System.Drawing.Point(0,0);
		}
		
		
		#endregion
		
		
		
		protected void RenderTable (BaseReportItem parent,ITableContainer tableContainer,ReportPageEventArgs rpea)
		{
			if (rpea == null) {
				throw new ArgumentNullException("rpea");
			}

			Point saveLocation = tableContainer.Location;
			
			Point currentPosition = new Point(this.CurrentSection.Location.X,this.CurrentSection.Location.Y);
			
			Point tableStart = currentPosition;

			
			int defaultLeftPos = PrintHelper.DrawingAreaRelativeToParent(parent,tableContainer).Left;
			
			tableContainer.Items.SortByLocation();
			
			rpea.SinglePage.StartRow  = this.dataNavigator.CurrentRow;
			foreach (BaseRowItem row in tableContainer.Items)
			{
				if (row != null)
				{
					PrintHelper.AdjustParent(tableContainer as BaseReportItem,tableContainer.Items);

					
					if (PrintHelper.IsTextOnlyRow(row) )
					{
						currentPosition = this.RenderContainer(row,currentPosition,rpea);
						currentPosition = new Point(parent.Location.X + row.Location.X,currentPosition.Y);
						tableContainer.Location = saveLocation;
					}
					else {
						do {
							if (PrintHelper.IsPageFull(new Rectangle(currentPosition,row.Size),this.SectionBounds)) {
								tableContainer.Location = saveLocation;
							
								rpea.SinglePage.EndRow = this.dataNavigator.CurrentRow;
								AbstractRenderer.PageBreak(rpea);
								return;
							}
							this.dataNavigator.Fill(row.Items);
							currentPosition = this.RenderContainer(row,currentPosition,rpea);
							currentPosition = new Point(parent.Location.X + row.Location.X,currentPosition.Y);
						}
						while (this.dataNavigator.MoveNext());
					}
				}
			}
			/*
			if (this.DrawBorder) {
				Border border = new Border(new BaseLine (this.ForeColor,System.Drawing.Drawing2D.DashStyle.Solid,1));
				border.DrawBorder(rpea.PrintPageEventArgs.Graphics,
				                  new Rectangle(parent.Location.X,tableStart.Y,
				                                parent.Size.Width,currentPosition.Y + 5));
			}
			*/
			
			rpea.LocationAfterDraw = new Point(rpea.LocationAfterDraw.X,rpea.LocationAfterDraw.Y + 20);
//			base.NotifyAfterPrint (rpea.LocationAfterDraw);
		}
		
		
		
		protected Point RenderItems (ReportPageEventArgs rpea) 
		{
			base.SinglePage.IDataNavigator = this.dataNavigator;
			base.CurrentRow = this.dataNavigator.CurrentRow;
			ISimpleContainer container = null;
			bool hasContainer = false;
			
			foreach (BaseReportItem item in this.CurrentSection.Items) {
				 container = item as ISimpleContainer;
				if (container != null) {
					hasContainer = true;
					break;
				}
			}
			if (hasContainer) {
				return RenderSectionWithSimpleContainer(this.CurrentSection,container,
				                                        new Point(CurrentSection.Location.X,CurrentSection.SectionOffset),
				                                        rpea);
				
			} else {
				return base.RenderSection(rpea);
			}
		}
		
		
		
		private  Point RenderSectionWithSimpleContainer (BaseSection section,
		                                                 ISimpleContainer container,
		                                                 Point offset,
		                                                 ReportPageEventArgs rpea)
			
		{
			
			Point currentPosition	= new Point(section.Location.X + container.Location.X,offset.Y);
			
			if (section.Visible){
				
				//Always set section.size to it's original value
				
				section.Size = this.SectionBounds.DetailSectionRectangle.Size;
				
				Size containerSize = new Size (section.Items[0].Size.Width,section.Items[0].Size.Height);
				
				PrintHelper.SetLayoutForRow(rpea.PrintPageEventArgs.Graphics,base.Layout,container);
				
				section.Render (rpea);
				
				PrintHelper.AdjustParent(section,section.Items);
				
				foreach (BaseReportItem item in section.Items) {

					item.SectionOffset = section.SectionOffset;
					Point saveLocation = item.Location;
					item.Render(rpea);
					
					item.Location = saveLocation;
					
					ISimpleContainer cont = item as ISimpleContainer;

					currentPosition = RenderContainer (cont,currentPosition,rpea);
					item.Location = saveLocation;
					currentPosition = new Point(item.Location.X,
					                            section.SectionOffset + section.Size.Height);
					
					rpea.LocationAfterDraw = new Point (rpea.LocationAfterDraw.X,section.SectionOffset + section.Size.Height);
				}
				
				if ((section.CanGrow == false)&& (section.CanShrink == false)) {
					return new Point(section.Location.X,section.Size.Height);
				}
				section.Items[0].Size = containerSize;
				return currentPosition;
			}
			
			return currentPosition;
		}
		
		
		
		private Point RenderContainer (ISimpleContainer simpleContainer,Point offset,ReportPageEventArgs rpea)
		{
			
			BaseReportItem item = simpleContainer as BaseReportItem;
			Point retVal = offset;
			
			if (simpleContainer.Items != null)  {	
				
				retVal = base.RenderPlainCollection(item,simpleContainer.Items,offset,rpea);
				
			}
			return retVal;
		}
		
		
		#region Properties
		
		protected IDataManager DataManager 
		{
			get {
				return dataManager;
			}
		}
		
		
		protected DataNavigator DataNavigator
		{
			get {
				return this.DataManager.GetNavigator;
			}
			set {this.dataNavigator = value;}
		}
		
		#endregion
		
		#region IDisposable
				
		protected override void Dispose(bool disposing)
		{
			try {
				if (disposing)
			{
//				if (this.dataManager != null) {
//					this.dataManager.Dispose();
//					this.dataManager = null;
//				}
				if (this.dataNavigator != null) {
					this.dataNavigator= null;
				}
			}
			} finally {
				base.Dispose(disposing);
			}
			
		}
		#endregion
	}
}


