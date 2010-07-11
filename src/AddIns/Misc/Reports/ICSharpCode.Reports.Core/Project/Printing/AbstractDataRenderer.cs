// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Drawing.Printing;

using ICSharpCode.Reports.Core.BaseClasses.Printing;
using ICSharpCode.Reports.Core.Interfaces;
using ICSharpCode.Reports.Expressions.ReportingLanguage;

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

			tableContainer.Items.SortByLocation();
			
			rpea.SinglePage.StartRow  = this.dataNavigator.CurrentRow;
			
			foreach (BaseRowItem row in tableContainer.Items)
			{
				if (row != null)
				{
					PrintHelper.AdjustParent(tableContainer as BaseReportItem,tableContainer.Items);
					
					if (PrintHelper.IsTextOnlyRow(row) )
					{
						
						Rectangle r =  StandardPrinter.RenderContainer(row,Evaluator,currentPosition,rpea);
						currentPosition =PrintHelper. ConvertRectangleToCurentPosition (r);
						
						currentPosition = new Point(parent.Location.X + row.Location.X,currentPosition.Y);
						tableContainer.Location = saveLocation;
					}
					else {
						int adjust = row.Location.Y - saveLocation.Y;
						row.Location = new Point(row.Location.X,row.Location.Y - adjust - 3 * GlobalValues.GapBetweenContainer);
						
						do {
							if (PrintHelper.IsPageFull(new Rectangle(currentPosition,row.Size),this.SectionBounds)) {
								tableContainer.Location = saveLocation;
							
								rpea.SinglePage.EndRow = this.dataNavigator.CurrentRow;
								AbstractRenderer.PageBreak(rpea);
								return;
							}
							this.dataNavigator.Fill(row.Items);

							Rectangle r =  StandardPrinter.RenderContainer(row,Evaluator,currentPosition,rpea);
							currentPosition = PrintHelper.ConvertRectangleToCurentPosition (r);
							
							currentPosition = new Point(parent.Location.X + row.Location.X,currentPosition.Y);
							
						}
						while (this.dataNavigator.MoveNext());
					}
				}
			}
		
//			
//			if (this.DrawBorder) {
//				Border border = new Border(new BaseLine (this.ForeColor,System.Drawing.Drawing2D.DashStyle.Solid,1));
//				border.DrawBorder(rpea.PrintPageEventArgs.Graphics,
//				                  new Rectangle(parent.Location.X,tableStart.Y,
//				                                parent.Size.Width,currentPosition.Y + 5));
//			}
		
			
			rpea.LocationAfterDraw = new Point(rpea.LocationAfterDraw.X,rpea.LocationAfterDraw.Y + 20);
//			base.NotifyAfterPrint (rpea.LocationAfterDraw);
		}
		
	
		
		protected Point RenderItems (ReportPageEventArgs rpea) 
		{
			
			base.SinglePage.IDataNavigator = this.dataNavigator;
			base.CurrentRow = this.dataNavigator.CurrentRow;
			
			ISimpleContainer container = PrintHelper.FindContainer(this.CurrentSection.Items);
		
			if (container != null) {
				
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

					ISimpleContainer con = item as ISimpleContainer;
					if (con != null) {
						Rectangle r = StandardPrinter.RenderContainer(container,Evaluator,offset,rpea);
						currentPosition = PrintHelper.ConvertRectangleToCurentPosition(r);
					}
					
					else
					{
						
						item.SectionOffset = section.SectionOffset;
						Point saveLocation = item.Location;
						item.Render(rpea);
						
						item.Location = saveLocation;
						
						ISimpleContainer cont = item as ISimpleContainer;

						Rectangle r =  StandardPrinter.RenderContainer(cont,Evaluator,currentPosition,rpea);
						currentPosition = PrintHelper.ConvertRectangleToCurentPosition (r);
						
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
			return currentPosition;
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


