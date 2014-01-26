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

namespace ICSharpCode.Reports.Core.Exporter
{
	/// <summary>
	/// Description of GapList.
	/// </summary>
	internal class GapList
	{
		public GapList()
		{
		}
		
		
		public void CalculateGapList (BaseSection section)
		{
			GapBetweenItems = new int[section.Items.Count +1];
			BaseReportItem oldItem = section.Items[0];
			for (int i = 0; i < section.Items.Count; i++) {
				GapBetweenItems[i] = CalculateGap(oldItem,section.Items[i]);
			}
			GapBetweenItems[section.Items.Count] = CalculateLastGap(section);
		}
		
		
		static int CalculateGap(BaseReportItem oldItem, BaseReportItem item)
		{
			if (oldItem == item) {
				return 0;
			}
			else
			{
				if (oldItem.Location.Y + oldItem.Size.Height < item.Location.Y ) {
					return item.Location.Y - (oldItem.Location.Y + oldItem.Size.Height) ;
				} else
				{
					return 0;
				}
			}
		}
		
		
		static int CalculateLastGap(BaseSection section)
		{
			BaseReportItem last = section.Items[section.Items.Count -1];
			int sectionHeight = section.Size.Height;
			int bottom = last.Location.Y + last.Size.Height;
			return sectionHeight - bottom;
		}
		
		public int[] GapBetweenItems {get;private set;}
		
		public int LastGap
		{
			get
			{
				return GapBetweenItems[GapBetweenItems.Length -1];
			}
		}
	}
}
