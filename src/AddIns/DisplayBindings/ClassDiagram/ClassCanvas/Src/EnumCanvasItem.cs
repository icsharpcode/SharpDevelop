// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
