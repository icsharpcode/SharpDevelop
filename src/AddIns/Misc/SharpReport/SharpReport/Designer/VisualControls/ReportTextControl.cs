/*
 * Created by SharpDevelop.
 * User: Fabio
 * Date: 09/10/2004
 * Time: 9.30
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;
using System.Windows.Forms;

using SharpReportCore;

namespace SharpReport.Designer{
	/// <summary>
	/// Description of ReportTextItem.
	/// </summary>
	internal class ReportTextControl : ReportControlBase{
		
		private TextDrawer textDrawer = new TextDrawer();
		
		public ReportTextControl():base(){
			InitializeComponent();
			
			this.SetStyle(ControlStyles.DoubleBuffer |
			              ControlStyles.UserPaint |
			              ControlStyles.AllPaintingInWmPaint |
			              ControlStyles.ResizeRedraw,
			              true);
			this.UpdateStyles();
			this.Size = GlobalValues.PreferedSize;
		}
		
		
		
		
		public override string Text{
			get { return base.Text; }
			set { 
				base.Text = value; 
			}
		}

		
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs pea){

			base.OnPaint(pea);
			
			Graphics graphics = pea.Graphics;
			StringFormat fmt = GlobalValues.StandartStringFormat();
			fmt.Alignment = this.StringAlignment;
			string str;
			
			if (String.IsNullOrEmpty(this.Text)) {
				str = this.Name;
			} else {
				str = this.Text;
			}
			textDrawer.DrawString(graphics,
			                      str,
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
			// 
			// ReportTextItem
			// 
			this.BackColor = System.Drawing.Color.White;
			this.Name = "ReportTextItem";
			this.Size = new System.Drawing.Size(120, 20);
		}
		#endregion
		
	}
}
