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
