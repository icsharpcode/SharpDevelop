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
