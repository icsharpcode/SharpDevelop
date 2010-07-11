/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 20.06.2010
 * Time: 19:04
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using ICSharpCode.Reports.Core.Interfaces;
using ICSharpCode.Reports.Core.old_Exporter;
using ICSharpCode.Reports.Expressions.ReportingLanguage;

namespace ICSharpCode.Reports.Core.BaseClasses.Printing
{
	/// <summary>
	/// Description of StandardPrinter.
	/// </summary>
	internal sealed class StandardPrinter
	{
		public StandardPrinter()
		{
		}
		
		
		public static void AdjustBackColor (ISimpleContainer container)
		{
			
			BaseReportItem parent = container as BaseReportItem;
			if (parent.BackColor != GlobalValues.DefaultBackColor) {
				foreach (BaseReportItem item in container.Items)
				{
					item.BackColor = parent.BackColor;
				}
			}
		}
		
		
		public static void FillBackground (Graphics  graphics,BaseStyleDecorator decorator)
		{
			if (decorator.BackColor != GlobalValues.DefaultBackColor) {
				
				RectangleShape backgroundShape = new RectangleShape();
				
				backgroundShape.FillShape(graphics,
				                          new SolidFillPattern(decorator.BackColor),
				                          decorator.DisplayRectangle);
			}
		}
		
		
		
		public static void DrawBorder (Graphics graphics,BaseStyleDecorator decorator)
		{
			if (decorator.DrawBorder)
			{
				if (decorator.FrameColor == Color.Empty)
				{
					decorator.FrameColor = decorator.ForeColor;
				}
				Border border = new Border(new BaseLine (decorator.FrameColor,System.Drawing.Drawing2D.DashStyle.Solid,1));

				border.DrawBorder(graphics,decorator.DisplayRectangle);
			}
		}
		
		
		#region Render/Convert SimpleItem
		
		/// <summary>
		/// Convert a single item, Location is calculated as follows
		/// </summary>
		/// <param name="offset"> only Y value is used, gives the offset to Items location.Y </param>
		/// <param name="item">Item to convert</param>
		/// <returns></returns>
		
		public static BaseExportColumn ConvertLineItem (BaseReportItem item,Point offset)
		{
			if (item == null) {
				throw new ArgumentNullException("item");
			}

			IExportColumnBuilder columnBuilder = item as IExportColumnBuilder;
			BaseExportColumn lineItem = null;
			
			if (columnBuilder != null) {
				lineItem = columnBuilder.CreateExportColumn();
				
				
				lineItem.StyleDecorator.Location = new Point(offset.X + lineItem.StyleDecorator.Location.X,
				                                             lineItem.StyleDecorator.Location.Y + offset.Y);
				
				lineItem.StyleDecorator.DisplayRectangle = new Rectangle(lineItem.StyleDecorator.Location,
				                                                         lineItem.StyleDecorator.Size);
				
			} 
			return lineItem;
		}
		
		
		private static void RenderLineItem (BaseReportItem item, Point offset,IExpressionEvaluatorFacade evaluator,ReportPageEventArgs rpea)
		{
			
			Point saveLocation = new Point (item.Location.X,item.Location.Y);
			
			
			item.Location = new Point(offset.X + item.Location.X,
			                           offset.Y + item.Location.Y);
			
			
			BaseTextItem textItem = item as BaseTextItem;
			
			if (textItem != null) {
				string str = textItem.Text;
				textItem.Text = evaluator.Evaluate(textItem.Text);
				textItem.Render(rpea);
				textItem.Text = str;
			} else {
				item.Render (rpea);
			}
			item.Location = saveLocation;
		}
		
		
		#endregion
		
		
		#region Render Collection
		
		public static Rectangle RenderPlainCollection (BaseReportItem parent,ReportItemCollection items,IExpressionEvaluatorFacade evaluator, Point offset,ReportPageEventArgs rpea)
		{
			Rectangle retVal = Rectangle.Empty;
			Size size = Size.Empty;
			
			if (items.Count > 0) {
				foreach (BaseReportItem child in items) {
					child.Parent = parent;
					 StandardPrinter.RenderLineItem (child,offset,evaluator,rpea);
				}
				
				retVal = new Rectangle(offset,size);
				return retVal;
				
			} else {
				retVal = new Rectangle(offset.X,offset.Y,0,0);
				return retVal;
			}
		}
		
		
		public static  ExporterCollection ConvertPlainCollection (BaseReportItem parent,ReportItemCollection items,Point offset)
		{
			if (items == null) {
				throw new ArgumentNullException("items");
			}
			ExporterCollection col = new ExporterCollection();
			if (items.Count > 0) {
				
				foreach(BaseReportItem item in items)
				{
					col.Add(StandardPrinter.ConvertLineItem(item,offset));
				}
			}
			return col;
		}
		
		#endregion
	
		#region Container
		
		
		public static Rectangle RenderContainer (ISimpleContainer simpleContainer,IExpressionEvaluatorFacade evaluator,Point offset,ReportPageEventArgs rpea)
		{
			BaseReportItem item = simpleContainer as BaseReportItem;
			Rectangle retVal = new Rectangle(offset,item.Size);
			Point loc = item.Location;
			item.Location = new Point (offset.X + item.Location.X,offset.Y + item.Location.Y);
			
			item.Render(rpea);
			
			if (simpleContainer.Items != null)  {
				retVal = StandardPrinter.RenderPlainCollection(item,simpleContainer.Items,evaluator,offset,rpea);
			}
		
			retVal = new Rectangle (retVal.X,retVal.Y,
		                        retVal.X + item.Size.Width,
		                        item.Size.Height + 3 * GlobalValues.GapBetweenContainer);
			item.Location = loc;
			return retVal;
		}
		
		
		public static ExportContainer ConvertToContainer (BaseReportItem parent,ISimpleContainer item,Point offset) 
		{
			if (item == null) {
				throw new ArgumentNullException("item");
			}
			
			IExportColumnBuilder lineBuilder = item as IExportColumnBuilder;
	
			if (lineBuilder != null) {
				ExportContainer lineItem = (ExportContainer)lineBuilder.CreateExportColumn();
				
				lineItem.StyleDecorator.Location = new Point (offset.X + lineItem.StyleDecorator.Location.X,
				                                              offset.Y);
				
				lineItem.StyleDecorator.DisplayRectangle = new Rectangle(lineItem.StyleDecorator.Location,
				                                                         lineItem.StyleDecorator.Size);
				
				return lineItem;
			}
			return null;
		}
		#endregion
	}
}
