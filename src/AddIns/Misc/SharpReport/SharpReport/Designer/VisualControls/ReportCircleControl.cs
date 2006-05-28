

using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

using SharpReportCore;

/// <summary>
/// This Class is used for drawing Circles
/// </summary>
/// <remarks>
/// 	created by - Forstmeier Peter
/// 	created on - 07.01.2005 15:49:38
/// </remarks>
namespace SharpReport.Designer{
	
	internal class ReportCircleControl : AbstractGraphicControl{
		EllipseShape shape = new EllipseShape();
		
		public ReportCircleControl():base(){
			InitializeComponent();
			this.SetStyle(ControlStyles.DoubleBuffer |
			              ControlStyles.UserPaint |
			              ControlStyles.AllPaintingInWmPaint |
			              ControlStyles.ResizeRedraw,
			              true);
			this.UpdateStyles();
			
		}
		
		#region overrides
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs pea){
			base.OnPaint(pea);
			base.DrawEdges (pea);
			shape.FillShape(pea.Graphics,
			                new SolidFillPattern(this.BackColor),
			                (RectangleF)this.ClientRectangle);
			shape.DrawShape (pea.Graphics,
			                 new BaseLine (this.ForeColor,base.DashStyle,base.Thickness),
			                 (RectangleF)this.ClientRectangle);
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
			this.BackColor = System.Drawing.Color.White;
			this.Name = "ReportCircleControl";
			this.Size = new System.Drawing.Size(50, 50);
		}
		#endregion
	
		
	
	}
}
