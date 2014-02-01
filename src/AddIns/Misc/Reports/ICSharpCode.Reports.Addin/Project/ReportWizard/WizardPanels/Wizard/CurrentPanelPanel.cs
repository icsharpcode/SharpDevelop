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
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using ICSharpCode.Core.WinForms;

namespace ICSharpCode.Reports.Addin.ReportWizard
{
	public class CurrentPanelPanel : UserControl
	{
		WizardDialog wizard;
		
		Font normalFont;
		
		
		public CurrentPanelPanel(WizardDialog wizard)
		{
			normalFont = WinFormsResourceService.LoadFont("SansSerif", 18, GraphicsUnit.World);

			this.wizard = wizard;
			Size = new Size(wizard.Width - 220, 30);
			ResizeRedraw  = false;
			
			SetStyle(ControlStyles.UserPaint, true);
		}
		
		protected override void OnPaintBackground(PaintEventArgs pe)
		{
			//    		base.OnPaintBackground(pe);
			Graphics g = pe.Graphics;
			//			g.FillRectangle(new SolidBrush(SystemColors.Control), pe.ClipRectangle);
			
			using (Brush brush = new LinearGradientBrush(new Point(0, 0), new Point(Width, Height),
			                                             Color.White,
			                                             SystemColors.Control)) {
				g.FillRectangle(brush, new Rectangle(0, 0, Width, Height));
			}
		}
		
		protected override void OnPaint(PaintEventArgs pe)
		{
			//    		base.OnPaint(pe);
			Graphics g = pe.Graphics;
			g.DrawString(((IDialogPanelDescriptor)wizard.WizardPanels[wizard.ActivePanelNumber]).Label, normalFont, Brushes.Black,
			             10,
			             24 - normalFont.Height,
			             StringFormat.GenericTypographic);
			g.DrawLine(Pens.Black, 10, 24, Width - 10, 24);
		}
	}
}
