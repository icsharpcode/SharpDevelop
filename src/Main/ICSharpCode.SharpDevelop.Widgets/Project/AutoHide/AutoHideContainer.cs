// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop.Widgets.AutoHide
{
	public class AutoHideContainer: Panel
	{
		protected Control control;
		
		bool autoHide = true;
		bool showOverlay = false;
		bool showOnMouseMove = true;
		bool showOnMouseDown = true;
		int activatorHeight = 1;
		
		protected bool mouseIn;
		
		public virtual bool AutoHide {
			get {
				return autoHide;
			}
			set {
				autoHide = value;
				Reformat();
			}
		}
		
		public bool ShowOverlay {
			get {
				return showOverlay;
			}
			set {
				showOverlay = value;
				Reformat();
			}
		}
		
		protected virtual void Reformat()
		{
			if (autoHide) {
				if (showOverlay) {
					// Show as overlay
					this.Height = activatorHeight;
					control.Dock = DockStyle.None;
					control.Size = new Size(this.Width, control.PreferredSize.Height);
					if (this.Dock != DockStyle.Bottom) {
						control.Location = new Point(this.Left, this.Top);
					} else {
						control.Location = new Point(this.Left, this.Top - control.PreferredSize.Height + 1);
					}
					Parent.Controls.Add(control);
					control.BringToFront();
				} else {
					// Hidden
					this.Height = activatorHeight;
					control.Dock = DockStyle.None;
					control.Size = new Size(this.Width, 1);
					control.Location = new Point(0, activatorHeight);
					this.Controls.Add(control);
				}
			} else {
				// Permanently shown
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
		
		public AutoHideContainer(Control control)
		{
			if (control == null) throw new ArgumentNullException("control");
			this.control = control;
			this.MouseMove += delegate { if (showOnMouseMove) ShowOverlay = true; };
			this.MouseDown += delegate { if (showOnMouseDown) ShowOverlay = true; };
			control.MouseEnter += OnControlMouseEnter;
			control.MouseLeave += OnControlMouseLeave;
			Reformat();
		}
		
		protected virtual void OnControlMouseEnter(object sender, EventArgs e)
		{
			mouseIn = true;
		}
		
		protected virtual void OnControlMouseLeave(object sender, EventArgs e)
		{
			mouseIn = false;
			ShowOverlay = false;
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
	}
}
