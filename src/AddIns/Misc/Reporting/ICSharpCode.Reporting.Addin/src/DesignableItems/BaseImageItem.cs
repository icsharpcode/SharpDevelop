/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 21.06.2015
 * Time: 10:35
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.ComponentModel;
using System.Drawing;
using System.Xml.Serialization;

using ICSharpCode.Reports.Addin.TypeProviders;
namespace ICSharpCode.Reporting.Addin.DesignableItems
{
	[Designer(typeof(ICSharpCode.Reporting.Addin.Designer.ImageDesigner))]
	public class BaseImageItem:AbstractItem
	{
//		string imageFileName;
		Image image;
//		GlobalEnums.ImageSource imageSource;
//		string reportFileName;
		
		public BaseImageItem(){
			TypeDescriptor.AddProvider(new ImageItemTypeProvider(), typeof(BaseImageItem));
			Size = new Size(300,40);
			BackColor = Color.LightGray;
		}
		
		
		[EditorBrowsableAttribute()]
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e){
			e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			base.OnPaint(e);
			Draw(e.Graphics);
		}
		
		
		public override void Draw(Graphics graphics){
		
			if (graphics == null) {
				throw new ArgumentNullException("graphics");
			}
			var rect = new Rectangle(ClientRectangle.Left,ClientRectangle.Top,
			                               ClientRectangle.Right -1,
			                               ClientRectangle.Bottom -1);

			DrawControl(graphics,rect);
			if (ScaleImageToSize) {
				graphics.DrawImageUnscaled(Image, Location.X, Location.Y);
			} else {
				graphics.DrawImage(Image, ClientRectangle);
			}
		}
		
		
		
		#region Property's
		
//		[Category("Image")]
//		[EditorAttribute ( typeof(FileNameEditor), typeof(UITypeEditor) ) ]
//		public string ImageFileName {
//			get { return imageFileName; }
//			set { imageFileName = value;
//				if (!String.IsNullOrEmpty(reportFileName)) {
////					this.RelativeFileName = FileUtility.GetRelativePath(Path.GetFullPath(this.reportFileName),Path.GetFullPath(this.ImageFileName));
//				}
//			}
//		}
		
		
		static Bitmap ErrorBitmap(Size size){
			var b = new Bitmap (size.Width,size.Height);
			using (Graphics g = Graphics.FromImage (b)){
				g.DrawRectangle (new Pen(Color.Black, 1),
				                 1,1,size.Width -2,size.Height -2);
				g.DrawString("Image",new Font("Microsoft Sans Serif", 8),
				             new SolidBrush(Color.Gray),
				             new RectangleF(1,1,size.Width,size.Height) );
			}
			return b;
		}
		
		
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
		
		
		/*
		Image LoadImage (){
		
			try {
//				Image im = null;

//		var im = FakeImage(new Size(200,20),"DummyDesignImage");
				
//				string absFileName = this.AbsoluteFileName;
//				if (!String.IsNullOrEmpty(absFileName) && File.Exists(absFileName)){
//					im = Image.FromFile (absFileName);
//				} else {
//					im = BaseImageItem.ErrorBitmap(base.Size);
//				}
				return (Image)im;
				
			} catch (System.OutOfMemoryException) {
				throw;
			} catch (System.IO.FileNotFoundException) {
				throw;
			}
		}
		*/
		
		
		[XmlIgnoreAttribute]
		public Image Image {
			get {
				string text = "<Dummy Design Image>";
//				if (this.imageSource == GlobalEnums.ImageSource.Database ) {
//					text = "<Database>";
//				}
				
//				if (!String.IsNullOrEmpty(imageFileName)) {
//						this.image = this.LoadImage();
//						return this.image;
//				} else
//				{
//					this.image = FakeImage(base.Size,text);
//				}
				this.image = FakeImage(Size, text);
				return this.image;
			}
			
			set {
				this.image = value;
//				this.imageSource = GlobalEnums.ImageSource.External;
			}
		}
		
		
		[Category("Layout")]
		public bool ScaleImageToSize {get;set;}
		
		
		
//		[Category("Image")]
//		public GlobalEnums.ImageSource ImageSource {
//			get { return imageSource; }
//			set { imageSource = value; }
//		}
		
			
//		[Category("DataBinding")]
//		public string ColumnName {get;set;}
//		
//		
//		[Category("DataBinding")]
//		public string BaseTableName {get;set;}
//		
//		
//		[Category("DataBinding")]
//		public string DataType {get;set;}
		
	/*	
		[XmlIgnoreAttribute]
		[Category("Image")]
		public string ReportFileName {
			get { return Path.GetFullPath(reportFileName); }
			set { reportFileName = value;}
		}
		
		
		[Category("Image")]
		[Browsable(false)]
		public string RelativeFileName {get;set;}
			
		
		
		[XmlIgnoreAttribute]
		[Category("Image")]
		[Browsable(false)]
		public string AbsoluteFileName
		{
			get {
				if (!string.IsNullOrEmpty(RelativeFileName)) {
					Console.WriteLine("");
					
					string testFileName = String.Empty;
					if (! String.IsNullOrEmpty(reportFileName)) {
//						testFileName = FileUtility.NormalizePath(Path.Combine(Path.GetDirectoryName(this.reportFileName),this.RelativeFileName));
					} 

					if (File.Exists(testFileName)){
						Console.WriteLine("Image found with Relative Filename");
						return testFileName;
					} else {
						Console.WriteLine("AbsoluteFileName can't load image");
						Console.WriteLine("Report Filename {0}",this.reportFileName);
						Console.WriteLine("Relative Filename {0}",this.RelativeFileName);
						Console.WriteLine("Image Filename {0}",this.ImageFileName);
					}
				}
				return this.ImageFileName;
			}
		}
		*/
//		public new string Name {get;set;}
		
		#endregion
	}	
}

