// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using ICSharpCode.Reports.Core.Exporter;
using ICSharpCode.Reports.Core.Interfaces;
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
			foreach (var item in container.Items)
			{
				item.BackColor = container.BackColor;
			}
		}
		
		
		public static void AdjustBackColor (ISimpleContainer container, Color defaultColor)
		{
			if (container.BackColor != defaultColor) {
				foreach (var item in container.Items)
				{
					item.BackColor = defaultColor;
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
			
			PrintHelper.AdjustChildLocation(item,offset);
			
			//BaseTextItem textItem = item as BaseTextItem;
			
			IReportExpression epr = item as IReportExpression;
			
			string evaluatedValue = String.Empty;
			
			if (epr != null) {
				try {
					if (!String.IsNullOrEmpty(epr.Expression))
					{
						evaluatedValue =  evaluator.Evaluate(epr.Expression);
						
					} else 
					{
						evaluatedValue =  evaluator.Evaluate(epr.Text);
					}
					epr.Text = evaluatedValue;
					
				} catch (UnknownFunctionException ufe) {
					
					epr.Text = GlobalValues.UnkownFunctionMessage(ufe.Message);
				}
				
			}
			item.Render (rpea);
			item.Location = saveLocation;
		}
		
		
		#endregion
		
		
		#region Render Collection
		
		public static Rectangle RenderPlainCollection (BaseReportItem parent,
		                                               ReportItemCollection items,
		                                               IExpressionEvaluatorFacade evaluator,
		                                               Point offset,
		                                               ReportPageEventArgs rpea)
		{
			
			if (items.Count > 0) {
				foreach (BaseReportItem child in items) {
					child.Parent = parent;
					 StandardPrinter.RenderLineItem (child,offset,evaluator,rpea);
				}
			}
			return new Rectangle(offset,parent.Size);
		}
		
		
		public static  ExporterCollection ConvertPlainCollection (ReportItemCollection items,Point offset)
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
			Point saveLocation = item.Location;
			PrintHelper.AdjustChildLocation(item,offset);
			
			item.Render(rpea);
			
			
			if (simpleContainer.Items != null)  {
				retVal = StandardPrinter.RenderPlainCollection(item,simpleContainer.Items,evaluator,offset,rpea);
			}
		
			retVal = new Rectangle (retVal.X,retVal.Y,
		                        retVal.X + item.Size.Width,
		                        item.Size.Height + 3 * GlobalValues.GapBetweenContainer);
			item.Location = saveLocation;
			return retVal;
		}
		
		
		public static ExportContainer ConvertToContainer (ISimpleContainer item,Point offset) 
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
		
		#region Evaluate
		
		
		public static IExpressionEvaluatorFacade  SetupEvaluator ()
		{
			return new ExpressionEvaluatorFacade(null);
		}
		
		
		public static IExpressionEvaluatorFacade  CreateEvaluator (ISinglePage singlePage,IDataNavigator dataNavigator)
		{
			if (singlePage == null) {
			
				throw new ArgumentNullException("singlePage");
			}
			if (dataNavigator == null) {
				throw new ArgumentNullException("dataNavigator");
			}
			singlePage.IDataNavigator = dataNavigator;
			IExpressionEvaluatorFacade evaluatorFacade = new ExpressionEvaluatorFacade(singlePage);
			return evaluatorFacade;
		}
		
		
		public static void EvaluateRow(IExpressionEvaluatorFacade evaluator,ExporterCollection row)
		{
			try {
				foreach (BaseExportColumn element in row) {
					ExportText textItem = element as ExportText;
					if (textItem != null) {
//						if (textItem.Text.StartsWith("=",StringComparison.InvariantCulture)) {
////							Console.WriteLine(textItem.Text);
//						}
						textItem.Text = evaluator.Evaluate(textItem.Text);
					}
				}
			} catch (Exception) {
				throw ;
			}
		}
		
		#endregion
	}
}
