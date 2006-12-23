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

using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

using Tools.Diagrams;
using Tools.Diagrams.Drawables;

namespace ClassDiagram
{
	//TODO - an enum looks differently.
	public class EnumCanvasItem : ClassCanvasItem
	{
		public EnumCanvasItem (IClass ct) : base (ct) {}
	
		private DrawableItemsStack fields = new DrawableItemsStack();
		
		protected override Color TitleBackground
		{
			get { return Color.Plum; }
		}
		
		protected override IDrawableRectangle InitContent()
		{
			fields.Border = 5;
			fields.OrientationAxis = Axis.Y;
			return fields;
		}
		
		protected override void PrepareMembersContent ()
		{
			fields.Clear();
			PrepareMembersContent <IField> (RepresentedClassType.Fields, fields);
		}
	}
}
