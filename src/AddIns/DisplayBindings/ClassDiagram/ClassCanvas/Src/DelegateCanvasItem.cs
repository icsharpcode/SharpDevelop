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

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

using System.Globalization;

using Tools.Diagrams;
using Tools.Diagrams.Drawables;

namespace ClassDiagram
{
	// TODO - perhaps abandon this base class and implement styles mechanism instead?
	public class EnumDelegateCanvasItem : ClassCanvasItem
	{
		public EnumDelegateCanvasItem (IClass ct) : base (ct) {}

		private InteractiveItemsStack items = new InteractiveItemsStack();
		
		public InteractiveItemsStack Items {
			get { return items; }
		}

		protected override DrawableRectangle InitContentBackground()
		{
			if (RoundedCorners)
			{
				int radius = CornerRadius;
				return new DrawableRectangle(ContentBG, null, 0, 0, radius, 0);
			}
			else
				return new DrawableRectangle(ContentBG, null, 0, 0, 0, 0);
		}
		
		protected override DrawableItemsStack InitContentContainer(params IDrawableRectangle[] items)
		{
			DrawableItemsStack decorator = new DrawableItemsStack();
			decorator.OrientationAxis = Axis.X;
			DrawableRectangle rect;
			if (RoundedCorners)
			{
				int radius = CornerRadius;
				rect = new DrawableRectangle(TitleBG, null, 0, 0, 0, radius);
			}
			else
			{
				rect = new DrawableRectangle(TitleBG, null, 0, 0, 0, 0);
			}
			
			rect.Width = 20;
			
			decorator.Add(rect);
			decorator.Add(base.InitContentContainer(items));
			return decorator;
		}
		
		protected override IDrawableRectangle InitContent()
		{
			items.Border = 5;
			items.OrientationAxis = Axis.Y;
			return items;
		}
	}
	
	public class DelegateCanvasItem : EnumDelegateCanvasItem
	{
		public DelegateCanvasItem (IClass ct) : base (ct) {}
		
		static Color titlesBG = Color.FromArgb(255, 237, 219, 221);
		protected override Color TitleBackground
		{
			get { return titlesBG; }
		}
		
		static Brush contentBG = new SolidBrush(Color.FromArgb(255, 247, 240, 240));
		protected override Brush ContentBG
		{
			get { return contentBG; }
		}

		protected override void PrepareMembersContent()
		{
			Items.Clear();
			IMethod invokeMethod = RepresentedClassType.SearchMember("Invoke", RepresentedClassType.ProjectContent.Language) as IMethod;
			IAmbience ambience = GetAmbience();
			foreach (IParameter par in invokeMethod.Parameters)
			{
				TextSegment ts = new TextSegment(Graphics, par.Name  + " : " + ambience.Convert(par.ReturnType), MemberFont, true);
				Items.Add(ts);
			}
		}
		
		// TODO - remove - for debug only.
		public override void DrawToGraphics(Graphics graphics)
		{
			base.DrawToGraphics(graphics);
		}
		
		protected override XmlElement CreateXmlElement(XmlDocument doc)
		{
			return doc.CreateElement("Delegate");
		}
	}
	
	public delegate TestEnum TestDelegate (int num, string str);
}
