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

	internal class ReportRowControl:ReportControlBase{
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
			base.DrawDecorations(pea);
			
			if (this.drawBorder) {
				shape.DrawShape (pea.Graphics,
				                 new BaseLine (this.ForeColor,System.Drawing.Drawing2D.DashStyle.Solid,1),
				                 base.FocusRectangle);
			}

			StringFormat fmt = GlobalValues.StandartStringFormat();
			fmt.LineAlignment = StringAlignment.Near;
			pea.Graphics.DrawString(this.Name,
			                        this.Font,
			                        new SolidBrush(this.ForeColor),
			                        new Rectangle(7,0,pea.ClipRectangle.Width,(int)this.Font.GetHeight(pea.Graphics) + 2),
			                        fmt);
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
			this.Name = "RowItem";
			this.Size = new System.Drawing.Size(72, 40);
		}
		#endregion
	}
}
