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
	public class InterfaceCanvasItem : ClassCanvasItem
	{
		public InterfaceCanvasItem (IClass ct) : base (ct) {}
		
		static Color titlesBG = Color.FromArgb(255, 233, 242, 224);
		protected override Color TitleBackground
		{
			get { return titlesBG;}
		}
	
		static Brush innerTitlesBG = new SolidBrush(Color.FromArgb(255, 243, 247, 240));
		protected override Brush InnerTitlesBackground
		{
			get { return innerTitlesBG; }
		}
				
		protected override XmlElement CreateXmlElement(XmlDocument doc)
		{
			return doc.CreateElement("Interface");
		}
	}
}
