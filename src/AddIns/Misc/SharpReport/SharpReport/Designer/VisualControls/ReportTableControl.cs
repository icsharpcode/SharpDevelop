/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 09.07.2006
 * Time: 15:39
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
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
		private Padding padding = new Padding (5,12,5,5);
		private ReportRowControl headerRow;
		private ReportRowControl detailRow;
		private ReportRowControl footerRow;
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
			this.Padding = padding;
			
			this.headerRow = new ReportRowControl();
			this.headerRow.Location = new Point (this.Padding.Left,this.Padding.Top);
			this.headerRow.Size = new Size (this.Width - this.Padding.Left - this.Padding.Right,this.headerRow.Size.Height);
			this.headerRow.BackColor = Color.LightGray;
			this.headerRow.Dock = DockStyle.Top;
			this.Controls.Add(headerRow);
		
			System.Console.WriteLine("\tHeader Location {0}",this.headerRow.Location);
			this.detailRow = new ReportRowControl();
			this.detailRow.Location = new Point (this.Padding.Left,
			                                     this.Padding.Top + this.headerRow.Size.Height + this.padding.Top);
			this.detailRow.Size = new Size (this.Width - this.Padding.Left - this.Padding.Right,this.detailRow.Size.Height);
			this.detailRow.BackColor = Color.Azure;
//			this.Controls.Add(detailRow);
			
			
			System.Console.WriteLine("\tDetail Location {0}",this.detailRow.Location);
			
			
			this.footerRow = new ReportRowControl();
			
			this.footerRow.Location = new Point (this.Padding.Left,
			                                     this.Padding.Top + this.detailRow.Location.Y + this.detailRow.Size.Height + this.padding.Top);
			this.footerRow.Size = new Size (this.Width - this.Padding.Left - this.Padding.Right,this.footerRow.Size.Height);
			this.footerRow.BackColor = Color.LightBlue;
			this.footerRow.Parent = this;
//			this.footerRow.Anchor = AnchorStyles.Bottom;
			this.footerRow.Dock = DockStyle.Bottom;
			
			this.Controls.Add(footerRow);
			System.Console.WriteLine("\tfooter Location {0}",this.footerRow.Location);
			int h,w;
			h = this.headerRow.Size.Height + this.detailRow.Size.Height + this.footerRow.Size.Height;
				h = h + (3 * this.padding.Top) + this.padding.Bottom;
			w = (GlobalValues.PreferedSize.Width * 2) + 10;
			
			this.Size = new Size(w,h);
			
			this.Resize += new EventHandler(OnResize);
		}
		
		private void OnResize (object sender, EventArgs e) {
			System.Console.WriteLine("");
//			System.Console.WriteLine("TabaleControl:Resize");
//			System.Console.WriteLine("");
//
//			int h,w;
//			h = this.headerRow.Size.Height + this.detailRow.Size.Height + this.footerRow.Size.Height;
//			h = h + (3 * this.padding.Top) + this.padding.Bottom;
//			w = (GlobalValues.PreferedSize.Width * 2) + 10;
//			
//			this.Size = new Size(w,h);
		}
		
		#region overrides
		protected override void OnPaint(PaintEventArgs pea){
			System.Console.WriteLine("");
			System.Console.WriteLine("TableOnPaint");
			
			base.OnPaint(pea);
			this.headerRow.Invalidate();
			this.detailRow.Invalidate();
			this.footerRow.Invalidate();
			base.DrawEdges (pea,
			                new Rectangle(0,5,
			                              this.ClientSize.Width - 1,this.ClientSize.Height - 6) );
			
//			ControlHelper.DrawHeadLine(this,pea);
			pea.Graphics.DrawString("Table",new Font("Microsoft Sans Serif",
				                              6),
				             new SolidBrush(Color.Gray),
				             new RectangleF(this.padding.Left,1,pea.ClipRectangle.Width,12) );
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
