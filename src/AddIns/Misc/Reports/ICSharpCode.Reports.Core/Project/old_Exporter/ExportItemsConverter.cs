// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using ICSharpCode.Reports.Core.Interfaces;

namespace ICSharpCode.Reports.Core.old_Exporter
{
	
	public class ExportItemsConverter:IExportItemsConverter
	{
	
		public ExportItemsConverter ()
		{
		}
		
		
		public ExporterCollection ConvertSimpleItems (BaseReportItem parent,Point offset,ReportItemCollection items)
		{
			if (items == null) {
				throw new ArgumentNullException("items");
			}
			ExporterCollection col = new ExporterCollection();
			if (items.Count > 0) {
				
				foreach(BaseReportItem item in items)
				{
					col.Add(ConvertToLineItem(parent,item,offset));
				}
			}
			return col;
		}
		
		/// <summary>
		/// Convert a single item, Location is calculated as follows
		/// (X = ParentRectangle.X + Item.X Y = offset.Y + Item.Y)
		/// </summary>
		/// <param name="offset"> only Y value is used, gives the offset to Items location.Y </param>
		/// <param name="item">Item to convert</param>
		/// <returns></returns>
		
//		protected Rectangle RenderPlainCollection (BaseReportItem parent,ReportItemCollection items, Point offset,ReportPageEventArgs rpea)
		
		
		private static BaseExportColumn ConvertToLineItem (BaseReportItem parent,BaseReportItem item,Point offset)
		{
			if (item == null) {
				throw new ArgumentNullException("item");
			}

			IExportColumnBuilder columnBuilder = item as IExportColumnBuilder;
			BaseExportColumn lineItem = null;
			
			if (columnBuilder != null) {
				lineItem = columnBuilder.CreateExportColumn();
				
				
				lineItem.StyleDecorator.Location = new Point(parent.Location.X + lineItem.StyleDecorator.Location.X,
				                                             lineItem.StyleDecorator.Location.Y + offset.Y);
				
				lineItem.StyleDecorator.DisplayRectangle = new Rectangle(lineItem.StyleDecorator.Location,
				                                                         lineItem.StyleDecorator.Size);
			} 
			return lineItem;
		}
		
		
		
		public ExportContainer ConvertToContainer (BaseReportItem parent,Point offset,ISimpleContainer item) 
		{
			if (item == null) {
				throw new ArgumentNullException("item");
			}
			IExportColumnBuilder lineBuilder = item as IExportColumnBuilder;
	
			if (lineBuilder != null) {
				ExportContainer lineItem = (ExportContainer)lineBuilder.CreateExportColumn();

				lineItem.StyleDecorator.Location = new Point (parent.Location.X + lineItem.StyleDecorator.Location.X,
				                                              offset.Y);
				lineItem.StyleDecorator.DisplayRectangle = new Rectangle(lineItem.StyleDecorator.Location,
				                                                         lineItem.StyleDecorator.Size);
				
				return lineItem;
			}
			return null;
		}
		
		public Rectangle ParentRectangle {get;set;}
		
	}
}
