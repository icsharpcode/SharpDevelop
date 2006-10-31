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
using System.Windows.Forms;

using SharpReportCore;

namespace SharpReport.Designer{
	
	/// <summary>
	/// Description of ReportTableControl.
	/// </summary>

	internal class ReportRowControl:ContainerControl{
		
		private RectangleShape shape = new RectangleShape();
		private bool drawBorder;
		
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
		}
		
		
		#region overrides
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs pea){
			base.OnPaint(pea);
			base.DrawEdges (pea,
			                new Rectangle(0,5,this.ClientSize.Width - 1,this.ClientSize.Height - 6) );
			
			if (this.drawBorder) {
				shape.DrawShape (pea.Graphics,
				                 new BaseLine (this.ForeColor,System.Drawing.Drawing2D.DashStyle.Solid,1),
				                 base.FocusRectangle);
			}
			ControlHelper.DrawHeadLine(this,pea);

		}
		
		public override string ToString() {
			return this.GetType().Name;
		}
		
		#endregion
		
		public bool DrawBorder {
			set {
				drawBorder = value;
				this.Invalidate();
			}
		}
		
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
			this.Name = "Row";
			this.Size = new System.Drawing.Size(72, 40);
		}
		#endregion
	}
}
