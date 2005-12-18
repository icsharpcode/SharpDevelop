// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class AutoHideContainer: Panel
	{
		protected Control control;
		
		protected bool autoHide = true;
		protected bool showOnMouseMove = true;
		protected bool showOnMouseDown = true;
		protected int activatorHeight = 1;
		
		protected bool mouseIn;
		
		public virtual bool AutoHide {
			get {
				return autoHide;
			}
			set {
				autoHide = value;
				RefreshSize();
			}
		}
		
		protected virtual void RefreshSize()
		{
			if (autoHide) {
				this.Height = activatorHeight;
				this.Controls.Clear();
			} else {
				this.Height = PreferredHeight;
				control.Dock = DockStyle.Fill;
				this.Controls.Add(control);
			}
		}
		
		protected virtual int PreferredHeight {
			get {
				return control.PreferredSize.Height;
			}
		}
		
		public bool ShowOnMouseMove {
			get {
				return showOnMouseMove;
			}
			set {
				showOnMouseMove = value;
			}
		}
		
		public bool ShowOnMouseDown {
			get {
				return showOnMouseDown;
			}
			set {
				showOnMouseDown = value;
			}
		}
		
		public Color ActivatorColor {
			get {
				return this.ForeColor;
			}
			set {
				this.ForeColor = value;
			}
		}
		
		public int ActivatorHeight {
			get {
				return activatorHeight;
			}
			set {
				activatorHeight = value;
			}
		}
		
		public AutoHideContainer(Control control)
		{
			if (control == null) {
				throw new ArgumentNullException("control");
			}
			this.control = control;
			RefreshSize();
			this.MouseMove += OnPanelMouseMove;
			this.MouseDown += OnPanelMouseDown;
			control.MouseEnter += OnControlMouseEnter;
			control.MouseLeave += OnControlMouseLeave;
		}
		
		protected virtual void OnPanelMouseMove(object sender, MouseEventArgs e)
		{
			if (showOnMouseMove && autoHide) {
				ShowOverlay();
			}
		}
		
		protected virtual void OnPanelMouseDown(object sender, MouseEventArgs e)
		{
			if (showOnMouseDown && autoHide) {
				ShowOverlay();
			}
		}
		
		protected virtual void OnControlMouseEnter(object sender, EventArgs e)
		{
			mouseIn = true;
		}
		
		protected virtual void OnControlMouseLeave(object sender, EventArgs e)
		{
			mouseIn = false;
			HideOverlay();
		}
		
		public virtual void ShowOverlay()
		{
			control.Dock = DockStyle.None;
			control.Size = new Size(this.Width, control.PreferredSize.Height);
			if (this.Dock != DockStyle.Bottom) {
				control.Location = new Point(this.Left, this.Top);
			} else {
				control.Location = new Point(this.Left, this.Top - control.PreferredSize.Height + 1);
			}
			Parent.Controls.Add(control);
			control.BringToFront();
		}
		
		public virtual void HideOverlay()
		{
			if (Parent.Controls.Contains(control)) {
				Parent.Controls.Remove(control);
			}
		}
	}
}
