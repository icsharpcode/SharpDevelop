/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 01.03.2006
 * Time: 14:29
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;

using SharpReportCore;

namespace SharpReport.Designer{
	
	/// <summary>
	/// Description of ReportTableControl.
	/// </summary>

	public class ReportRowControl:ReportControlBase{
		ControlHelper controlHelper;

		public ReportRowControl():base(){
			InitializeComponent();
			this.SetStyle(ControlStyles.DoubleBuffer |
			              ControlStyles.UserPaint |
			              ControlStyles.AllPaintingInWmPaint |
			              ControlStyles.ResizeRedraw,
			              true);
			this.UpdateStyles();
			
			this.Size = new Size((GlobalValues.PreferedSize.Width * 2) + 10,
			                     GlobalValues.PreferedSize.Height + 10);
			
			controlHelper = new ControlHelper((Control)this);
		}
		#region overrides
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e){
			base.OnPaint(e);
			TextDrawer tx = new TextDrawer();
			
			tx.DrawString(e.Graphics,this.Name,
			                        this.Font,
			                        new SolidBrush(this.ForeColor),
			                        new Rectangle(1,0,e.ClipRectangle.Width,(int)this.Font.GetHeight(e.Graphics) + 2),
			                        new StringFormat());
		}
		
		public override string ToString() {
			return this.Name;
		}
		
		#endregion
		
		#region Windows Forms Designer generated code
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent() {
			// 
			// ReportRectangleControl
			// 
			this.BackColor = System.Drawing.Color.White;
			this.Name = "RowItem";
			this.Size = new System.Drawing.Size(72, 40);
		}
		#endregion
	}
}
