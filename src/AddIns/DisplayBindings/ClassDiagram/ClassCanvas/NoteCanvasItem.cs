// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Itai Bar-Haim" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

using System.Drawing;
using System.Drawing.Drawing2D;

using System.Xml;
using System.Xml.XPath;

using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

using System.Globalization;

namespace ClassDiagram
{
	//TODO - complete note item
	public class NoteCanvasItem : CanvasItem
	{
		private string note = "<Text editing not implemented yet.>";
		
		public NoteCanvasItem()
		{
		}
		
		public string Note
		{
			get { return note; }
			set { note = value; }
		}
		
		public override void DrawToGraphics (Graphics graphics)
		{
			if (graphics == null) return;
			
			// Draw Shadow
			graphics.FillRectangle(CanvasItem.ShadowBrush, X + ActualWidth, Y + 3, 4, ActualHeight);
			graphics.FillRectangle(CanvasItem.ShadowBrush, X + 4, Y + ActualHeight, ActualWidth - 4, 3);
			
			// Draw Note Area
			graphics.FillRectangle(Brushes.LightYellow, X, Y, ActualWidth, ActualHeight);
			graphics.DrawRectangle(Pens.Olive, X, Y, ActualWidth, ActualHeight);
			
			// Draw Note Text
			RectangleF rect = new RectangleF (X + 5, Y + 5, ActualWidth - 10, ActualHeight - 10);
			graphics.DrawString(note, MemberFont, Brushes.Black, rect);
			
			base.DrawToGraphics(graphics);
		}
		
		protected override bool DragAreaHitTest(PointF pos)
		{
			return (pos.X > X && pos.Y > Y && pos.X < X + ActualWidth && pos.Y < Y + ActualHeight);
		}
		
		protected override XmlElement CreateXmlElement(XmlDocument doc)
		{
			return doc.CreateElement("Note");
		}
		
		protected override void FillXmlElement(XmlElement element, XmlDocument document)
		{
			base.FillXmlElement(element, document);
			element.SetAttribute("Height", Height.ToString(CultureInfo.InvariantCulture));
			element.SetAttribute("Note", Note);
		}
		
		public override void LoadFromXml (XPathNavigator navigator)
		{
			base.LoadFromXml(navigator);
			Height = float.Parse(navigator.GetAttribute("Height", ""), CultureInfo.InvariantCulture);
			Note = navigator.GetAttribute("Note", "");
		}
		
		public override float Width
		{
			set { base.Width = Math.Max (value, 40.0f); }
		}
		
		public override float Height
		{
			set { base.Height = Math.Max (value, 40.0f); }
		}
	}
}
