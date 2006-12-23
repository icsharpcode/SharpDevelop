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
	public class ClassCanvasItem : CanvasItem, IDisposable
	{
		IClass classtype;
		string typeclass;
		InteractiveHeaderedItem classItemHeaderedContent;
		DrawableItemsStack classItemContainer = new DrawableItemsStack();

		const int radius = 20;

		#region Graphics related variables
		
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		public static readonly Font TitleFont = new Font (FontFamily.GenericSansSerif, 11, FontStyle.Bold, GraphicsUnit.Pixel);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		public static readonly Font SubtextFont = new Font (FontFamily.GenericSansSerif, 11, FontStyle.Regular, GraphicsUnit.Pixel);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		public static readonly Font GroupTitleFont = new Font (FontFamily.GenericSansSerif, 11, FontStyle.Regular, GraphicsUnit.Pixel);

		LinearGradientBrush grad;
		
		GraphicsPath shadowpath;
		
		#endregion

		CollapseExpandShape collapseExpandShape = new CollapseExpandShape();

		DrawableItemsStack titles = new DrawableItemsStack();

		DrawableItemsStack titlesCollapsed = new DrawableItemsStack();
		DrawableItemsStack titlesExpanded = new DrawableItemsStack();
		
		DrawableItemsStack<InteractiveHeaderedItem> groups = new DrawableItemsStack<InteractiveHeaderedItem>();
		Dictionary<InteractiveHeaderedItem, string> groupNames = new Dictionary<InteractiveHeaderedItem, string>(); // TODO - this is really an ugly patch

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
			
			classItemHeaderedContent = new InteractiveHeaderedItem(titlesCollapsed, titlesExpanded, InitContentContainer(InitContent()));

			classItemContainer.Container = this;
			classItemContainer.Add(classItemHeaderedContent);
			classItemContainer.Add(new DrawableRectangle(null, Pens.Gray, radius, radius, radius, radius));
			classItemContainer.OrientationAxis = Axis.Z;
			
			grad = new LinearGradientBrush(
				new PointF(0, 0), new PointF(1, 0),
				TitleBackground, Color.White);
			
			titlesBackgroundCollapsed = new DrawableRectangle(grad, null, radius, radius, radius, radius);
			titlesBackgroundExpanded = new DrawableRectangle(grad, null, radius, radius, 1, 1);

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
		
		public override bool IsVResizable
		{
			get { return false; }
		}

		protected virtual DrawableItemsStack InitContentContainer(params IDrawableRectangle[] items)
		{
			DrawableItemsStack content = new DrawableItemsStack();
			content.OrientationAxis = Axis.Z;
			content.Add(new DrawableRectangle(Brushes.White, null, 1, 1, radius, radius));
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
		
		protected virtual Color TitleBackground
		{
			get { return Color.LightSteelBlue;}
		}

		protected virtual Brush InnerTitlesBackground
		{
			get { return Brushes.AliceBlue;}
		}

		#endregion
		
		#region Preparations
		
		protected IAmbience GetAmbience()
		{
			IAmbience ambience = null;
			
			try
			{
				ambience = AmbienceService.CurrentAmbience;
			}
			catch (NullReferenceException)
			{
				ambience = ICSharpCode.SharpDevelop.Dom.CSharp.CSharpAmbience.Instance;
			}
			
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
			
			DrawableItemsStack memberItem = new DrawableItemsStack();
			
			string memberString;
			
			public string MemberString
			{
				get { return memberString; }
			}
			
			public int CompareTo(MemberData other)
			{
				return memberString.CompareTo(other.MemberString);
			}
			
			public DrawableItemsStack<IDrawableRectangle> Item
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
			
			headerPlus.Add(new PlusShape());
			headerPlus.Add(titleSegment);
			
			headerMinus.Add(new MinusShape());
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
			tg.RedrawNeeded += HandleRedraw;
			
			return tg;
		}
		
		protected virtual DrawableItemsStack PrepareMembersContent <MT> (ICollection<MT> members) where MT : IMember
		{
			if (members == null) return null;
			if (members.Count == 0) return null;
			DrawableItemsStack content = new DrawableItemsStack();
			content.OrientationAxis = Axis.Y;
			PrepareMembersContent <MT> (members, content);
			return content;
		}
		
		protected virtual void PrepareMembersContent <MT> (ICollection<MT> members, DrawableItemsStack content) where MT : IMember
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
		
		private void AddGroupToContent(string title, DrawableItemsStack groupContent)
		{
			if (groupContent != null)
			{
				InteractiveHeaderedItem tg = PrepareGroup (title, groupContent);
				groupNames.Add(tg, title);
				groups.Add(tg);
			}
		}
			
		protected virtual void PrepareMembersContent ()
		{
			if (classtype == null) return;
			
			groups.Clear();
			
			DrawableItemsStack propertiesContent = PrepareMembersContent <IProperty> (classtype.Properties);
			DrawableItemsStack methodsContent = PrepareMembersContent <IMethod> (classtype.Methods);
			DrawableItemsStack fieldsContent = PrepareMembersContent <IField> (classtype.Fields);
			DrawableItemsStack eventsContent = PrepareMembersContent <IEvent> (classtype.Events);
			
			AddGroupToContent("Properties", propertiesContent);
			AddGroupToContent("Methods", methodsContent);
			AddGroupToContent("Fields", fieldsContent);
			AddGroupToContent("Events", eventsContent);
		}
		
		protected virtual void PrepareFrame ()
		{
			ActualHeight = classItemContainer.GetAbsoluteContentHeight();

			shadowpath = new GraphicsPath();
			shadowpath.AddArc(ActualWidth-radius + 4, 3, radius, radius, 300, 60);
			shadowpath.AddArc(ActualWidth-radius + 4, ActualHeight-radius + 3, radius, radius, 0, 90);
			shadowpath.AddArc(4, ActualHeight-radius + 3, radius, radius, 90, 45);
			shadowpath.AddArc(ActualWidth-radius, ActualHeight-radius, radius, radius, 90, -90);
			shadowpath.CloseFigure();
		}
		
		#endregion
		
		public override void DrawToGraphics (Graphics graphics)
		{
			grad.ResetTransform();
			grad.TranslateTransform(X, Y);
			grad.ScaleTransform(ActualWidth, 1);
			
			GraphicsState state = graphics.Save();
			graphics.TranslateTransform (X, Y);
			
			//Draw Shadow
			graphics.FillPath(CanvasItem.ShadowBrush, shadowpath);
					
			classItemContainer.Width = Width;
			classItemContainer.Height = Height;
			
			graphics.Restore(state);
			
			classItemContainer.DrawToGraphics(graphics);
			
			//Draw interfaces lollipops
			//TODO - should be converted to an headered item.
			if (interfaces.Count > 0)
			{
				interfaces.X = X + 15;
				interfaces.Y = Y - interfaces.ActualHeight - 1;
				interfaces.DrawToGraphics(graphics);
				
				graphics.DrawEllipse(Pens.Black, X + 9, Y - interfaces.ActualHeight - 11, 10, 10);
				graphics.DrawLine(Pens.Black, X + 14, Y - interfaces.ActualHeight - 1, X + 14, Y);
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
			return doc.CreateElement("ClassItem");
		}
		
		protected override void FillXmlElement(XmlElement element, XmlDocument document)
		{
			base.FillXmlElement(element, document);
			element.SetAttribute("Type", RepresentedClassType.FullyQualifiedName);
			element.SetAttribute("Collapsed", Collapsed.ToString());
			
			foreach (InteractiveHeaderedItem tg in groups)
			{
				XmlElement grp = document.CreateElement(groupNames[tg]);
				grp.SetAttribute("Collapsed", tg.Collapsed.ToString());
				element.AppendChild(grp);
			}
			
		}
		
		public override void LoadFromXml (XPathNavigator navigator)
		{
			base.LoadFromXml(navigator);
			
			Collapsed = bool.Parse(navigator.GetAttribute("Collapsed", ""));
			
			foreach (InteractiveHeaderedItem tg in groups)
			{
				XPathNodeIterator ni = navigator.SelectChildren(groupNames[tg], "");
				ni.MoveNext();
				tg.Collapsed = bool.Parse(ni.Current.GetAttribute("Collapsed", ""));
			}
		}
		
		#endregion
		
		public void Dispose()
		{
			grad.Dispose();
			shadowpath.Dispose();
		}
		
		public override string ToString()
		{
			return "ClasCanvasItem: " + classtype.Name;
		}
	}
	
	/// <summary>
	/// Test interface
	/// </summary>
	public interface TestInterface
	{
		
	}
	
	/// <summary>
	/// Test class.
	/// </summary>
	public class TestClass_Long_Title : TestInterface
	{
		/// <summary>
		/// A method with a common test name and one parameter.
		/// </summary>
		public void foo(string str) {}
		
		/// <summary>
		/// Some test field.
		/// </summary>
		public int bar;
		
		/// <summary>
		/// The getter for the 'bar' field.
		/// </summary>
		public int Bar { get { return bar; } }
		
		/// <summary>
		/// A simple test event.
		/// </summary>
		public event EventHandler stupid = delegate {};
	}

}
