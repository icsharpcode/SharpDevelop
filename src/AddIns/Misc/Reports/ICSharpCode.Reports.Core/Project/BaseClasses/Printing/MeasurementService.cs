// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using ICSharpCode.Reports.Core.Interfaces;

namespace ICSharpCode.Reports.Core.BaseClasses.Printing
{
	/// <summary>
	/// Description of MeasurementService.
	/// </summary>
	
	
	public static class MeasurementService
	{

		public static  Size MeasureReportItem(Graphics graphics,IReportItem item)
		{
			
			BaseTextItem textItem = item as BaseTextItem;
			if (textItem != null)
			{
				string str = String.Empty;
				BaseDataItem dataItem = item as BaseDataItem;
				if (dataItem != null)
				{
					str = dataItem.DBValue;
				}
				else
				{
					BaseTextItem it = item as BaseTextItem;
					
					if (it != null)
					{
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
