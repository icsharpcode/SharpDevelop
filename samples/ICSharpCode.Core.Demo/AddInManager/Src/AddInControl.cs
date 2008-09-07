// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;

namespace ICSharpCode.AddInManager
{
	public class AddInControl : Control
	{
		AddIn addIn;
		bool isExternal;
		
		public AddIn AddIn {
			get {
				return addIn;
			}
		}
		
		public AddInControl(AddIn addIn)
		{
			this.addIn = addIn;
			this.BackColor = SystemColors.Window;
			this.ContextMenuStrip = MenuService.CreateContextMenu(this, "/AddIns/AddInManager/ContextMenu");
			
			isExternal = !FileUtility.IsBaseDirectory(FileUtility.ApplicationRootPath, addIn.FileName)
				&& !FileUtility.IsBaseDirectory(PropertyService.ConfigDirectory, addIn.FileName);
			
			this.ClientSize = new Size(100, isExternal ? 35 + pathHeight : 35);
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
		
		Color Mix(Color c1, Color c2, double perc)
		{
			double p1 = 1 - perc;
			double p2 = perc;
			return Color.FromArgb((int)(c1.R * p1 + c2.R * p2),
			                      (int)(c1.G * p1 + c2.G * p2),
			                      (int)(c1.B * p1 + c2.B * p2));
		}
		
		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			Rectangle bounds = this.ClientRectangle;
			bounds.Offset(1, 1);
			bounds.Inflate(-2, -2);
			Color startColor = SystemColors.ControlLightLight;
			Color endColor = SystemColors.Control;
			if (selected) {
				startColor = Mix(SystemColors.ControlLightLight, SystemColors.Highlight, 0.1);
				endColor   = Mix(SystemColors.ControlLightLight, SystemColors.Highlight, 0.65);
			}
			Brush gradient = new LinearGradientBrush(bounds,
			                                         startColor,
			                                         endColor,
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
			textBounds.Inflate(-innerMargin * 2, -innerMargin * 2 + 2);
			if (isExternal)
				textBounds.Height -= pathHeight;
			using (StringFormat sf = new StringFormat(StringFormatFlags.LineLimit)) {
				sf.Trimming = StringTrimming.EllipsisWord;
				g.DrawString(description, Font, textBrush, textBounds, sf);
			}
			if (isExternal) {
				textBounds.Y = textBounds.Bottom + 2;
				textBounds.Height = pathHeight + 2;
				using (Font font = new Font(Font.Name, 7, FontStyle.Italic)) {
					using (StringFormat sf = new StringFormat(StringFormatFlags.NoWrap)) {
						sf.Trimming = StringTrimming.EllipsisPath;
						sf.Alignment = StringAlignment.Far;
						g.DrawString(addIn.FileName, font,
						             selected ? SystemBrushes.HighlightText : SystemBrushes.ControlText,
						             textBounds, sf);
					}
				}
			}
		}
		
		const int pathHeight = 10;
		
		string GetText(out Brush textBrush)
		{
			switch (addIn.Action) {
				case AddInAction.Enable:
					if (addIn.Enabled) {
						textBrush = SystemBrushes.ControlText;
						return addIn.Properties["description"];
					} else {
						textBrush = SystemBrushes.ActiveCaption;
						return ResourceService.GetString("AddInManager.AddInEnabled");
					}
				case AddInAction.Disable:
					textBrush = SystemBrushes.GrayText;
					if (addIn.Enabled)
						return ResourceService.GetString("AddInManager.AddInWillBeDisabled");
					else
						return ResourceService.GetString("AddInManager.AddInDisabled");
				case AddInAction.Install:
					textBrush = SystemBrushes.ActiveCaption;
					return ResourceService.GetString("AddInManager.AddInInstalled");
				case AddInAction.Uninstall:
					textBrush = SystemBrushes.GrayText;
					return ResourceService.GetString("AddInManager.AddInRemoved");
				case AddInAction.Update:
					textBrush = SystemBrushes.ActiveCaption;
					return ResourceService.GetString("AddInManager.AddInUpdated");
				case AddInAction.InstalledTwice:
					textBrush = Brushes.Red;
					return ResourceService.GetString("AddInManager.AddInInstalledTwice");
				case AddInAction.DependencyError:
					textBrush = Brushes.Red;
					return ResourceService.GetString("AddInManager.AddInDependencyFailed");
				default:
					textBrush = Brushes.Yellow;
					return addIn.Action.ToString();
			}
		}
	}
}
