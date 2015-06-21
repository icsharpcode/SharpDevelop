/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 21.06.2015
 * Time: 17:01
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;

namespace ICSharpCode.Reporting.Items
{
	/// <summary>
	/// Description of ExportExtension.
	/// </summary>
	public static class ExportExtension
	{
		public static void ToExportItem (this ExportColumn export, PrintableItem item) {
			export.Name = item.Name;
			export.Location = item.Location;
			export.Size = item.Size;
			export.ForeColor = item.ForeColor;
			export.FrameColor = item.FrameColor;
			export.BackColor = item.BackColor;
			export.CanGrow = item.CanGrow;
			export.DrawBorder = item.DrawBorder;
		}	
	}
}
