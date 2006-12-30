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
