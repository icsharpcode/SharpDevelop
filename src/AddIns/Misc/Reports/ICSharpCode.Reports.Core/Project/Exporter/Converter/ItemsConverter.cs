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
			foreach (BaseReportItem element in itemsSource) {
				
				col.Add(element);
			}
			this.locationAfterConvert = new Point(parent.Size.Width,parent.Size.Height);
			return col;
		}
		
		
		public Point LocationAfterConvert {
			get { return locationAfterConvert; }
		}
	}
	
}
