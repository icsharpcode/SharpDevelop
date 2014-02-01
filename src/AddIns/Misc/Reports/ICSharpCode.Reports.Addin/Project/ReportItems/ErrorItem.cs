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
