//
// SharpDevelop ReportEditor
//
// Copyright (C) 2005 Peter Forstmeier
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// Peter Forstmeier (Peter.Forstmeier@t-online.de)

using System;
using System.Drawing;

/// <summary>
/// Utility Class to help with printing
/// </summary>
/// <remarks>
/// 	created by - Forstmeier Peter
/// 	created on - 26.03.2005 10:08:23
/// </remarks>
namespace SharpReportCore {
	public class a_PrintingUtilities : object {
		
		/// <summary>
		/// Measure the Size of the String
		/// </summary>
		/// <param name="item">ReportItem to Measure</param>
		/// <param name="e">ReportPageEventArgs</param>
		/// <returns>SizeF</returns>
		public static System.Drawing.SizeF a_MeasureReportItem(IItemRenderer item,
		                                                    ReportPageEventArgs e) {
			SizeF sizeF = new SizeF ();
			BaseTextItem myItem = item as BaseTextItem;
			if (myItem != null) {
				string str = String.Empty;
				if (item is BaseTextItem) {
					BaseTextItem it = item as BaseTextItem;
					str = it.Text;
				} else if(item is BaseDataItem) {
					BaseDataItem it = item as BaseDataItem;
					str = it.DbValue;
				}
		// TODO need a much better way		
				sizeF = e.PrintPageEventArgs.Graphics.MeasureString(str,
				                                                    myItem.Font,
				                                                    myItem.Size.Width,
				                                                    GlobalValues.StandartStringFormat());
			} else {
				sizeF = new SizeF (item.Size.Width,item.Size.Height);
			}
		
			return sizeF;
		}
	}
}
