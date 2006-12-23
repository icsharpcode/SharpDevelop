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

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

using System.Globalization;

using Tools.Diagrams;
using Tools.Diagrams.Drawables;

namespace ClassDiagram
{
	// TODO - should look at the Invoke method and extract the parameters and return type.
	//        this is the only information that need to be shown. The parameters should
	//        be listed in the form of paramName : paramType, as for the return type, still
	//        need to figure that out ;)
	public class DelegateCanvasItem : ClassCanvasItem
	{
		public DelegateCanvasItem (IClass ct) : base (ct) {}
		
		private DrawableItemsStack parameters = new DrawableItemsStack();
		
		protected override Color TitleBackground
		{
			get { return Color.LightPink;}
		}
		
		protected override IDrawableRectangle InitContent()
		{
			parameters.Border = 5;
			parameters.OrientationAxis = Axis.Y;
			return parameters;
		}
				
		protected override void PrepareMembersContent()
		{
			parameters.Clear();
			IMethod invokeMethod = RepresentedClassType.SearchMember("Invoke", RepresentedClassType.ProjectContent.Language) as IMethod;
			IAmbience ambience = GetAmbience();
			foreach (IParameter par in invokeMethod.Parameters)
			{
				TextSegment ts = new TextSegment(Graphics, par.Name  + " : " + ambience.Convert(par.ReturnType), MemberFont, true);
				parameters.Add(ts);
			}
		}
	}
	
	public delegate TestEnum TestDelegate (int num, string str);
}
