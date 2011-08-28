// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Windows.Forms.Design;
using System.Xml.Serialization;

using ICSharpCode.Reports.Addin.TypeProviders;
using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Globals;

namespace ICSharpCode.Reports.Addin
{
	/// <summary>
	/// Description of BaseImageItem.
	/// </summary>
	[Designer(typeof(ICSharpCode.Reports.Addin.Designer.ImageDesigner))]
	public class BaseImageItem:AbstractItem
	{
		private string imageFileName;
		private Image image;
		private GlobalEnums.ImageSource imageSource;
		private string reportFileName;
		
		public BaseImageItem()
		{
			TypeDescriptor.AddProvider(new ImageItemTypeProvider(), typeof(BaseImageItem));
		}
		
		
		[System.ComponentModel.EditorBrowsableAttribute()]
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
		{
			
			e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			base.OnPaint(e);
			Draw(e.Graphics);
		}
		
		
		public override void Draw(Graphics graphics)
		{
			if (graphics == null) {
				throw new ArgumentNullException("graphics");
			}
			Rectangle rect = new Rectangle(this.ClientRectangle.Left,this.ClientRectangle.Top,
			                               this.ClientRectangle.Right -1,
			                               this.ClientRectangle.Bottom -1);

			base.DrawControl(graphics,rect);
			if (this.ScaleImageToSize) {
				graphics.DrawImageUnscaled(this.Image,this.Location.X,this.Location.Y);
			} else {
				graphics.DrawImage(this.Image,this.ClientRectangle);
			}
		}
		
		
		
		#region Property's
		[Category("Image")]
		[EditorAttribute ( typeof(FileNameEditor), typeof(UITypeEditor) ) ]
		public string ImageFileName {
			get { return imageFileName; }
			set { imageFileName = value;
				if (!String.IsNullOrEmpty(reportFileName)) {
					this.RelativeFileName = FileUtility.GetRelativePath(Path.GetFullPath(this.reportFileName),Path.GetFullPath(this.ImageFileName));
				}
			}
		}
		
		
		private static Bitmap ErrorBitmap(Size size)
		{
			Bitmap b = new Bitmap (size.Width,size.Height);
			using (Graphics g = Graphics.FromImage (b)){
				g.DrawRectangle (new Pen(Color.Black, 1),
				                 1,1,size.Width -2,size.Height -2);
				g.DrawString("Image",new Font("Microsoft Sans Serif", 8),
				             new SolidBrush(Color.Gray),
				             new RectangleF(1,1,size.Width,size.Height) );
			}
			return b;
		}
		
		
		private static Bitmap FakeImage(Size size, string text)
		{
			Bitmap b = new Bitmap (size.Width,size.Height);
			using (Graphics g = Graphics.FromImage (b)){
				g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
				g.DrawRectangle (new Pen(Color.Black, 1),
				                 1,1,size.Width -2,size.Height -2);
				
				g.DrawString(text,GlobalValues.DefaultFont,
				             new SolidBrush(Color.Gray),
				             new RectangleF(2,2,size.Width,size.Height) );
			}
			return b;
		}
		
		
		private Image LoadImage ()
		{
			try {
				Image im = null;

				string absFileName = this.AbsoluteFileName;
				if (!String.IsNullOrEmpty(absFileName) && File.Exists(absFileName)){
					im = Image.FromFile (absFileName);
				} else {
					im = BaseImageItem.ErrorBitmap(base.Size);
				}
				return im;
				
			} catch (System.OutOfMemoryException) {
				throw;
			} catch (System.IO.FileNotFoundException) {
				throw;
			}
		}
		
		
		
		[XmlIgnoreAttribute]
		public Image Image {
			get {
				string text = "<Image>";
				if (this.imageSource == GlobalEnums.ImageSource.Database ) {
					text = "<Database>";
				}
				
				if (!String.IsNullOrEmpty(imageFileName)) {
						this.image = this.LoadImage();
						return this.image;
				} else
				{
					this.image = FakeImage(base.Size,text);
				}
				return this.image;
			}
			
			set {
				this.image = value;
				this.imageSource = GlobalEnums.ImageSource.External;
			}
		}
		
		
		[Category("Layout")]
		public bool ScaleImageToSize {get;set;}
		
		
		
		[Category("Image")]
		public GlobalEnums.ImageSource ImageSource {
			get { return imageSource; }
			set { imageSource = value; }
		}
		
		
		
		
		[Category("DataBinding")]
		public string ColumnName {get;set;}
		
		
		[Category("DataBinding")]
		public string BaseTableName {get;set;}
		
		
		[Category("DataBinding")]
		public string DataType {get;set;}
		
		
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
						testFileName = FileUtility.NormalizePath(Path.Combine(Path.GetDirectoryName(this.reportFileName),this.RelativeFileName));
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
		
		public new string Name {get;set;}
		
		#endregion
	}
	
	
	
}
