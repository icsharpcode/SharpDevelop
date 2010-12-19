// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using ICSharpCode.AvalonEdit.Rendering;

namespace EmbeddedImageAddIn
{
	/// <summary>
	/// VisualLineElement implementation for embedded images.
	/// </summary>
	public class ImageElement : VisualLineElement
	{
		readonly string imageFileName;
		readonly ImageSource image;
		
		public ImageElement(string imageFileName, ImageSource image, int documentLength) : base(1, documentLength)
		{
			if (imageFileName == null)
				throw new ArgumentNullException("imageFileName");
			if (image == null)
				throw new ArgumentNullException("image");
			this.imageFileName = imageFileName;
			this.image = image;
		}
		
		public override TextRun CreateTextRun(int startVisualColumn, ITextRunConstructionContext context)
		{
			return new ImageTextRun(image, this.TextRunProperties);
		}
		
		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			base.OnMouseDown(e);
			if (!e.Handled && e.ClickCount == 2) {
				// double click on image: open the image in editor
				try {
					Process.Start(imageFileName);
				} catch (Exception ex) {
					MessageBox.Show(ex.Message);
				}
			}
		}
		
		sealed class ImageTextRun : TextEmbeddedObject
		{
			readonly ImageSource image;
			readonly TextRunProperties properties;
			
			public ImageTextRun(ImageSource image, TextRunProperties properties)
			{
				this.image = image;
				this.properties = properties;
			}
			
			public override LineBreakCondition BreakBefore {
				get { return LineBreakCondition.BreakPossible; }
			}
			
			public override LineBreakCondition BreakAfter {
				get { return LineBreakCondition.BreakPossible; }
			}
			
			public override bool HasFixedSize {
				get { return true; }
			}
			
			public override CharacterBufferReference CharacterBufferReference {
				get { return new CharacterBufferReference(); }
			}
			
			public override int Length {
				get { return 1; }
			}
			
			public override TextRunProperties Properties {
				get { return properties; }
			}
			
			public override TextEmbeddedObjectMetrics Format(double remainingParagraphWidth)
			{
				return new TextEmbeddedObjectMetrics(image.Width, image.Height, image.Height);
			}
			
			public override Rect ComputeBoundingBox(bool rightToLeft, bool sideways)
			{
				return new Rect(0, 0, image.Width, image.Height);
			}
			
			public override void Draw(DrawingContext drawingContext, Point origin, bool rightToLeft, bool sideways)
			{
				drawingContext.DrawImage(image, new Rect(origin.X, origin.Y - image.Height, image.Width, image.Height));
			}
		}
	}
}
