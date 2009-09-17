// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Itai Bar-Haim" email=""/>
//     <version>$Revision: 2231 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.ComponentModel;

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using System.Xml;
using System.Xml.XPath;

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

using Tools.Diagrams;

namespace ClassDiagram
{
	public partial class ClassCanvas
	{
		private class CanvasItemData : IDisposable
		{
			public CanvasItemData (CanvasItem item,
			                       EventHandler<SizeGripEventArgs> SizeGripMouseEntered,
			                       EventHandler<SizeGripEventArgs> SizeGripMouseLeft)
			{
				this.item = item;
				
				focusDecorator = new FocusDecorator(item);
				sizeGripDecorator = new SizeGripDecorator(item);
				
				sizeGripDecorator.SizeGripMouseEnter += SizeGripMouseEntered;
				sizeGripDecorator.SizeGripMouseLeave += SizeGripMouseLeft;
				
				item.AddDecorator(focusDecorator);
				item.AddDecorator(sizeGripDecorator);
			}
			
			CanvasItem item;
			
			public CanvasItem Item
			{
				get { return item; }
			}
			
			public bool Focused
			{
				get { return focusDecorator.Active; }
				set
				{
					focusDecorator.Active = value;
					sizeGripDecorator.Active = value;
				}
			}
			
			bool justGainedFocus;
			public bool JustGainedFocus
			{
				get { return justGainedFocus; }
				set { justGainedFocus = value; }
			}
			
			FocusDecorator focusDecorator;
			SizeGripDecorator sizeGripDecorator;
			
			public void Dispose()
			{
				item.RemoveDecorator(focusDecorator);
				item.RemoveDecorator(sizeGripDecorator);
			}
		}
		
		LinkedListNode<CanvasItemData> dragItemNode;
		LinkedListNode<CanvasItemData> hoverItemNode;
		LinkedList<CanvasItemData> itemsList = new LinkedList<CanvasItemData>();
		Dictionary<CanvasItem, CanvasItemData> itemsData = new Dictionary<CanvasItem, CanvasItemData>();
		Dictionary<IClass, CanvasItemData> classesToData = new Dictionary<IClass, CanvasItemData>();
				
		DiagramRouter diagramRouter = new DiagramRouter();
		
		public event EventHandler ZoomChanged = delegate { };
		float zoom = 1.0f;
		bool ctrlDown;
		bool holdRedraw;
		bool redrawNeeded;
		
		PointF lastMouseClickPosition;
		
		public ClassCanvas()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
		}
		
		#region Diagram Activities
		
		public float Zoom
		{
			get { return zoom; }
			set
			{
				zoom = value;
				pictureBox1.Invalidate();
				LayoutChanged (this, EventArgs.Empty);
				ZoomChanged(this, EventArgs.Empty);
			}
		}
		
		public void CollapseAll ()
		{
			foreach (CanvasItemData item in itemsList)
			{
				ClassCanvasItem classitem = item.Item as ClassCanvasItem;
				if (classitem != null)
					classitem.Collapsed = true;
			}
			LayoutChanged (this, EventArgs.Empty);
		}
		
		public void ExpandAll ()
		{
			foreach (CanvasItemData item in itemsList)
			{
				ClassCanvasItem classitem = item.Item as ClassCanvasItem;
				if (classitem != null)
					classitem.Collapsed = false;
			}
			LayoutChanged (this, EventArgs.Empty);
		}
		
		public void MatchAllWidths ()
		{
			foreach (CanvasItemData item in itemsList)
			{
				ClassCanvasItem classitem = item.Item as ClassCanvasItem;
				if (classitem != null)
					classitem.Width = classitem.GetAbsoluteContentWidth();
			}
			LayoutChanged (this, EventArgs.Empty);
		}
		
		public void ShrinkAllWidths ()
		{
			foreach (CanvasItemData item in itemsList)
			{
				ClassCanvasItem classitem = item.Item as ClassCanvasItem;
				if (classitem != null)
					classitem.Width = 0;
			}
			LayoutChanged (this, EventArgs.Empty);
		}
		
		#endregion
		
		public SizeF GetDiagramLogicalSize ()
		{
			float w=1, h=1;
			foreach (CanvasItemData item in itemsList)
			{
				w = Math.Max(w, item.Item.X + item.Item.ActualWidth + item.Item.Border);
				h = Math.Max(h, item.Item.Y + item.Item.ActualHeight + item.Item.Border);
			}
			return new SizeF(w + 50, h + 50);
		}
		
		public Size GetDiagramPixelSize ()
		{
			float zoom = Math.Max(this.zoom, 0.1f);
			SizeF size = GetDiagramLogicalSize();
			return new Size((int)(size.Width * zoom), (int)(size.Height * zoom));
		}
		
		public void SetRecommendedGraphicsAttributes (Graphics graphics)
		{
			if (graphics == null) return;
			graphics.CompositingQuality = CompositingQuality.HighSpeed;
			//graphics.SmoothingMode = SmoothingMode.AntiAlias;
			graphics.SmoothingMode = SmoothingMode.HighQuality;
			graphics.PageUnit = GraphicsUnit.Pixel;
			graphics.PixelOffsetMode = PixelOffsetMode.Half;
			graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
		}
		
		public void DrawToGraphics(Graphics graphics)
		{
			foreach (CanvasItemData item in itemsList)
				item.Item.DrawToGraphics(graphics);

			DrawRoutes(graphics);
		}
		
		private void PictureBox1Paint (object sender, PaintEventArgs e)
		{
//			System.Diagnostics.Debug.WriteLine("ClassCanvas.PictureBox1Paint");
			Size bbox = GetDiagramPixelSize();
			
			pictureBox1.Width = Math.Min(10000, bbox.Width + 100);
			pictureBox1.Height = Math.Min(10000, bbox.Height + 100);

			e.Graphics.PageScale = zoom;
			SetRecommendedGraphicsAttributes(e.Graphics);
			DrawToGraphics(e.Graphics);
		}
		
		private void DrawRoutes (Graphics g)
		{
			foreach (Route route in diagramRouter.Routes)
			{
				route.Recalc(itemsList as IEnumerable<IRectangle>);
				PointF origin = route.GetStartPoint();
				RouteSegment[] segments = route.RouteSegments;
				foreach (RouteSegment rs in segments)
				{
					PointF dest = rs.CreateDestinationPoint(origin);
					g.DrawLine(Pens.Black, origin, dest);
					origin = dest;
				}
				
				if (route.EndShape != null)
					((RouteShape)route.EndShape).Draw(g, route, true);

				if (route.StartShape != null)
					((RouteShape)route.StartShape).Draw(g, route, false);
			}
		}
		
		private LinkedListNode<CanvasItemData> FindCanvasItemNode (PointF pos)
		{
			LinkedListNode<CanvasItemData> itemNode = itemsList.Last;
			while (itemNode != null)
			{
				if (itemNode.Value.Item.HitTest(pos))
				{
					return itemNode;
				}
				itemNode = itemNode.Previous;
			}
			return null;
		}
		
		#region Diagram Items Drag and Selection
		
		private void PictureBox1MouseClick (object sender, MouseEventArgs e)
		{
			PointF pos = new PointF(e.X / zoom, e.Y / zoom);
			lastMouseClickPosition = pos;
			LinkedListNode<CanvasItemData> itemNode = FindCanvasItemNode(pos);
			if (itemNode != null)
			{
				itemNode.Value.Item.HandleMouseClick(pos);
				if (itemNode.Value.Focused)
				{
					if (itemNode.Value.JustGainedFocus)
					{
						itemNode.Value.JustGainedFocus = false;
					}
					else if (itemNode.Value.Item.StartEditing())
					{
						Control ec = itemNode.Value.Item.GetEditingControl();
						if (ec != null)
						{
							//TODO - refactor this damn thing... why couldn't they make the "Scale" scale the font as well?
							ec.Scale(new SizeF(zoom, zoom));
							Font ecf = ec.Font;
							ec.Font = new Font(ecf.FontFamily,
							                   ecf.Size * zoom,
							                   ecf.Style, ec.Font.Unit,
							                   ecf.GdiCharSet, ec.Font.GdiVerticalFont);
							ec.Hide();
							ec.VisibleChanged += delegate { if (!ec.Visible) ec.Font = ecf; };
							panel1.Controls.Add(ec);
							ec.Top -= panel1.VerticalScroll.Value;
							ec.Left -= panel1.HorizontalScroll.Value;
							ec.Show();
							panel1.Controls.SetChildIndex(ec, 0);
							this.ActiveControl = ec;
							ec.Focus();
						}
					}
				}
			}
		}
		
		private void PictureBox1MouseDown (object sender, MouseEventArgs e)
		{
			HoldRedraw = true;
			PointF pos = new PointF(e.X / zoom, e.Y / zoom);
			LinkedListNode<CanvasItemData> itemNode = FindCanvasItemNode(pos);
			dragItemNode = itemNode;
			
			if (!ctrlDown)
			{
				foreach (CanvasItemData item in itemsList)
				{
					item.Item.StopEditing();
					if (itemNode == null || item != itemNode.Value)
						item.Focused = false;
				}
			}
			
			if (itemNode != null)
			{
				if (!itemNode.Value.Focused)
				{
					itemNode.Value.JustGainedFocus = true;
					itemNode.Value.Focused = true;
					itemsList.Remove(itemNode);
					itemsList.AddLast(itemNode);
					CanvasItemSelected (this, new CanvasItemEventArgs (itemNode.Value.Item));
				}
				itemNode.Value.Item.HandleMouseDown(pos);
			}
			HoldRedraw = false;
		}
		
		private void PictureBox1MouseMove (object sender, MouseEventArgs e)
		{
			HoldRedraw = true;
			PointF pos = new PointF(e.X / zoom, e.Y / zoom);
			if (dragItemNode != null)
				dragItemNode.Value.Item.HandleMouseMove(pos);
			else
			{
				LinkedListNode<CanvasItemData> itemNode = FindCanvasItemNode(pos);
				if (hoverItemNode != itemNode)
				{
					if (hoverItemNode != null && hoverItemNode.Value != null)
						hoverItemNode.Value.Item.HandleMouseLeave();
					hoverItemNode = itemNode;
				}
				
				if (itemNode != null)
					itemNode.Value.Item.HandleMouseMove(pos);
			}
			HoldRedraw = false;
		}
		
		private void PictureBox1MouseUp (object sender, MouseEventArgs e)
		{
			PointF pos = new PointF(e.X / zoom, e.Y / zoom);
			
			if (dragItemNode != null)
				dragItemNode.Value.Item.HandleMouseUp(pos);
			dragItemNode = null;

			LinkedListNode<CanvasItemData> itemNode = FindCanvasItemNode(pos);
			if (itemNode != null)
				itemNode.Value.Item.HandleMouseUp(pos);
		}
		
		#endregion
		
		private bool HoldRedraw
		{
			get { return holdRedraw; }
			set
			{
				holdRedraw = value;
				if (!value && redrawNeeded)
				{
					redrawNeeded = false;
					HandleRedraw (this, EventArgs.Empty);
				}
			}
		}

		private void HandleItemLayoutChange (object sender, EventArgs args)
		{
			LayoutChanged (this, args);
			if (HoldRedraw)
				redrawNeeded = true;
			else
				HandleRedraw (sender, args);
		}

		private void HandleRedraw (object sender, EventArgs args)
		{
			if (HoldRedraw)
			{
				redrawNeeded = true;
				return;
			}
//			System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();
//			System.Diagnostics.Debug.WriteLine(st.ToString());
			this.Invalidate(true);
		}
		
		private void HandleItemPositionChange (object sender, ValueChangingEventArgs<PointF> args)
		{
			PointF pos = new PointF(args.Value.X, args.Value.Y);
			
			pos.X = Math.Max ((float) Math.Round(pos.X / 10.0f) * 10.0f, 50.0f);
			pos.Y = Math.Max ((float) Math.Round(pos.Y / 10.0f) * 10.0f, 50.0f);
			
			args.Cancel = (pos.X == args.Value.X) && (pos.Y == args.Value.Y);
			args.Value = pos;
		}
		
		private void HandleItemSizeChange (object sender, ValueChangingEventArgs<SizeF> args)
		{
			SizeF size = new SizeF(args.Value);
			
			size.Width = (float) Math.Round(size.Width / 10.0f) * 10.0f;
			size.Height = (float) Math.Round(size.Height / 10.0f) * 10.0f;
			
			args.Cancel = (size.Width == args.Value.Width) && (size.Height == args.Value.Height);
			args.Value = size;
		}
				
		private void SizeGripMouseEntered (object sender, SizeGripEventArgs e)
		{
			if ((e.GripPosition & SizeGripPositions.EastWest) != SizeGripPositions.None)
			{
				pictureBox1.Cursor = Cursors.SizeWE;
			}
			else if ((e.GripPosition & SizeGripPositions.NorthSouth) != SizeGripPositions.None)
			{
				pictureBox1.Cursor = Cursors.SizeNS;
			}
		}

		private void SizeGripMouseLeft (object sender, SizeGripEventArgs e)
		{
			pictureBox1.Cursor = Cursors.Default;
		}
		
		public void AddCanvasItem (CanvasItem item)
		{
			diagramRouter.AddItem(item);
			CanvasItemData itemData = new CanvasItemData(item, SizeGripMouseEntered, SizeGripMouseLeft);
			itemsData[item] = itemData;

			ClassCanvasItem classItem = item as ClassCanvasItem;
			if (classItem != null)
			{
				classesToData.Add(classItem.RepresentedClassType, itemData);
				foreach (CanvasItemData ci in itemsList)
				{
					ClassCanvasItem cci = ci.Item as ClassCanvasItem;
					if (cci != null)
					{
						Route r = null;
						if (cci.RepresentedClassType == classItem.RepresentedClassType.BaseClass)
							r = diagramRouter.AddRoute(item, cci);
						else if (classItem.RepresentedClassType == cci.RepresentedClassType.BaseClass)
							r = diagramRouter.AddRoute(cci, classItem);
						
						if (r != null)
							r.EndShape = new RouteInheritanceShape();
					}
				}
			}

			itemsList.AddLast(itemData);
			item.RedrawNeeded += HandleRedraw;
			item.LayoutChanged += HandleItemLayoutChange;
			item.PositionChanging += HandleItemPositionChange;
			item.SizeChanging += HandleItemSizeChange;
		}
		
		public void RemoveCanvasItem (CanvasItem item)
		{
			itemsList.Remove(itemsData[item]);
			Stack<Route> routesToRemove = new Stack<Route>();
			foreach (Route r in diagramRouter.Routes)
			{
				if (r.From == item || r.To == item)
					routesToRemove.Push(r);
			}
			
			foreach (Route r in routesToRemove)
				diagramRouter.RemoveRoute(r);

			diagramRouter.RemoveItem (item);

			ClassCanvasItem classItem = item as ClassCanvasItem;
			if (classItem != null)
			{
				classesToData.Remove (classItem.RepresentedClassType);
			}
			
			LayoutChanged(this, EventArgs.Empty);
		}
		
		public void ClearCanvas()
		{
			itemsList.Clear();
			classesToData.Clear();
			itemsList.Clear();
			dragItemNode = null;
			hoverItemNode = null;
			diagramRouter.Clear();
		}
		
		/// <summary>
		/// Retruns a copy of the the canvas items list as an array.
		/// </summary>
		public CanvasItem[] GetCanvasItems()
		{
			CanvasItem[] items = new CanvasItem[itemsList.Count];
			int i = 0;
			foreach (CanvasItemData item in itemsList)
				items[i++] = item.Item;
			return items;
		}
		
		public bool Contains (IClass ct)
		{
			return classesToData.ContainsKey(ct);
			/*
 			foreach (CanvasItemData ci in itemsList)
			{
				ClassCanvasItem cci = ci.Item as ClassCanvasItem;
				if (cci != null)
					if (cci.RepresentedClassType.Equals(ct)) return true;
			}*/
			
			//return false;
		}
		
		public void AutoArrange ()
		{
			diagramRouter.RecalcPositions();
		}
		
		public static ClassCanvasItem CreateItemFromType (IClass ct)
		{
			if (ct == null) return null;
			ClassCanvasItem item = null;
			if (ct.ClassType == ClassType.Interface)
				item = new InterfaceCanvasItem(ct);
			else if (ct.ClassType == ClassType.Enum)
				item = new EnumCanvasItem(ct);
			else if (ct.ClassType == ClassType.Struct)
				item = new StructCanvasItem(ct);
			else if (ct.ClassType == ClassType.Delegate)
				item = new DelegateCanvasItem(ct);
			else
				item = new ClassCanvasItem(ct);
			item.Initialize();
			return item;
		}
		
		#region File Save/Load
		
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode")]
		public XmlDocument WriteToXml ()
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml("<ClassDiagram/>");
			
			XmlDeclaration decl = doc.CreateXmlDeclaration("1.0", "utf-8", "yes");
			doc.InsertBefore(decl, doc.FirstChild);
			
			XmlAttribute zoom = doc.CreateAttribute("Zoom"); // Non-Standard attribute
			zoom.Value = Zoom.ToString(System.Globalization.CultureInfo.InvariantCulture);
			doc.DocumentElement.Attributes.Append(zoom);
			
			#region unsupported attributes - added for compatability
			// FIXME - Attribute not yet supported
			XmlAttribute majorVersion = doc.CreateAttribute("MajorVersion");
			majorVersion.Value = "1";
			doc.DocumentElement.Attributes.Append(majorVersion);
			
			// FIXME - Attribute not yet supported
			XmlAttribute minorVersion = doc.CreateAttribute("MinorVersion");
			minorVersion.Value = "1";
			doc.DocumentElement.Attributes.Append(minorVersion);

			// FIXME - Attribute not yet supported
			XmlAttribute membersFormat = doc.CreateAttribute("MembersFormat");
			membersFormat.Value = "FullSignature";
			doc.DocumentElement.Attributes.Append(membersFormat);
			
			// FIXME - Element not yet supported
			XmlAttribute fontName = doc.CreateAttribute("Name");
			fontName.Value = "Tahoma";

			XmlAttribute fontSize = doc.CreateAttribute("Size");
			fontSize.Value = "8.25";
			
			XmlElement fontElement = doc.CreateElement("Font");
			fontElement.Attributes.Append(fontName);
			fontElement.Attributes.Append(fontSize);
			#endregion
			
			foreach (CanvasItemData item in itemsList)
			{
				item.Item.WriteToXml(doc);
			}
			return doc;
		}
		
		public void LoadFromXml (IXPathNavigable doc, IProjectContent pc)
		{
			if (pc == null) return;
			if (doc == null) return;
			ClearCanvas();
			
			XPathNavigator nav = doc.CreateNavigator();
			XPathNodeIterator ni = nav.Select(@"/ClassDiagram/Class | /ClassDiagram/Struct | /ClassDiagram/Enum | /ClassDiagram/Interface | /ClassDiagram/Delegate");
			while (ni.MoveNext())
			{
				string typeName = ni.Current.GetAttribute("Name", "");
				IClass ct = pc.GetClass(typeName, 0);
				ClassCanvasItem canvasitem = ClassCanvas.CreateItemFromType(ct);
				if (canvasitem != null)
				{
					canvasitem.LoadFromXml (ni.Current);
					AddCanvasItem(canvasitem);
				}
			}
			ni = nav.Select(@"/ClassDiagram/Comment");
			while (ni.MoveNext())
			{
				NoteCanvasItem note = new NoteCanvasItem();
				note.LoadFromXml(ni.Current);
				AddCanvasItem(note);
			}
		}
		
		#endregion
		
		public event EventHandler LayoutChanged = delegate {};
		public event EventHandler<CanvasItemEventArgs> CanvasItemSelected = delegate {};
		
		public Bitmap GetAsBitmap ()
		{
			Size bbox = GetDiagramPixelSize();
			Bitmap bitmap = new Bitmap(bbox.Width, bbox.Height);
			Graphics g = Graphics.FromImage(bitmap);
			g.PageScale = zoom;
			SetRecommendedGraphicsAttributes(g);
			DrawToGraphics(g);
			return bitmap;
		}
		
		public void SaveToImage (string filename)
		{
			GetAsBitmap().Save(filename);
		}
		
		public PointF LastMouseClickPosition
		{
			get { return lastMouseClickPosition; }
		}
		
		#region Drag/Drop from Class Browser Handling
		
		private void ClassCanvasDragOver(object sender, DragEventArgs e)
		{
			e.Effect = DragDropEffects.Link;
		}
		
		private void ClassCanvasDragDrop(object sender, DragEventArgs e)
		{
			
		}
		
		#endregion
		
		void ClassCanvasKeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Control)
				ctrlDown = true;
		}
		
		void ClassCanvasKeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Control)
				ctrlDown = false;
		}
	}

	public class CanvasItemEventArgs : EventArgs
	{
		public CanvasItemEventArgs (CanvasItem canvasItem)
		{
			this.canvasItem = canvasItem;
		}
		
		private CanvasItem canvasItem;
		
		public CanvasItem CanvasItem
		{
			get { return canvasItem; }
		}
	}
	
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
	public enum TestEnum {Dog, Cat, Fish};
}
