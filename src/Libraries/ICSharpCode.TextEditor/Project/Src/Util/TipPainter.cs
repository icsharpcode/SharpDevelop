// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="?" email="?"/>
//     <version value="$version"/>
// </file>

using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

namespace ICSharpCode.TextEditor.Util
{
	sealed class TipPainter
	{
		const float HorizontalBorder = 2;
		const float VerticalBorder   = 1;
		static RectangleF workingArea = RectangleF.Empty;
		
		//static StringFormat centerTipFormat = CreateTipStringFormat();
		
		TipPainter()
		{
		}
		
		public static Size GetTipSize(Control control, Graphics graphics, Font font, string description)
		{
			return GetTipSize(control, graphics, new TipText (graphics, font, description));
		}
		
		public static Size GetTipSize(Control control, Graphics graphics, TipSection tipData)
		{
			Size tipSize = Size.Empty;
			SizeF tipSizeF = SizeF.Empty;
			
			if (workingArea == RectangleF.Empty) {
				Form ownerForm = control.FindForm();
				if (ownerForm.Owner != null) {
					ownerForm = ownerForm.Owner;
				}
				
				workingArea = Screen.GetWorkingArea(ownerForm);
			}
			
			PointF screenLocation = control.PointToScreen(Point.Empty);
			
			SizeF maxLayoutSize = new SizeF(workingArea.Right - screenLocation.X - HorizontalBorder * 2,
			                                workingArea.Bottom - screenLocation.Y - VerticalBorder * 2);
			
			if (maxLayoutSize.Width > 0 && maxLayoutSize.Height > 0) {
				graphics.TextRenderingHint =
				TextRenderingHint.AntiAliasGridFit;
				
				tipData.SetMaximumSize(maxLayoutSize);
				tipSizeF = tipData.GetRequiredSize();
				tipData.SetAllocatedSize(tipSizeF);
				
				tipSizeF += new SizeF(HorizontalBorder * 2,
				                      VerticalBorder   * 2);
				tipSize = Size.Ceiling(tipSizeF);
			}
			
			if (control.ClientSize != tipSize) {
				control.ClientSize = tipSize;
			}
			
			return tipSize;
		}
		
		public static Size DrawTip(Control control, Graphics graphics, Font font, string description)
		{
			return DrawTip(control, graphics, new TipText (graphics, font, description));
		}
		
		public static Size DrawTip(Control control, Graphics graphics, TipSection tipData)
		{
			Size tipSize = Size.Empty;
			SizeF tipSizeF = SizeF.Empty;
			
			PointF screenLocation = control.PointToScreen(Point.Empty);
			
			if (workingArea == RectangleF.Empty) {
				Form ownerForm = control.FindForm();
				if (ownerForm.Owner != null) {
					ownerForm = ownerForm.Owner;
				}
				
				workingArea = Screen.GetWorkingArea(ownerForm);
			}
	
			SizeF maxLayoutSize = new SizeF(workingArea.Right - screenLocation.X - HorizontalBorder * 2,
			                                workingArea.Bottom - screenLocation.Y - VerticalBorder * 2);
			
			if (maxLayoutSize.Width > 0 && maxLayoutSize.Height > 0) {
				graphics.TextRenderingHint =
				TextRenderingHint.AntiAliasGridFit;
				
				tipData.SetMaximumSize(maxLayoutSize);
				tipSizeF = tipData.GetRequiredSize();
				tipData.SetAllocatedSize(tipSizeF);
				
				tipSizeF += new SizeF(HorizontalBorder * 2,
				                      VerticalBorder   * 2);
				tipSize = Size.Ceiling(tipSizeF);
			}
			
			if (control.ClientSize != tipSize) {
				control.ClientSize = tipSize;
			}
			
			if (tipSize != Size.Empty) {
				Rectangle borderRectangle = new Rectangle
				(Point.Empty, tipSize - new Size(1, 1));
				
				RectangleF displayRectangle = new RectangleF
				(HorizontalBorder, VerticalBorder,
				 tipSizeF.Width - HorizontalBorder * 2,
				 tipSizeF.Height - VerticalBorder * 2);
				
				// DrawRectangle draws from Left to Left + Width. A bug? :-/
				graphics.DrawRectangle(SystemPens.WindowFrame,
				                       borderRectangle);
				tipData.Draw(new PointF(HorizontalBorder, VerticalBorder));
			}
			return tipSize;
		}
	}
}
