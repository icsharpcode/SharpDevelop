/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 09.07.2006
 * Time: 15:39
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using SharpReportCore;

namespace SharpReport.Designer
{
	/// <summary>
	/// Description of ReportTableControl.
	/// </summary>
	public  class ReportTableControl:ContainerControl
	{
		private RectangleShape shape = new RectangleShape();
		private bool drawBorder;
		public ReportTableControl()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			this.SetStyle(ControlStyles.DoubleBuffer |
			              ControlStyles.UserPaint |
			              ControlStyles.AllPaintingInWmPaint |
			              ControlStyles.ResizeRedraw,
			              true);
			this.UpdateStyles();
			int h,w;
			h = GlobalValues.PreferedSize.Height * 3 + 30;
			w = (GlobalValues.PreferedSize.Width * 2) + 10;

//			this.Size = new Size(w,h);
		}
		
		#region overrides
		protected override void OnPaint(PaintEventArgs pea){
			
			
			base.OnPaint(pea);
			base.DrawEdges (pea,
			                new Rectangle(0,5,
			                              this.ClientSize.Width - 1,this.ClientSize.Height - 6) );
			
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
		#region FormsDesigner
		
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the control.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			
			this.SuspendLayout();
			
			// 
			// ReportTableControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		
			this.Name = "Table";
			this.Size = new System.Drawing.Size(200,40);
			this.ResumeLayout(false);
		}
		
		#endregion
	}
}
