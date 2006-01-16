/*
 * Created by SharpDevelop.
 * User: Fabio
 * Date: 09/10/2004
 * Time: 9.31
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using SharpReportCore;
namespace SharpReport.Designer
{
	/// <summary>
	/// Description of ReportDbTextItem.
	/// </summary>
	internal class ReportDbTextControl : ReportControlBase
	{
		
		private TextDrawer textDrawer = new TextDrawer();
		
		public ReportDbTextControl()
		{
			
			InitializeComponent();
			this.SetStyle(ControlStyles.DoubleBuffer |
			              ControlStyles.UserPaint |
			              ControlStyles.AllPaintingInWmPaint |
			              ControlStyles.ResizeRedraw,
			              true);
			this.UpdateStyles();
			
		}
		
		
	
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
		{
			base.OnPaint(e);
			Graphics g = e.Graphics;
			
			// we can draw in the control as we draw in preview
			
			StringFormat fmt = base.StringFormat;
			fmt.Alignment = base.StringAlignment;
			textDrawer.DrawString(g,
			                      this.Text,
			                      this.Font,
			                      new SolidBrush(this.ForeColor),
			                      (RectangleF)this.ClientRectangle,
			                      fmt);
		}
		
		
		
		#region Windows Forms Designer generated code
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent() {
			this.BackColor = System.Drawing.Color.White;
			this.Name = "ReportDbTextItem";
			this.Size = new System.Drawing.Size(120, 20);
		}
		#endregion
	}
}
