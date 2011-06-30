// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using ICSharpCode.Reports.Core.BaseClasses.Printing;

namespace ICSharpCode.Reports.Addin.Designer
{
	/// <summary>
	/// Description of ErrorItem.
	/// </summary>
	public class ErrorItem:BaseTextItem
	{
		public ErrorItem()
		{
		}
		
		
		[System.ComponentModel.EditorBrowsableAttribute()]
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
		{
			base.OnPaint(e);
			this.Draw(e.Graphics);
		}
	
		
		public override void Draw(Graphics graphics)
		{
			if (graphics == null) {
				throw new ArgumentNullException("graphics");
			}
			
			using (Brush b = new SolidBrush(this.BackColor)){
				graphics.FillRectangle(b, base.DrawingRectangle);
			}
			
			string s = String.Format(System.Globalization.CultureInfo.CurrentCulture,
			                         "Error : <{0}> is missing or obsolete",base.Text);
			
			StringFormat stringFormat = TextDrawer.BuildStringFormat(base.StringTrimming,base.ContentAlignment);
			TextDrawer.DrawString(graphics,s,this.Font,
			                      new SolidBrush(Color.Red),
			                      this.ClientRectangle,
			                      stringFormat);
			base.DrawControl(graphics,base.DrawingRectangle);
		}
	}
}
