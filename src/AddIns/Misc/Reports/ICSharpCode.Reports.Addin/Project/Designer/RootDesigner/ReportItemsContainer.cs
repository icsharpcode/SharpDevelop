// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Printing;

namespace ICSharpCode.Reports.Addin.Designer
{
	/// <summary>
	/// Description of ReportItemsContainer.
	/// </summary>
	public class RootReportModel:RootDesignedComponent
	{
	
		bool showDebugFrame;
		
		[System.ComponentModel.EditorBrowsableAttribute()]
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs pea)
		{
			base.OnPaint(pea);
			if (this.showDebugFrame) {
				PrintMargin(pea.Graphics);
			}
			using (Font f = GlobalsDesigner.DesignerFont) {
				foreach(System.Windows.Forms.Control ctrl in this.Controls)
				{
					pea.Graphics.DrawString(ctrl.Name,
					                        f,
					                        Brushes.LightGray,
					                        ctrl.Location.X,ctrl.Location.Y - (int)f.GetHeight() - 3);
				}
			}
		}
		
		public void Toggle ()
		{
			this.showDebugFrame = !this.showDebugFrame;
			this.Invalidate(true);
		}
		
		private void PrintMargin( Graphics e)
		{
			string s = String.Format(System.Globalization.CultureInfo.CurrentCulture,
			                         "[Size : {0}] [Landscape : {1}]  [Bounds : {2}]",
			                         this.Page,this.Landscape,this.PageMargin);
			using (Font f = GlobalsDesigner.DesignerFont){
				SizeF size = e.MeasureString(s,f);
				e.DrawString(s,f,
				             new SolidBrush(Color.LightGray),
				             new Rectangle(this.PageMargin.Left + 100,
				                           this.PageMargin.Top - (int)f.GetHeight() - 3,
				                           (int)size.Width,
				                           (int)size.Height));
				
				Rectangle rect = new Rectangle(this.PageMargin.Left - 2,this.PageMargin.Top - 2,
				                            this.Page.Width - this.PageMargin.Left - this.PageMargin.Right + 2,
				                            this.Size.Height - this.PageMargin.Top - this.PageMargin.Bottom + 2);
				e.DrawRectangle(new Pen(Color.LightGray,1),rect);
			}
		}
			
		
		public Margins PageMargin {get;set;}
			
		
		public Rectangle Page {get;set;}
			
		public bool Landscape {get;set;}
	}
	
		
	[Designer(typeof(ICSharpCode.Reports.Addin.Designer.ReportRootDesigner), typeof(IRootDesigner))]
	public class RootDesignedComponent : System.Windows.Forms.UserControl
	{
		public RootDesignedComponent()
		{
			InitializeComponent();
		}
		
		
		private void InitializeComponent()
		{
			this.SuspendLayout();
			// 
			// UserControl1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Window;
			this.Name = "UserControl1";
			this.Size = new System.Drawing.Size(800, 800);
			this.ResumeLayout(false);
		}
	}
	
}
