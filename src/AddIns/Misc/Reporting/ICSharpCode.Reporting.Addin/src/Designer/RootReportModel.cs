/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 24.02.2014
 * Time: 19:51
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Printing;
using ICSharpCode.Reporting.Items;
using ICSharpCode.Reporting.Addin.Globals;

namespace ICSharpCode.Reporting.Addin.Designer
{
	/// <summary>
	/// Description of RootReportModel.
	/// </summary>
	/// 
	
	
	public class RootReportModel:RootDesignedComponent
	{
	
		bool showDebugFrame;
		
		public RootReportModel(){
			Console.WriteLine("create RootReportModel");
		}
		
		
		[EditorBrowsableAttribute()]
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs pea)
		{
			base.OnPaint(pea);
			if (this.showDebugFrame) {
				PrintMargin(pea.Graphics);
			}
			using (Font font = DesignerGlobals.DesignerFont) {
				foreach(System.Windows.Forms.Control ctrl in this.Controls)
				{
					pea.Graphics.DrawString(ctrl.Name,
					                        font,
					                        Brushes.LightGray,
					                        ctrl.Location.X,ctrl.Location.Y - (int)font.GetHeight() - 3);
				}
			}
		}
		
		public void Toggle ()
		{
			showDebugFrame = !this.showDebugFrame;
			Invalidate(true);
		}
		
		private void PrintMargin( Graphics graphics)
		{
			string s = String.Format(System.Globalization.CultureInfo.CurrentCulture,
			                         "[Size : {0}] [Landscape : {1}]  [Bounds : {2}]",
			                         this.Page,this.Landscape,this.PageMargin);
			using (Font font = DesignerGlobals.DesignerFont){
				SizeF size = graphics.MeasureString(s,font);
				graphics.DrawString(s,font,
				             new SolidBrush(Color.LightGray),
				             new Rectangle(this.PageMargin.Left + 100,
				                           this.PageMargin.Top - (int)font.GetHeight() - 3,
				                           (int)size.Width,
				                           (int)size.Height));
				
				var rectangle = new Rectangle(this.PageMargin.Left - 2,this.PageMargin.Top - 2,
				                            this.Page.Width - this.PageMargin.Left - this.PageMargin.Right + 2,
				                            this.Size.Height - this.PageMargin.Top - this.PageMargin.Bottom + 2);
				graphics.DrawRectangle(new Pen(Color.LightGray,1),rectangle);
			}
		}
			
		
		public Margins PageMargin {get;set;}
			
		public Rectangle Page {get;set;}
			
		public bool Landscape {get;set;}
	}
	
		
	[Designer(typeof(ReportRootDesigner), typeof(IRootDesigner))]
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
