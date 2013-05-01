/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 30.04.2013
 * Time: 19:44
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using ICSharpCode.Reporting.Items;

namespace ICSharpCode.Reporting.Globals
{
	/// <summary>
	/// Description of MeasurementService.
	/// </summary>
	internal class MeasurementService
	{
		
		public MeasurementService()
		{
		}
		
		public static Size Measure (ITextItem item,Graphics graphics) {
			
			
			if (!String.IsNullOrEmpty(item.Text)) {
				SizeF size = graphics.MeasureString(item.Text.TrimEnd(),
				                                    item.Font,
				                                    item.Size.Width);
				var i = (int)size.Height/item.Font.Height;
				Console.WriteLine("\t\tMeasure <{0}> - {1} - {2}",item.Text,item.Size,size);
				return size.ToSize();
			}
			
			return item.Size;
		}
	}
}
