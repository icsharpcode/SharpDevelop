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
	public class StructCanvasItem : ClassCanvasItem
	{
		public StructCanvasItem (IClass ct) : base (ct) {}
		
		protected override Color TitleBackground
		{
			get { return Color.Wheat;}
		}
	
		protected override Brush InnerTitlesBackground
		{
			get { return Brushes.PapayaWhip;}
		}
	}
}
