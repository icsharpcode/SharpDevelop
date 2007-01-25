/*
 * Created by SharpDevelop.
 * User: itai
 * Date: 28/09/2006
 * Time: 19:03
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;

using System.Drawing;
using System.Drawing.Drawing2D;

using System.Xml;
using System.Xml.XPath;

using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

namespace ClassDiagram
{
	public class StructCanvasItem : ClassCanvasItem
	{
		public StructCanvasItem (IClass ct) : base (ct) {}
		/*
		protected override Color TitleBackground
		{
			get { return Color.Wheat;}
		}
	
		protected override Brush InnerTitlesBackground
		{
			get { return Brushes.PapayaWhip;}
		}
		*/
		
		protected override bool RoundedCorners
		{
			get { return false; }
		}
		
		protected override XmlElement CreateXmlElement(XmlDocument doc)
		{
			return doc.CreateElement("Struct");
		}		
	}
}
