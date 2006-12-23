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

//using System.Reflection;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

namespace ClassDiagram
{
	public class InterfaceCanvasItem : ClassCanvasItem
	{
		static Brush innerTitlesBG = new SolidBrush(Color.FromArgb(255, 224, 255, 224));
		static Color titlesBG = Color.FromArgb(255, 192, 224, 192);
	
		public InterfaceCanvasItem (IClass ct) : base (ct) {}
	
		protected override Color TitleBackground
		{
			get { return titlesBG;}
		}
	
		protected override Brush InnerTitlesBackground
		{
			get { return innerTitlesBG; }
		}
	}

}
