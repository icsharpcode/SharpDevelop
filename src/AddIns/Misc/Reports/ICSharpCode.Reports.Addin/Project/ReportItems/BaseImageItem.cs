/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 01.02.2008
 * Zeit: 08:24
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.IO;
using System.Windows.Forms.Design;
using System.Xml.Serialization;

using ICSharpCode.Reports.Core;

namespace ICSharpCode.Reports.Addin
{
	/// <summary>
	/// Description of BaseImageItem.
	/// </summary>
	[Designer(typeof(ICSharpCode.Reports.Addin.ImageDesigner))]
	public class BaseImageItem:AbstractItem
	{
		private string imageFileName;
		private Image image;
		private bool scaleImageToSize;
		private GlobalEnums.ImageSource imageSource;
		private string columnName;
		private string baseTableName;
		private string reportFileName;
		private string relativeFileName;
		
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
			if (this.scaleImageToSize) {
				graphics.DrawImageUnscaled(this.Image,this.Location.X,this.Location.Y);
			} else {
				Image im = this.image;
				if (im != null) {
					graphics.DrawImage(this.Image,this.ClientRectangle);
				} 
			}
		}
		
		
		#region Property's
		[Category("Image")]
		[EditorAttribute ( typeof(FileNameEditor), typeof(UITypeEditor) ) ]
		public string ImageFileName {
			get { return imageFileName; }
			set { imageFileName = value;
				this.relativeFileName = FileUtility.GetRelativePath(Path.GetFullPath(this.reportFileName),Path.GetFullPath(this.ImageFileName));
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
				this.image = FakeImage(base.Size,text);
				if (this.image != null) {
					return image;
				} else {
					if (!String.IsNullOrEmpty(imageFileName)) {
						this.image = this.LoadImage();
						return this.image;
					}
				}
				return null;
			}
			set {
				this.image = value;
				this.imageSource = GlobalEnums.ImageSource.External;
			}
		}
		
		
		[Category("Layout")]
		public bool ScaleImageToSize {
			get { return scaleImageToSize; }
			set { scaleImageToSize = value; }
		}
		
		
		[Category("Image")]
		public GlobalEnums.ImageSource ImageSource {
			get { return imageSource; }
			set { imageSource = value; }
		}
		
		
		
		
		[Category("Image from Database")]
		public string ColumnName {
			get { return columnName; }
			set {
				columnName = value;
			}
		}
		
		
		[Category("Image from Database")]
		public string BaseTableName {
			get { return baseTableName; }
			set {
				baseTableName = value;
			}
		}
		
		
		[XmlIgnoreAttribute]
		[Category("Image")]
		public string ReportFileName {
			get { return Path.GetFullPath(reportFileName); }
			set { reportFileName = value;}
		}
		
		
		[Category("Image")]
		[Browsable(false)]
		public string RelativeFileName {
			get { return relativeFileName; }
			set { relativeFileName = value;}
		}
		
		
		[XmlIgnoreAttribute]
		[Category("Image")]
//		[Browsable(false)]
		public string AbsoluteFileName
		{
			get {
//D:\SharpDevelop3.0_WorkingCopy\SharpDevelop\samples\SharpDevelopReports\SampleReports\Logos				
				
				if (!string.IsNullOrEmpty(relativeFileName)) {
					Console.WriteLine("");
					string testFileName = FileUtility.NormalizePath(Path.Combine(Path.GetDirectoryName(this.reportFileName),this.relativeFileName));
					if (File.Exists(testFileName)){
						Console.WriteLine("Image found with Relative Filename");
						Console.WriteLine("Report Filename {0}",this.reportFileName);
						Console.WriteLine("Relative Filename {0}",this.relativeFileName);
						Console.WriteLine("Image Filename {0}",this.ImageFileName);
						return testFileName;
					} else {
						Console.WriteLine("AbsoluteFileName can't load image");
						Console.WriteLine("Report Filename {0}",this.reportFileName);
						Console.WriteLine("Relative Filename {0}",this.relativeFileName);
						Console.WriteLine("Image Filename {0}",this.ImageFileName);
					}
				}
				return this.ImageFileName;
			}
		}
		
		#endregion
	}
	
	
	internal class ImageItemTypeProvider : TypeDescriptionProvider
	{
		public ImageItemTypeProvider() :  base(TypeDescriptor.GetProvider(typeof(AbstractItem)))
		{
		}
		
//		public ImageItemTypeProvider(TypeDescriptionProvider parent): base(parent)
//		{
//
//		}

		
		public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
		{
			ICustomTypeDescriptor td = base.GetTypeDescriptor(objectType,instance);
			return new ImageItemTypeDescriptor(td, instance);
		}
	}
	
	
	internal class ImageItemTypeDescriptor : CustomTypeDescriptor
	{
		
		public ImageItemTypeDescriptor(ICustomTypeDescriptor parent, object instance)
			: base(parent)
		{
//			instance = instance as BaseTextItem;
		}

		
		public override PropertyDescriptorCollection GetProperties()
		{
			return GetProperties(null);
		}

		
		public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			PropertyDescriptorCollection props = base.GetProperties(attributes);
			List<PropertyDescriptor> allProperties = new List<PropertyDescriptor>();
			

			DesignerHelper.AddDefaultProperties(allProperties,props);
			
			PropertyDescriptor prop = prop = props.Find("imageFileName",true);
			allProperties.Add(prop);
			
			prop = props.Find("Image",true);
			allProperties.Add(prop);
			
			prop = props.Find("ScaleImageToSize",true);
			allProperties.Add(prop);
			
			prop = props.Find("ImageSource",true);
			allProperties.Add(prop);
			
			prop = props.Find("ReportFileName",true);
			allProperties.Add(prop);
			
			prop = props.Find("RelativeFileName",true);
			allProperties.Add(prop);
			
			prop = props.Find("AbsoluteFileName",true);
			allProperties.Add(prop);
			
			prop = props.Find("ColumnName",true);
			allProperties.Add(prop);
			
			prop = props.Find("BaseTableName",true);
			allProperties.Add(prop);
			
			return new PropertyDescriptorCollection(allProperties.ToArray());
		}
	}
}
