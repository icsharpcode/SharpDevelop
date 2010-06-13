/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 21.05.2010
 * Time: 19:35
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using iTextSharp.text;

namespace ICSharpCode.Reports.Core.Exporter.Converter
{
	
	public interface IItemsConverter
	{
		ReportItemCollection Convert (BaseReportItem parent,IEnumerable<BaseReportItem> itemsSource);
		Point LocationAfterConvert {get;}
	}
		
	
	/// <summary>
	/// Description of ItemsConverterr.
	/// </summary>
	
	
	public class ItemsConverter:IItemsConverter
	{
		Point locationAfterConvert;
		
		
		
		public ItemsConverter()
		{
		}
		
		
		
		public ReportItemCollection Convert (BaseReportItem parent,IEnumerable<BaseReportItem> itemsSource)
		{
			var col =  new ReportItemCollection();	
			foreach (BaseReportItem item in itemsSource) {
				item.Location = AdjustLocation (parent,item);
				col.Add(item);
			}
			this.locationAfterConvert = AdjustLocationAfterDraw(parent);
			return col;
		}
		
		
		
		
		private static Point AdjustLocation (IReportItem parent , IReportItem item)
		{
			return new Point(parent.Location.X + item.Location.X,parent.Location.Y + item.Location.Y);
		}
			
		
		private static Point AdjustLocationAfterDraw (IReportItem parent)
		{
			return new Point(parent.Location.X + parent.Size.Width,parent.Location.Y + parent.Size.Height);
		}
		
		
		public Point LocationAfterConvert {
			get { return locationAfterConvert; }
		}
	}
	
}
