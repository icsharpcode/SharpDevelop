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

using Tools.Diagrams;
using Tools.Diagrams.Drawables;

namespace ClassDiagram
{
	public class EnumCanvasItem : EnumDelegateCanvasItem
	{
		public EnumCanvasItem (IClass ct) : base (ct) {}
	
		private InteractiveItemsStack fields = new InteractiveItemsStack();
		
		static Color titlesBG = Color.FromArgb(255, 221, 214, 239);
		protected override Color TitleBackground
		{
			get { return titlesBG; }
		}
		
		protected override bool RoundedCorners
		{
			get { return false; }
		}
		
		protected override void PrepareMembersContent ()
		{
			Items.Clear();
			PrepareMembersContent <IField> (RepresentedClassType.Fields, Items);
		}
		
		protected override XmlElement CreateXmlElement(XmlDocument doc)
		{
			return doc.CreateElement("Enum");
		}
	}
}
