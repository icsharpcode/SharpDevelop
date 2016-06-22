/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 21.06.2015
 * Time: 11:54
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using ICSharpCode.Reporting.Interfaces.Export;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;

namespace ICSharpCode.Reporting.Items
{
	/// <summary>
	/// Description of BaseImageItem.
	/// </summary>
	public class BaseImageItem:PrintableItem
	{
		Image image;
		
		public BaseImageItem()
		{
		}
		
		#region IExportColumnBuilder  implementation
		
		public override IExportColumn CreateExportColumn(){
			var export = new ExportImage();
			export.ToExportItem(this);
			
			export.Image = Image;
			
			export.ScaleImageToSize = ScaleImageToSize;
			return export;
		}

		#endregion
		
		static Bitmap FakeImage(Size size, string text){
		
			var b = new Bitmap (size.Width,size.Height);
			using (Graphics g = Graphics.FromImage (b)){
				g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
				g.DrawRectangle (new Pen(Color.Black, 1),
				                 1,1,size.Width -2,size.Height -2);
				
				g.DrawString(text,new Font("Microsoft Sans Serif",
				                           16,
				                           FontStyle.Regular,
				                           GraphicsUnit.Point),
				             new SolidBrush(Color.Gray),
				             new RectangleF(2,2,size.Width,size.Height) );
			}
			return b;
		}
		
		
		public Image Image {
			get {
				
				if (image == null) {
					string text = "<Dummy Design Image>";
					this.image = FakeImage(Size, text);
				}
				
				return this.image;
			}
			
			set {
				this.image = value;
			}
		}
		
		public bool ScaleImageToSize {get;set;}
	}
}
