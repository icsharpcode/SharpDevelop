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
	public class ClassCanvasItem : CanvasItem, IDisposable
	{
		IClass classtype;
		string typeclass;
		InteractiveHeaderedItem classItemHeaderedContent;
		DrawableItemsStack classItemContainer = new DrawableItemsStack();

		#region Graphics related variables
		
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		public static readonly Font TitleFont = new Font (FontFamily.GenericSansSerif, 11, FontStyle.Bold, GraphicsUnit.Pixel);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		public static readonly Font SubtextFont = new Font (FontFamily.GenericSansSerif, 11, FontStyle.Regular, GraphicsUnit.Pixel);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		public static readonly Font GroupTitleFont = new Font (FontFamily.GenericSansSerif, 11, FontStyle.Regular, GraphicsUnit.Pixel);

		LinearGradientBrush grad;
		GraphicsPath shadowpath;
		DrawableRectangle containingShape;
		
		#endregion

		CollapseExpandShape collapseExpandShape = new CollapseExpandShape();

		DrawableItemsStack titles = new DrawableItemsStack();

		DrawableItemsStack titlesCollapsed = new DrawableItemsStack();
		DrawableItemsStack titlesExpanded = new DrawableItemsStack();
		
		DrawableItemsStack<InteractiveHeaderedItem> groups = new DrawableItemsStack<InteractiveHeaderedItem>();
		Dictionary<InteractiveHeaderedItem, string> groupNames = new Dictionary<InteractiveHeaderedItem, string>(); // TODO - this is really an ugly patch
		Dictionary<string, InteractiveHeaderedItem> groupsByName = new Dictionary<string, InteractiveHeaderedItem>(); // TODO - this is really an ugly patch
		
		DrawableItemsStack<TextSegment> interfaces = new DrawableItemsStack<TextSegment>();
		
		DrawableRectangle titlesBackgroundCollapsed;
		DrawableRectangle titlesBackgroundExpanded;
		
		protected override bool AllowHeightModifications()
		{
			return false;
		}
		
		public override float Width
		{
			set
			{
				base.Width = Math.Max (value, 100.0f);
				PrepareFrame();
			}
		}
		
		public override float GetAbsoluteContentWidth()
		{
			return classItemHeaderedContent.GetAbsoluteContentWidth();
		}
		
		public override float GetAbsoluteContentHeight()
		{
			return classItemHeaderedContent.GetAbsoluteContentHeight();
		}
		
		public IClass RepresentedClassType
		{
			get { return classtype; }
		}

		#region Constructors
		
		public ClassCanvasItem (IClass ct)
		{
			classtype = ct;
	
			grad = new LinearGradientBrush(
				new PointF(0, 0), new PointF(1, 0),
				TitleBackground, Color.White);
					
			classItemHeaderedContent = new InteractiveHeaderedItem(titlesCollapsed, titlesExpanded, InitContentContainer(InitContent()));

			classItemContainer.Container = this;
			classItemContainer.Add(classItemHeaderedContent);
			Pen outlinePen = GetClassOutlinePen();
			if (RoundedCorners)
			{
				int radius = CornerRadius;
				containingShape = new DrawableRectangle(null, outlinePen, radius, radius, radius, radius);
			}
			else
				containingShape = new DrawableRectangle(null, outlinePen, 0, 0, 0, 0);
			
			classItemContainer.Add(containingShape);
			classItemContainer.OrientationAxis = Axis.Z;
			
			if (RoundedCorners)
			{
				int radius = CornerRadius;
				titlesBackgroundCollapsed = new DrawableRectangle(grad, null, radius, radius, radius, radius);
				titlesBackgroundExpanded = new DrawableRectangle(grad, null, radius, radius, 0, 0);
			}
			else
			{
				titlesBackgroundCollapsed = new DrawableRectangle(grad, null, 0, 0, 0, 0);
				titlesBackgroundExpanded = new DrawableRectangle(grad, null, 0, 0, 0, 0);
			}

			titles.Border = 5;
			
			titlesCollapsed.Add(titlesBackgroundCollapsed);
			titlesCollapsed.Add(titles);
			titlesCollapsed.OrientationAxis = Axis.Z;
			
			titlesExpanded.Add(titlesBackgroundExpanded);
			titlesExpanded.Add(titles);
			titlesExpanded.OrientationAxis = Axis.Z;
			
			if (classtype != null)
			{
				typeclass = classtype.Modifiers.ToString();
				typeclass += " " + classtype.ClassType.ToString();
			}
		}
		
		#endregion
		
		private Pen GetClassOutlinePen()
		{
			Pen pen = new Pen(Color.Gray);
			
			if (classtype.IsAbstract)
			{
				pen.DashStyle = DashStyle.Dash;
			}
			
			if (classtype.IsSealed)
			{
				pen.Width = 3;
			}
			
			if (classtype.IsStatic)
			{
				pen.DashStyle = DashStyle.Dash;
				pen.Width = 3;
			}
			
			return pen;
		}
		
		public override bool IsVResizable
		{
			get { return false; }
		}

		protected virtual DrawableRectangle InitContentBackground()
		{
			if (RoundedCorners)
			{
				int radius = CornerRadius;
				return new DrawableRectangle(ContentBG, null, 0, 0, radius, radius);
			}
			else
				return new DrawableRectangle(ContentBG, null, 0, 0, 0, 0);
		}
		
		protected virtual DrawableItemsStack InitContentContainer(params IDrawableRectangle[] items)
		{
			DrawableItemsStack content = new DrawableItemsStack();
			content.OrientationAxis = Axis.Z;
			
			content.Add(InitContentBackground());
			
			foreach (IDrawableRectangle item in items)
				content.Add(item);
			
			return content;
		}
		
		protected virtual IDrawableRectangle InitContent ()
		{
			groups.MinWidth = 80;
			groups.Spacing = 5;
			groups.Border = 5;
			
			return groups;
		}
		
		public void Initialize ()
		{
			PrepareMembersContent();
			PrepareTitles();
			Width = GetAbsoluteContentWidth();
		}
		
		#region Graphics related members
		
		static Color titlesBG = Color.FromArgb(255,217, 225, 241);
		protected virtual Color TitleBackground
		{
			get { return titlesBG; }
		}

		protected virtual LinearGradientBrush TitleBG
		{
			get { return grad; }
		}
		
		static Brush innerTitlesBG = new SolidBrush(Color.FromArgb(255, 240, 242, 249));
		protected virtual Brush InnerTitlesBackground
		{
			get { return innerTitlesBG; }
		}

		static Brush contentBG = new SolidBrush(Color.FromArgb(255, 255, 255, 255));
		protected virtual Brush ContentBG
		{
			get { return contentBG; }
		}
		
		protected virtual bool RoundedCorners
		{
			get { return true; }
		}
		
		protected virtual int CornerRadius
		{
			get { return 15; }
		}
		#endregion
		
		#region Preparations
		
		protected IAmbience GetAmbience()
		{
			IAmbience ambience = AmbienceService.GetCurrentAmbience();
			ambience.ConversionFlags = ConversionFlags.None;
			return ambience;
		}
		
		protected virtual void PrepareTitles ()
		{
			if (classtype == null) return;
			
			IAmbience ambience = GetAmbience();
			
			DrawableItemsStack title = new DrawableItemsStack();
			title.OrientationAxis = Axis.X;
			
			TextSegment titleString = new TextSegment(base.Graphics, classtype.Name, TitleFont, true);
			title.Add(titleString);
			title.Add(collapseExpandShape);
			
			collapseExpandShape.Collapsed = Collapsed;
			
			titles.OrientationAxis = Axis.Y;
			
			titles.Add(title);
			
			titles.Add(new TextSegment(base.Graphics, typeclass, SubtextFont, true));

			if (classtype.BaseClass != null)
			{
				DrawableItemsStack inherits = new DrawableItemsStack();
				inherits.OrientationAxis = Axis.X;
				inherits.Add(new InheritanceShape());
				inherits.Add(new TextSegment(base.Graphics, classtype.BaseClass.Name, SubtextFont, true));
				titles.Add(inherits);
			}
			
			foreach (IReturnType rt in classtype.BaseTypes)
			{
				IClass ct = rt.GetUnderlyingClass();
				if (ct != null && ct.ClassType == ClassType.Interface)
					interfaces.Add(new TextSegment(base.Graphics, ambience.Convert(rt), SubtextFont, true));
			}
		}
		
		protected class MemberData : IComparable<MemberData>
		{
			public MemberData (IMember member, IAmbience ambience, Graphics graphics, Font font)
			{
				IMethod methodMember = member as IMethod;
				IEvent eventMember = member as IEvent;
				IProperty propertyMember = member as IProperty;
				IField fieldMember = member as IField;
				
				DrawableItemsStack<VectorShape> image = new DrawableItemsStack<VectorShape>();
				image.OrientationAxis = Axis.Z; // stack image components one on top of the other
				image.KeepAspectRatio = true;
				
				if (methodMember != null)
				{
					memberString = ambience.Convert(methodMember) + " : " + ambience.Convert(member.ReturnType);
					image.Add(new MethodShape());
				}
				else if (eventMember != null)
				{
					memberString = ambience.Convert(eventMember) + " : " + ambience.Convert(member.ReturnType);
					image.Add(new EventShape());
				}
				else if (fieldMember != null)
				{
					memberString = ambience.Convert(fieldMember) + " : " + ambience.Convert(member.ReturnType);
					image.Add(new FieldShape());
				}
				else if (propertyMember != null)
				{
					memberString = ambience.Convert(propertyMember) + " : " + ambience.Convert(member.ReturnType);
					image.Add(new PropertyShape());
				}
				
				memberItem.OrientationAxis = Axis.X;
				memberItem.Add(image);
				memberItem.Add(new TextSegment(graphics, memberString, font, true));
			
				image.Border = 1;
			}
			
			InteractiveItemsStack memberItem = new InteractiveItemsStack(false);
			
			string memberString;
			
			public string MemberString
			{
				get { return memberString; }
			}
			
			public int CompareTo(MemberData other)
			{
				return memberString.CompareTo(other.MemberString);
			}
			
			public InteractiveItemsStack Item
			{
				get { return memberItem; }
			}
		}
		
		protected InteractiveHeaderedItem PrepareGroup (string title, IDrawableRectangle content)
		{
			#region Prepare Group Container
			DrawableItemsStack headerPlus = new DrawableItemsStack();
			DrawableItemsStack headerMinus = new DrawableItemsStack();
			
			headerPlus.OrientationAxis = Axis.X;
			headerMinus.OrientationAxis = Axis.X;
			#endregion
			
			#region Create Header
			TextSegment titleSegment = new TextSegment(Graphics, title, GroupTitleFont, true);
			
			PlusShape plus = new PlusShape();
			plus.Border = 3;
			headerPlus.Add(plus);
			headerPlus.Add(titleSegment);
			
			MinusShape minus = new MinusShape();
			minus.Border = 3;
			headerMinus.Add(minus);
			headerMinus.Add(titleSegment);
			
			DrawableItemsStack headerCollapsed = new DrawableItemsStack();
			DrawableItemsStack headerExpanded = new DrawableItemsStack();
			
			headerCollapsed.OrientationAxis = Axis.Z;
			headerExpanded.OrientationAxis = Axis.Z;
			
			headerCollapsed.Add (new DrawableRectangle(InnerTitlesBackground, null));
			headerCollapsed.Add (headerPlus);
			
			headerExpanded.Add (new DrawableRectangle(InnerTitlesBackground, null));
			headerExpanded.Add (headerMinus);
			#endregion
			
			InteractiveHeaderedItem tg = new InteractiveHeaderedItem(headerCollapsed, headerExpanded, content);
			tg.HeaderClicked += delegate { tg.Collapsed = !tg.Collapsed; };
			IMouseInteractable interactive = content as IMouseInteractable;
			if (interactive != null)
				tg.ContentClicked += delegate (object sender, PointF pos) { interactive.HandleMouseClick(pos); };
			tg.RedrawNeeded += HandleRedraw;
			
			return tg;
		}
		
		protected virtual InteractiveItemsStack PrepareMembersContent <MT> (ICollection<MT> members) where MT : IMember
		{
			if (members == null) return null;
			if (members.Count == 0) return null;
			InteractiveItemsStack content = new InteractiveItemsStack();
			content.OrientationAxis = Axis.Y;
			PrepareMembersContent <MT> (members, content);
			return content;
		}
		
		private InteractiveItemsStack PrepareNestedTypesContent()
		{
			InteractiveItemsStack innerItems = new InteractiveItemsStack();
			innerItems.OrientationAxis = Axis.Y;
			innerItems.Spacing = 10;
			innerItems.Padding = 10;
			foreach (IClass ct in classtype.InnerClasses)
			{
				ClassCanvasItem innerItem = ClassCanvas.CreateItemFromType(ct);
				innerItems.Add(innerItem);
				innerItem.LayoutChanged += HandleRedraw;
			}
			return innerItems;
		}
		
		protected virtual void PrepareMembersContent <MT> (ICollection<MT> members, InteractiveItemsStack content) where MT : IMember
		{
			if (members == null) return;
			if (members.Count == 0) return;
			
			IAmbience ambience = GetAmbience();
			
			#region Prepare Group Members
			List<MemberData> membersData = new List<MemberData>();
			foreach (MT member in members)
			{
				membersData.Add(new MemberData(member, ambience, Graphics, MemberFont));
			}
			membersData.Sort();
			#endregion
			
			#region Add Members To Group
			foreach (MemberData memberData in membersData)
			{
				content.Add(memberData.Item);
			}
			#endregion
		}
		
		private void AddGroupToContent(string title, InteractiveItemsStack groupContent)
		{
			if (groupContent != null)
			{
				InteractiveHeaderedItem tg = PrepareGroup (title, groupContent);
				groupNames.Add(tg, title);
				groupsByName.Add(title, tg);
				groups.Add(tg);
			}
		}
		
		protected virtual void PrepareMembersContent ()
		{
			if (classtype == null) return;
			
			groups.Clear();
			
			InteractiveItemsStack propertiesContent = PrepareMembersContent <IProperty> (classtype.Properties);
			InteractiveItemsStack methodsContent = PrepareMembersContent <IMethod> (classtype.Methods);
			InteractiveItemsStack fieldsContent = PrepareMembersContent <IField> (classtype.Fields);
			InteractiveItemsStack eventsContent = PrepareMembersContent <IEvent> (classtype.Events);
			
			AddGroupToContent("Properties", propertiesContent);
			AddGroupToContent("Methods", methodsContent);
			AddGroupToContent("Fields", fieldsContent);
			AddGroupToContent("Events", eventsContent);
			
			if (classtype.InnerClasses.Count > 0)
			{
				InteractiveItemsStack nestedTypesContent = PrepareNestedTypesContent();
				AddGroupToContent("Nested Types", nestedTypesContent);
			}
		}
		
		protected virtual void PrepareFrame ()
		{
			ActualHeight = classItemContainer.GetAbsoluteContentHeight();
			
			if (Container != null) return;
			
			shadowpath = new GraphicsPath();
			if (RoundedCorners)
			{
				int radius = CornerRadius;
				shadowpath.AddArc(ActualWidth-radius + 4, 3, radius, radius, 300, 60);
				shadowpath.AddArc(ActualWidth-radius + 4, ActualHeight - radius + 3, radius, radius, 0, 90);
				shadowpath.AddArc(4, ActualHeight-radius + 3, radius, radius, 90, 45);
				shadowpath.AddArc(ActualWidth-radius, ActualHeight - radius, radius, radius, 90, -90);
			}
			else
			{
				shadowpath.AddPolygon(new PointF[] {
				                      	new PointF(ActualWidth, 3),
				                      	new PointF(ActualWidth + 4, 3),
				                      	new PointF(ActualWidth + 4, ActualHeight + 3),
				                      	new PointF(4, ActualHeight + 3),
				                      	new PointF(4, ActualHeight),
				                      	new PointF(ActualWidth, ActualHeight)
				                      });
			}
			shadowpath.CloseFigure();
		}
		
		#endregion
		
		public override void DrawToGraphics (Graphics graphics)
		{
			grad.ResetTransform();
			grad.TranslateTransform(AbsoluteX, AbsoluteY);
			grad.ScaleTransform(ActualWidth, 1);
			
			GraphicsState state = graphics.Save();
			graphics.TranslateTransform (AbsoluteX, AbsoluteY);
			
			if (Container == null)
			{
				//Draw Shadow
				graphics.FillPath(CanvasItem.ShadowBrush, shadowpath);
			}
			
			classItemContainer.Width = Width;
			classItemContainer.Height = Height;
				
			graphics.Restore(state);
			classItemContainer.DrawToGraphics(graphics);
			
			//Draw interfaces lollipops
			//TODO - should be converted to an headered item.
			if (interfaces.Count > 0)
			{
				interfaces.X = AbsoluteX + 15;
				interfaces.Y = AbsoluteY - interfaces.ActualHeight - 1;
				interfaces.DrawToGraphics(graphics);
				
				graphics.DrawEllipse(Pens.Black, AbsoluteX + 9, AbsoluteY - interfaces.ActualHeight - 11, 10, 10);
				graphics.DrawLine(Pens.Black, AbsoluteX + 14, AbsoluteY - interfaces.ActualHeight - 1, AbsoluteX + 14, AbsoluteY);
			}
			
			base.DrawToGraphics(graphics);
		}
		
		public bool Collapsed
		{
			get { return classItemHeaderedContent.Collapsed; }
			set
			{
				classItemHeaderedContent.Collapsed = value;
				collapseExpandShape.Collapsed = value;
				PrepareFrame();
				EmitLayoutUpdate();
			}
		}
		
		private void HandleRedraw (object sender, EventArgs args)
		{
			PrepareFrame();
			EmitLayoutUpdate();
		}
		
		#region Behaviour
		
		public override void HandleMouseClick (PointF pos)
		{
			base.HandleMouseClick(pos);

			if (collapseExpandShape.IsInside(pos.X, pos.Y))
			{
				Collapsed = !Collapsed;
			}
			else
			{
				foreach (InteractiveHeaderedItem tg in groups)
				{
					if (tg.HitTest(pos))
					{
						tg.HandleMouseClick(pos);
					}
				}
			}
		}
		
		#endregion
		
		#region Storage
		
		protected override XmlElement CreateXmlElement(XmlDocument doc)
		{
			return doc.CreateElement("Class");
		}
		
		protected override void FillXmlElement(XmlElement element, XmlDocument document)
		{
			base.FillXmlElement(element, document);
			element.SetAttribute("Name", RepresentedClassType.FullyQualifiedName);
			element.SetAttribute("Collapsed", Collapsed.ToString());
			
			//<Compartments>
			XmlElement compartments = document.CreateElement("Compartments");
			foreach (InteractiveHeaderedItem tg in groups)
			{
				XmlElement grp = document.CreateElement("Compartment");
				grp.SetAttribute("Name", groupNames[tg]);
				grp.SetAttribute("Collapsed", tg.Collapsed.ToString());
				compartments.AppendChild(grp);
			}
			element.AppendChild(compartments);
			
		}
		
		public override void LoadFromXml (XPathNavigator navigator)
		{
			base.LoadFromXml(navigator);
			
			Collapsed = bool.Parse(navigator.GetAttribute("Collapsed", ""));
			
			XPathNodeIterator compNI = navigator.Select("Compartments/Compartment");
			while (compNI.MoveNext())
			{
				XPathNavigator compNav = compNI.Current;
				InteractiveHeaderedItem grp;
				if (groupsByName.TryGetValue(compNav.GetAttribute("Name", ""), out grp))
				{
					grp.Collapsed = bool.Parse(compNav.GetAttribute("Collapsed", ""));
				}
			}
		}
		
		#endregion
		
		public void Dispose()
		{
			grad.Dispose();
			if (shadowpath != null)
				shadowpath.Dispose();
		}
		
		public override string ToString()
		{
			return "ClasCanvasItem: " + classtype.Name;
		}
	}
}
