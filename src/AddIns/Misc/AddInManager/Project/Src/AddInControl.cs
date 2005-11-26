// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using ICSharpCode.Core;

namespace ICSharpCode.AddInManager
{
	public class AddInControl : Control
	{
		AddIn addIn;
		
		public AddIn AddIn {
			get {
				return addIn;
			}
		}
		
		public AddInControl(AddIn addIn)
		{
			this.addIn = addIn;
			this.BackColor = SystemColors.Window;
			this.Size = new Size(100, 40);
			this.SetStyle(ControlStyles.Selectable, true);
			this.SetStyle(ControlStyles.UserPaint, true);
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			this.SetStyle(ControlStyles.ResizeRedraw, true);
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
		}
		
		bool selected;
		
		public bool Selected {
			get {
				return selected;
			}
			set {
				if (selected != value) {
					selected = value;
					Invalidate();
				}
			}
		}
		
		protected override void OnClick(EventArgs e)
		{
			base.OnClick(e);
			Focus();
		}
		
		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			Rectangle bounds = this.ClientRectangle;
			bounds.Offset(1, 1);
			bounds.Inflate(-2, -2);
			Brush gradient = new LinearGradientBrush(bounds,
			                                         selected ? SystemColors.Control   : SystemColors.ControlLightLight,
			                                         selected ? SystemColors.Highlight : SystemColors.ControlDark,
			                                         LinearGradientMode.ForwardDiagonal);
			
			GraphicsPath path = new GraphicsPath();
			
			const int egdeRadius = 3;
			const int innerMargin = egdeRadius + 2;
			
			RectangleF arcRect = new RectangleF(bounds.Location, new SizeF(egdeRadius * 2, egdeRadius * 2));
			//top left Arc
			path.AddArc(arcRect, 180, 90);
			path.AddLine(bounds.X + egdeRadius, bounds.Y, bounds.Right - egdeRadius, bounds.Y);
			// top right arc
			arcRect.X = bounds.Right - egdeRadius * 2;
			path.AddArc(arcRect, 270, 90);
			path.AddLine(bounds.Right, bounds.Left + egdeRadius, bounds.Right, bounds.Bottom - egdeRadius);
			// bottom right arc
			arcRect.Y = bounds.Bottom - egdeRadius * 2;
			path.AddArc(arcRect, 0, 90);
			path.AddLine(bounds.X + egdeRadius, bounds.Bottom, bounds.Right - egdeRadius, bounds.Bottom);
			// bottom left arc
			arcRect.X = bounds.Left;
			path.AddArc(arcRect, 90, 90);
			path.AddLine(bounds.X, bounds.Left + egdeRadius, bounds.X, bounds.Bottom - egdeRadius);
			
			g.FillPath(gradient, path);
			g.DrawPath(SystemPens.ControlText, path);
			path.Dispose();
			gradient.Dispose();
			Brush textBrush;
			string description = GetText(out textBrush);
			if (selected && textBrush == SystemBrushes.GrayText)
				textBrush = SystemBrushes.HighlightText;
			int titleWidth;
			using (Font boldFont = new Font("Arial", 8, FontStyle.Bold)) {
				g.DrawString(addIn.Name, boldFont, textBrush, innerMargin, innerMargin);
				titleWidth = (int)g.MeasureString(addIn.Name, boldFont).Width + 1;
			}
			if (addIn.Version != null && addIn.Version.ToString() != "0.0.0.0") {
				g.DrawString(addIn.Version.ToString(), Font, textBrush, innerMargin + titleWidth + 4, innerMargin);
			}
			RectangleF textBounds = bounds;
			textBounds.Offset(innerMargin, innerMargin);
			textBounds.Inflate(-innerMargin * 2, -innerMargin * 2);
			g.DrawString(description, Font, textBrush, textBounds);
		}
		
		string GetText(out Brush textBrush)
		{
			switch (addIn.Action) {
				case AddInAction.Enable:
					if (addIn.Enabled) {
						textBrush = SystemBrushes.ControlText;
						return addIn.Properties["description"];
					} else {
						textBrush = SystemBrushes.ActiveCaption;
						return "AddIn will be enabled after restarting SharpDevelop";
					}
				case AddInAction.Disable:
					textBrush = SystemBrushes.GrayText;
					if (addIn.Enabled)
						return "AddIn will be disabled after restarting SharpDevelop";
					else
						return "Disabled"; // TODO: Test if it was disabled because of conflict
				case AddInAction.Install:
					textBrush = SystemBrushes.ActiveCaption;
					return "AddIn will be installed after restarting SharpDevelop";
				case AddInAction.Uninstall:
					textBrush = SystemBrushes.GrayText;
					return "AddIn will be removed after restarting SharpDevelop";
				case AddInAction.Update:
					textBrush = SystemBrushes.ActiveCaption;
					return "AddIn will be updated after restarting SharpDevelop";
				default:
					textBrush = Brushes.Yellow;
					return addIn.Action.ToString();
			}
		}
	}
}
