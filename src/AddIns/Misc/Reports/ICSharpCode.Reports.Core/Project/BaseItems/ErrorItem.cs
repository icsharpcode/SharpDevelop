// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using ICSharpCode.Reports.Core.Exporter;

namespace ICSharpCode.Reports.Core
{
	/// <summary>
	/// Description of ErrorItem.
	/// </summary>
	public class ErrorItem:BaseTextItem,IExportColumnBuilder
	{
		string errMess;
		
		public ErrorItem():base()
		{
		}
		
		private void SetErrorLayout ()
		{
			base.DrawBorder = true;
			base.ForeColor = Color.Red;
			this.errMess = String.Format(System.Globalization.CultureInfo.CurrentCulture,
			                             "Error : <{0}> is missing or obsolete",base.Text);
		}
		
		
		public override void Render(ReportPageEventArgs rpea)
		{	
			if (rpea == null) {
				throw new ArgumentNullException("rpea");
			}
			
			this.SetErrorLayout();
			Border b = new Border(new BaseLine (this.ForeColor,System.Drawing.Drawing2D.DashStyle.Solid,1));
			base.DrawFrame(rpea.PrintPageEventArgs.Graphics,b);
			Print (rpea,this.errMess,base.DisplayRectangle);
		}
		
		#region IExportColumnBuilder  implementation
		
		public override IBaseExportColumn CreateExportColumn(){
			this.SetErrorLayout();
			TextStyleDecorator st = base.CreateItemStyle();
			ExportText item = new ExportText(st);
			item.Text = this.errMess;
			return item;
		}
		
		#endregion
	}
}
