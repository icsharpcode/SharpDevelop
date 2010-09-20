// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
