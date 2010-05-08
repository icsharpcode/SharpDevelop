/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 11.11.2007
 * Zeit: 22:08
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Printing;

namespace ICSharpCode.Reports.Addin
{
	/// <summary>
	/// Description of ReportItemsContainer.
	/// </summary>
	public class RootReportModel:RootDesignedComponent
	{
		Margins pageMargin;
		Rectangle page;
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
			string s = String.Format(System.Globalization.CultureInfo.CurrentCulture,"[Bounds : {0}]",this.pageMargin);
			using (Font f = GlobalsDesigner.DesignerFont){
				SizeF size = e.MeasureString(s,f);
				e.DrawString(s,f,
				             new SolidBrush(Color.LightGray),
				             new Rectangle(this.pageMargin.Left + 100,
				                           this.pageMargin.Top - (int)f.GetHeight() - 3,
				                           (int)size.Width,
				                           (int)size.Height));
				
				Rectangle rect = new Rectangle(this.pageMargin.Left - 2,this.pageMargin.Top - 2,
				                            this.page.Width - this.pageMargin.Left - this.pageMargin.Right + 2,
				                            this.Size.Height - this.pageMargin.Top - this.pageMargin.Bottom + 2);
				e.DrawRectangle(new Pen(Color.LightGray,1),rect);
			}
		}
			
		
		public Margins PageMargin {
			get { return pageMargin; }
			set { pageMargin = value; }
		}
		
		public Rectangle Page {
			get { return page; }
			set { page = value; }
		}
		
	}
	
		
	[Designer(typeof(ICSharpCode.Reports.Addin.ReportRootDesigner), typeof(IRootDesigner))]
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
