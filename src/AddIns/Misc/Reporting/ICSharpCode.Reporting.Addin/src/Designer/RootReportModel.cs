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
using ICSharpCode.Reporting.Addin.Globals;

namespace ICSharpCode.Reporting.Addin.Designer
{
	/// <summary>
	/// Description of RootReportModel.
	/// </summary>
	/// 
	
	
	class RootReportModel:RootDesignedComponent
	{
		
		
		[EditorBrowsableAttribute()]
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs pea)
		{
			base.OnPaint(pea);
			PrintMargin(pea.Graphics);
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
		
		
		void PrintMargin( Graphics graphics)
		{
			var header = String.Format(System.Globalization.CultureInfo.CurrentCulture,
				"[Size : {0}] [Landscape : {1}]  [Bounds : {2}]",
				Page.Size, Landscape, PageMargin);
			using (var font = DesignerGlobals.DesignerFont){
				SizeF size = graphics.MeasureString(header,font);
				graphics.DrawString(header,font,
					new SolidBrush(Color.LightGray),
					new Rectangle(
						3,
						3 + (int)font.GetHeight(),
						(int)size.Width,
						(int)size.Height));
				
				var rect = new Rectangle(PageMargin.Left - 2,PageMargin.Top - 2,
					Page.Width - PageMargin.Left - PageMargin.Right + 2,
					Size.Height - PageMargin.Top - PageMargin.Bottom + 2);
				graphics.DrawRectangle(new Pen(Color.LightGray,1),rect);
				//				e.FillRectangle(new SolidBrush(SystemColors.Window),rect);
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
