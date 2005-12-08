// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="none" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using ICSharpCode.FormsDesigner.Services;

namespace ICSharpCode.FormsDesigner 
{
	/// <summary>
	/// This class represents a visual feedback for the current tab index of a
	/// form control.
	/// </summary>
	public class TabIndexControl : Control
	{
		static Color BACKCOLOR2 = Color.DarkViolet;     // background color when changed
		static Color BACKCOLOR  = Color.DarkBlue;       // normal background color  
		static Color TEXTCOLOR  = Color.White;          // text color
		static Font  TEXTFONT   = new Font("Tahoma",8); // font for the tab index number text
		static Size  DEFSIZE    = new Size(20, 20);     // default (also minimum) size of the control 
		
		protected Control associatedControl;  // Associated form control
		protected bool    hasChanged = false; // indicates if tab index has changed.
		
		/// <summary>
		/// Gets or sets that the tab index contro has changed the index 
		/// </summary>		
		public bool HasChanged  {
			get { 
				return hasChanged; 
			}
			set { 
				hasChanged = value; 
			}
		}
		
		public Control AssociatedControl {
			get {
				return associatedControl;
			}
		}
		
		/// <summary>
		/// Creates a tab index control.
		/// </summary>
		public TabIndexControl(Control c) 
		{
			associatedControl = c;
			this.Size = DEFSIZE;
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
			c.LocationChanged += new EventHandler(SetLocation);
			c.TabIndexChanged += new EventHandler(RepaintOnTabIndexChange);
			SetLocation(this, EventArgs.Empty);
		}
		
		void RepaintOnTabIndexChange(object sender, EventArgs e)
		{
			HasChanged = true;
			Refresh();
		}
		
		void SetLocation(object sender, EventArgs e)
		{
			this.Left = associatedControl.Left;
			this.Top  = associatedControl.Top;
		}
		
		/// <summary>
		/// Drawing code to draw the tab index number above the associated control. 
		/// </summary>
		protected override void OnPaint(PaintEventArgs pe) 
		{
			Graphics g = pe.Graphics;
			
			string tabIndexAsString = associatedControl.TabIndex.ToString();
			Size sz = Size.Round(g.MeasureString( tabIndexAsString,TEXTFONT));
			
			this.Width = (sz.Width < DEFSIZE.Width )? DEFSIZE.Width : sz.Width;
			this.Height = DEFSIZE.Height;
			Rectangle r = new Rectangle(0,0,this.Width,this.Height);
			
			Color bkColor = ( hasChanged )? BACKCOLOR2 : BACKCOLOR;
			g.FillRectangle(new SolidBrush(bkColor),r);
			
			StringFormat sf = new StringFormat();
			sf.Alignment = StringAlignment.Center;
			sf.LineAlignment = StringAlignment.Center;
			g.DrawString(tabIndexAsString, TEXTFONT, new SolidBrush(TEXTCOLOR), r, sf);
		}
	}
}
