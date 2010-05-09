// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using ICSharpCode.Reports.Core.Interfaces;

namespace ICSharpCode.Reports.Core
{
	/// <summary>
	/// Description of MeasurementService.
	/// </summary>
	
	
	internal static class MeasurementService
	{
		
		public static  Size MeasureReportItem(Graphics graphics,IReportItem item)
		{
			BaseTextItem textItem = item as BaseTextItem;
			if (textItem != null) {
				string str = String.Empty;
				BaseDataItem dataItem = item as BaseDataItem;
				if (dataItem != null) {
					str = dataItem.DBValue;
				} else {
					BaseTextItem it = item as BaseTextItem;
					if (it != null) {
						str = it.Text;
					}
				}
				
				SizeF sf = graphics.MeasureString(str.TrimEnd(),
				                                  textItem.Font,
				                                  textItem.Size.Width);
				return sf.ToSize();
			}
			return item.Size;
		}
		
	}
}
