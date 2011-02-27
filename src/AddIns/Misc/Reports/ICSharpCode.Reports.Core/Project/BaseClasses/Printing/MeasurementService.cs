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
				
				/*
				if (str.Contains(Environment.NewLine))
				{
				    	Console.WriteLine ("newline");
				}
				
				
				int lp = 0;
				int cp = 0;
				SizeF test = graphics.MeasureString(str.TrimEnd(), textItem.Font, textItem.Size, 
				
				                                      StringFormat.GenericTypographic,out cp, out lp);

				
				*/
				SizeF sf = graphics.MeasureString(str.TrimEnd(),
				                                  textItem.Font,
				                                  textItem.Size.Width);
				var hh = textItem.Font.GetHeight();
				var h1 = textItem.Font.Height;
				var aa = sf.Height / textItem.Font.GetHeight();
				
				return sf.ToSize();
			}
			return item.Size;
		}
		
	}
}
