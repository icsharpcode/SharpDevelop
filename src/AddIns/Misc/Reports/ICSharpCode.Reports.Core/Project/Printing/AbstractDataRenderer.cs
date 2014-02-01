// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Drawing;
using System.Drawing.Printing;

using ICSharpCode.Reports.Core.BaseClasses.Printing;
using ICSharpCode.Reports.Core.Globals;
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
			
			Point currentPosition = new Point(PrintHelper.DrawingAreaRelativeToParent(this.CurrentSection,tableContainer).Location.X,
			                                                                           this.CurrentSection.Location.Y);
			tableContainer.Items.SortByLocation();
			
			Size rs = Size.Empty;
			
			foreach (BaseRowItem row in tableContainer.Items)
			{
			
				if (row != null)
				{
					rs = row.Size;
					PrintHelper.AdjustParent(tableContainer,tableContainer.Items);
					
					if (PrintHelper.IsTextOnlyRow(row) )
					{
						
						LayoutHelper.SetLayoutForRow(rpea.PrintPageEventArgs.Graphics,base.Layout,row);
						
						Rectangle r =  StandardPrinter.RenderContainer(row,Evaluator,currentPosition,rpea);
						
						
						currentPosition = PrintHelper.ConvertRectangleToCurentPosition (r);
					
						tableContainer.Location = saveLocation;
					}
					else {
						int adjust = row.Location.Y - saveLocation.Y;
						row.Location = new Point(row.Location.X,row.Location.Y - adjust - 3 * GlobalValues.GapBetweenContainer);
						rs = row.Size;
						
						do {
							if (PrintHelper.IsPageFull(new Rectangle(currentPosition,row.Size),this.SectionBounds)) {
								tableContainer.Location = saveLocation;
								AbstractRenderer.PageBreak(rpea);
								return;
							}
							
							this.dataNavigator.Fill(row.Items);
							
							LayoutHelper.SetLayoutForRow(rpea.PrintPageEventArgs.Graphics,base.Layout,row);
							
							Rectangle r =  StandardPrinter.RenderContainer(row,Evaluator,currentPosition,rpea);
							
							currentPosition = PrintHelper.ConvertRectangleToCurentPosition (r);

							row.Size = rs;
						}
						while (this.dataNavigator.MoveNext());
						
					}
				}
				row.Size = rs;
			}
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
			
//			if (section.VisibleInReport)
//			{
			
			//Always set section.size to it's original value
			
			section.Size = this.SectionBounds.DetailSectionRectangle.Size;
			
			Size containerSize = new Size (section.Items[0].Size.Width,section.Items[0].Size.Height);
			
			LayoutHelper.SetLayoutForRow(rpea.PrintPageEventArgs.Graphics,base.Layout,container);
			
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
					
					rpea.LocationAfterDraw = new Point (rpea.LocationAfterDraw.X,section.SectionOffset + section.Size.Height);
					
				}
				
				section.Items[0].Size = containerSize;
				return currentPosition;
			}
			
			return currentPosition;
//			}
//			return currentPosition;
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
