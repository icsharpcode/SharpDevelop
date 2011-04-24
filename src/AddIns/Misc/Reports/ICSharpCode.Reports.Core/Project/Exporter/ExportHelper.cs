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
using ICSharpCode.Reports.Core.Exporter;
using ICSharpCode.Reports.Core.Globals;
using ICSharpCode.Reports.Core.Interfaces;

namespace ICSharpCode.Reports.Core.Project.Exporter
{
	/// <summary>
	/// Description of ExportHelper.
	/// </summary>
	internal static class ExportHelper
	{
		
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
		
		
		public static  ExporterCollection ConvertPlainCollection (ReportItemCollection items,Point offset)
		{
			if (items == null) {
				throw new ArgumentNullException("items");
			}
			ExporterCollection col = new ExporterCollection();
			if (items.Count > 0) {
				
				foreach(BaseReportItem item in items)
				{
					col.Add(ExportHelper.ConvertLineItem(item,offset));
				}
			}
			return col;
		}
		
		/*
		public static Point   ConvertPlainCollection_2 (ExportContainer container,ReportItemCollection items,Point offset)
		{
			if (items == null) {
				throw new ArgumentNullException("items");
			}
			
			ExporterCollection col = new ExporterCollection();
			Point o = offset;
			if (items.Count > 0) {
				
				foreach(BaseReportItem item in items)
				{
					container.Items.Add(ExportHelper.ConvertLineItem(item,offset));
				}
				Size max = Size.Empty;
				foreach (var element in items) {
					if (element.Size.Height > max.Height) {
						max = element.Size;
					}
				}
				
				if (container.StyleDecorator.Size.Height > max.Height) {
					offset = new Point (o.X,o.Y + container.StyleDecorator.Size.Height + GlobalValues.GapBetweenContainer);
				}
				
			}
			return offset;
		}
		
		*/
		
		
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
