// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using ICSharpCode.Reporting.Interfaces.Export;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;

namespace ICSharpCode.Reporting.Items{
	/// <summary>
	/// Description of BaseLineItem.
	/// </summary>
	public class BaseLineItem:BaseGraphics
	{
		public BaseLineItem()
		{
		}
		
		public override IExportColumn CreateExportColumn()
		{
			var ex = new ExportLine();
			ex.Location = Location;
			ex.ForeColor = ForeColor;
			ex.BackColor = BackColor;
			ex.Size = Size;
			ex.DesiredSize = Size;
			ex.Thickness = Thickness;
			ex.DashStyle = DashStyle;
			ex.StartLineCap = StartLineCap;
			ex.EndLineCap = EndLineCap;
			return ex;
		}
	}
}
