// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Xml;
using System.Collections;
using System.Drawing;
using System.Drawing.Text;
using System.Reflection;
using System.Windows.Forms;
using ICSharpCode.Core;

using System.ComponentModel;
using System.Data;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;

namespace ICSharpCode.SharpDevelop.Gui.XmlForms
{
	public class MyGroupBox : GroupBox
	{
		public override Color BackColor {
			get {
				return base.BackColor;
			}
			set {
				//overrided to allow set Transparent to BackColor when FlatStyle ==System
				if (value == Color.Transparent && FlatStyle == FlatStyle.System) {
					this.SetStyle(ControlStyles.UserPaint | ControlStyles.SupportsTransparentBackColor, true);
				}
				base.BackColor = value;
			}
		}
		
		public MyGroupBox()
		{
			BackColor = Color.Transparent;
		}
		
		protected override void WndProc(ref Message m)
		{
			//Handle the PaintBackGround by ourself.
			//tricky using the SetStyle, but I'm afraid it maybe broken in the next version.
			if (m.Msg == 20 && FlatStyle == FlatStyle.System) {
				this.SetStyle(ControlStyles.UserPaint | ControlStyles.SupportsTransparentBackColor, true);
				Graphics g = Graphics.FromHdcInternal(m.WParam);
				this.OnPaintBackground(new PaintEventArgs(g,this.ClientRectangle));
				g.Dispose();
				this.SetStyle(ControlStyles.UserPaint | ControlStyles.SupportsTransparentBackColor, false);
				return;
			}
			base.WndProc(ref m);
		}
	}
	
	public class MyCheckBox : CheckBox
	{
		public override Color BackColor {
			get {
				return base.BackColor;
			}
			set {
				//overrided to allow set Transparent to BackColor when FlatStyle ==System
				if (value == Color.Transparent && FlatStyle == FlatStyle.System) {
					this.SetStyle(ControlStyles.UserPaint | ControlStyles.SupportsTransparentBackColor, true);
				}
				base.BackColor = value;
			}
		}
		
		public MyCheckBox()
		{
			BackColor = Color.Transparent;
		}
		
		protected override void WndProc(ref Message m)
		{
			//Handle the PaintBackGround by ourself.
			//tricky using the SetStyle, but I'm afraid it maybe broken in the next version.
			if (m.Msg == 20 && FlatStyle == FlatStyle.System) {
				this.SetStyle(ControlStyles.UserPaint | ControlStyles.SupportsTransparentBackColor, true);
				Graphics g = Graphics.FromHdcInternal(m.WParam);
				this.OnPaintBackground(new PaintEventArgs(g,this.ClientRectangle));
				g.Dispose();
				this.SetStyle(ControlStyles.UserPaint | ControlStyles.SupportsTransparentBackColor, false);
				return;
			}
			base.WndProc(ref m);
		}
	}
	
	public class SharpDevelopObjectCreator : DefaultObjectCreator
	{
		public override object CreateObject(string name, XmlElement el)
		{
			object o = null;
			
			if (name == "System.Windows.Forms.GroupBox") {
				return new MyGroupBox();
			} else if (name == "System.Windows.Forms.CheckBox") {
				return new MyCheckBox();
			} else {
				o = base.CreateObject(name, el);
			}
			if (o != null) {
				try {
					PropertyInfo propertyInfo = o.GetType().GetProperty("FlatStyle");
					if (propertyInfo != null) {
						if (o is Label) {
							propertyInfo.SetValue(o, FlatStyle.Standard, null);
						} else {
							propertyInfo.SetValue(o, FlatStyle.System, null);
						}
					}
				} catch (Exception) {}
			}
			return o;
		}
	}
}
