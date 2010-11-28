/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 10.10.2010
 * Time: 17:37
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using ICSharpCode.Reports.Core.BaseClasses.Printing;
using ICSharpCode.Reports.Core.Interfaces;
using ICSharpCode.Reports.Expressions.ReportingLanguage;

namespace ICSharpCode.Reports.Core
{
	/// <summary>
	/// Description of RenderTable.
	/// </summary>
	public class RenderTable:IBaseRenderer
	{

		IDataNavigator dataNavigator;
		Rectangle parentRectangle;
		ISinglePage singlePage;
		ILayouter layouter;
		ReportPageEventArgs reportPageEventArgs;
		BaseSection currentSection;
		
		public RenderTable(IDataNavigator datanavigator,Rectangle parentRectangle,ISinglePage singlePage,ILayouter layouter,BaseSection section)
		{
			this.dataNavigator = datanavigator;
			
			this.parentRectangle = parentRectangle;
			this.singlePage = singlePage;
			this.layouter = layouter;
			this.currentSection = section;
			
		}
		
		
		public event EventHandler<ICSharpCode.Reports.Core.Events.NewPageEventArgs> PageFull;
		
		public event EventHandler<SectionRenderEventArgs> SectionRendering;
		
		
		public void Render (ISimpleContainer table,ReportPageEventArgs rpea,IExpressionEvaluatorFacade evaluator)
		{
			if (this.dataNavigator.CurrentRow < 0 ) {
				this.dataNavigator.MoveNext();
			}
			this.reportPageEventArgs = rpea;
			Point saveLocation = table.Location;
			Rectangle pageBreakRect = Rectangle.Empty;
			
			Point currentPosition = new Point(PrintHelper.DrawingAreaRelativeToParent(this.currentSection,table).Location.X,
			                                  this.currentSection.Location.Y);
			table.Items.SortByLocation();
			
			Size rs = Size.Empty;
			
			
			ISimpleContainer headerRow = null;
			
			var simpleContainer = table.Items[0] as ISimpleContainer;
			
//			foreach (BaseRowItem row in table.Items)
//			{
			rs = simpleContainer.Size;
			PrintHelper.AdjustParent(table as BaseReportItem,table.Items);
			
//				if (PrintHelper.IsTextOnlyRow(simpleContainer) )
//				{
			
			PrintHelper.SetLayoutForRow(ReportPageEventArgs.PrintPageEventArgs.Graphics,Layouter,simpleContainer);
			
			var r =  StandardPrinter.RenderContainer(simpleContainer,evaluator,currentPosition,ReportPageEventArgs);
			
			currentPosition = PrintHelper.ConvertRectangleToCurentPosition (r);
			
			table.Location = saveLocation;
//				}
//				else {
			//---------------
			simpleContainer = table.Items[1] as ISimpleContainer;
			
			int adjust = simpleContainer.Location.Y - saveLocation.Y;
			simpleContainer.Location = new Point(simpleContainer.Location.X,simpleContainer.Location.Y - adjust - 3 * GlobalValues.GapBetweenContainer);
			rs = simpleContainer.Size;
			
			do {
				
				pageBreakRect = PrintHelper.CalculatePageBreakRectangle((BaseReportItem)table,currentPosition);
				
				if (PrintHelper.IsPageFull(pageBreakRect,this.SectionBounds)) {
					Console.WriteLine("PageBreak - PageFull");
					table.Location = saveLocation;
					AbstractRenderer.PageBreak(ReportPageEventArgs);
					return;
				}
				
				this.dataNavigator.Fill(simpleContainer.Items);
				
				PrintHelper.SetLayoutForRow(ReportPageEventArgs.PrintPageEventArgs.Graphics,Layouter,simpleContainer);
				
				r =  StandardPrinter.RenderContainer(simpleContainer,evaluator,currentPosition,ReportPageEventArgs);
				
				currentPosition = PrintHelper.ConvertRectangleToCurentPosition (r);

				simpleContainer.Size = rs;
			}
			while (this.dataNavigator.MoveNext());
			//-----
//				}
		}
		
		

		
		public void old_Render (ISimpleContainer table,ReportPageEventArgs rpea,IExpressionEvaluatorFacade evaluator)
		{
			if (this.dataNavigator.CurrentRow < 0 ) {
				this.dataNavigator.MoveNext();
			}
			this.reportPageEventArgs = rpea;
			Point saveLocation = table.Location;
			Rectangle pageBreakRect = Rectangle.Empty;
			
			Point currentPosition = new Point(PrintHelper.DrawingAreaRelativeToParent(this.currentSection,table).Location.X,
			                                  this.currentSection.Location.Y);
			table.Items.SortByLocation();
			
			Size rs = Size.Empty;
			
			
			
			
			foreach (BaseRowItem row in table.Items)
			{
				rs = row.Size;
				PrintHelper.AdjustParent(table as BaseReportItem,table.Items);
				
				if (PrintHelper.IsTextOnlyRow(row) )
				{
					
					PrintHelper.SetLayoutForRow(ReportPageEventArgs.PrintPageEventArgs.Graphics,Layouter,row);
					
					Rectangle r =  StandardPrinter.RenderContainer(row,evaluator,currentPosition,ReportPageEventArgs);
					
					currentPosition = PrintHelper.ConvertRectangleToCurentPosition (r);
					
					table.Location = saveLocation;
				}
				else {
					int adjust = row.Location.Y - saveLocation.Y;
					row.Location = new Point(row.Location.X,row.Location.Y - adjust - 3 * GlobalValues.GapBetweenContainer);
					rs = row.Size;
					
					do {
						
						pageBreakRect = PrintHelper.CalculatePageBreakRectangle((BaseReportItem)table,currentPosition);
						
						if (PrintHelper.IsPageFull(parentRectangle,this.SectionBounds)) {
							Console.WriteLine("PageBreak - PageFull");
							table.Location = saveLocation;
							AbstractRenderer.PageBreak(ReportPageEventArgs);
							return;
						}
						
						this.dataNavigator.Fill(row.Items);
						
						PrintHelper.SetLayoutForRow(ReportPageEventArgs.PrintPageEventArgs.Graphics,Layouter,row);
						
						Rectangle r =  StandardPrinter.RenderContainer(row,evaluator,currentPosition,ReportPageEventArgs);
						
						currentPosition = PrintHelper.ConvertRectangleToCurentPosition (r);

						row.Size = rs;
					}
					while (this.dataNavigator.MoveNext());
					
				}
				
				row.Size = rs;
			}
//			base.NotifyAfterPrint (rpea.LocationAfterDraw);
		}
		
		
		
		public ICSharpCode.Reports.Core.BaseClasses.SectionBounds SectionBounds {
			get {
				return this.singlePage.SectionBounds;
			}
		}
		
		public IDataNavigator DataNavigator {
			get {
				return this.dataNavigator;
			}
		}
		
		public System.Drawing.Rectangle ParentRectangle {
			get {
				return this.parentRectangle;
			}
		}
		
		public ISinglePage SinglePage {
			get {
				return this.singlePage;
			}
		}
		
		public ILayouter Layouter {
			get {
				return this.layouter;
			}
		}
		
		public System.Drawing.Graphics Graphics {get;set;}
		
		
		public ReportPageEventArgs ReportPageEventArgs {
			get { return reportPageEventArgs; }
			set { reportPageEventArgs = value; }
		}
		
	}
}
