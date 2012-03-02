/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 26.03.2011
 * Time: 18:03
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using ICSharpCode.Reports.Core.BaseClasses.Printing;
using ICSharpCode.Reports.Core.Interfaces;

namespace ICSharpCode.Reports.Core.Exporter
{
	/// <summary>
	/// Description of ExportHelper.
	/// </summary>
	internal static class ExportHelper
	{
		
		public static IBaseExportColumn ConvertLineItem (BaseReportItem item,Point offset)
		{
			if (item == null) {
				throw new ArgumentNullException("item");
			}

//			if (item.VisibleInReport == true) {
				
				var columnBuilder = item as IExportColumnBuilder;
				IBaseExportColumn lineItem = null;
				
				if (columnBuilder != null) {
					lineItem = columnBuilder.CreateExportColumn();
					lineItem.StyleDecorator.Location = new Point(offset.X + lineItem.StyleDecorator.Location.X,
					                                             lineItem.StyleDecorator.Location.Y + offset.Y);
					
					lineItem.StyleDecorator.DisplayRectangle = new Rectangle(lineItem.StyleDecorator.Location,
					                                                         lineItem.StyleDecorator.Size);
				}
				return lineItem;
//			} else
//			{
//				return null;
//			}
		}
		
		
		public static  ExporterCollection ConvertPlainCollection (ReportItemCollection items,Point offset)
		{
			if (items == null) {
				throw new ArgumentNullException("items");
			}
			Console.WriteLine("Convert plain collection");
			ExporterCollection col = new ExporterCollection();
			if (items.Count > 0) {
				items.SortByLocation();
				foreach(BaseReportItem item in items)
				{
					var converteditem = ExportHelper.ConvertLineItem(item,offset);
					Console.WriteLine("{0} - {1}",converteditem.ToString(),converteditem.StyleDecorator.DisplayRectangle);
					col.Add((BaseExportColumn)converteditem);
				}
			}
			Console.WriteLine("");
			return col;
		}
		
		
		public static ExportContainer ConvertToContainer (ISimpleContainer container,Point offset) 
		{
			if (container == null) {
				throw new ArgumentNullException("item");
			}
			
			PrintHelper.AdjustParent(container,container.Items);
			IExportColumnBuilder lineBuilder = container as IExportColumnBuilder;
	
			if (lineBuilder != null) {
				ExportContainer lineItem = (ExportContainer)lineBuilder.CreateExportColumn();
				
				lineItem.StyleDecorator.Location = new Point (offset.X + lineItem.StyleDecorator.Location.X,
				                                              offset.Y);
				
				lineItem.StyleDecorator.DisplayRectangle = new Rectangle(lineItem.StyleDecorator.Location,
				                                                         lineItem.StyleDecorator.Size);
				
				StandardPrinter.AdjustBackColor (container);
				return lineItem;
			}
			
			return null;
		}
	}
}
