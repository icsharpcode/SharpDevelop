/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 19.06.2011
 * Time: 19:34
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
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
