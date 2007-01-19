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

namespace ClassDiagram
{
	public class ValueChangedEventArgs<T> : EventArgs
	{
		private T val;
		
		public ValueChangedEventArgs (T value)
		{
			this.val = value;
		}
		
		public virtual T Value
		{
			get { return val; }
			set { throw new InvalidOperationException("Cannot set the value of an event that type of event."); }
		}
		
		protected void SetValue (T value)
		{
			val = value;
		}
	}
	
	public class ValueChangingEventArgs<T> : ValueChangedEventArgs<T>
	{
		bool cancel;
		
		public ValueChangingEventArgs (T value) : base (value) {}
		
		public override T Value
		{
			set { base.SetValue(value); }
		}
		
		public bool Cancel
		{
			get { return cancel; }
			set { cancel = value; }
		}
	}
	
	public abstract class CanvasItem : IInteractiveDrawable, IRectangle
	{
		#region Constructors
		
		protected CanvasItem ()
		{
			Bitmap bitmap = new Bitmap(1, 1);
			this.g = Graphics.FromImage(bitmap);
		}
		
		#endregion
		
		public virtual bool IsHResizable
		{
			get { return true; }
		}
		
		public virtual bool IsVResizable
		{
			get { return true; }
		}
		
		#region Layout Events
		
		bool dontHandleLayoutChanges;
		bool layoutChangesOccured;
		
		public event EventHandler LayoutChanged = delegate {};
		public event EventHandler RedrawNeeded = delegate {};
		
		protected bool HoldLayoutChanges
		{
			get { return dontHandleLayoutChanges; }
			set
			{
				dontHandleLayoutChanges = value;
				if (layoutChangesOccured && !value)
					EmitLayoutUpdate();
			}
		}
		
		protected void EmitLayoutUpdate()
		{
			if (HoldLayoutChanges)
			{
				layoutChangesOccured = true;
				return;
			}
			
			LayoutChanged(this, EventArgs.Empty);
		}
		
		protected void EmitRedrawNeeded()
		{
			RedrawNeeded(this, EventArgs.Empty);
		}
		
		#endregion
		
		#region Geometry
		
		private float x, y, w, h;
		
		#region Geometry related events
		
		public event EventHandler<ValueChangingEventArgs<PointF>> PositionChanging = delegate { };
		public event EventHandler<ValueChangedEventArgs<PointF>> PositionChanged = delegate { };
		
		public event EventHandler<ValueChangingEventArgs<SizeF>> SizeChanging = delegate { };
		public event EventHandler<ValueChangedEventArgs<SizeF>> SizeChanged = delegate { };
		
		#endregion
		
		#region Geometry related methods
		
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
		public void Move (float x, float y)
		{
			ValueChangingEventArgs<PointF> e = new ValueChangingEventArgs<PointF>(new PointF(x, y));
			PositionChanging (this, e);
			if (e.Cancel) return;

			if (AllowXModifications())
				this.x = e.Value.X;
			else
				this.x = x;
			
			if (AllowYModifications())
				this.y = e.Value.Y;
			else
				this.y = y;

			PositionChanged (this, new ValueChangedEventArgs<PointF> (new PointF(this.x, this.y)));
			EmitLayoutUpdate();
		}
		
		public void Resize (float width, float height)
		{
			ValueChangingEventArgs<SizeF> e = new ValueChangingEventArgs<SizeF>(new SizeF(width, height));
			SizeChanging (this, e);
			if (e.Cancel) return;

			if (AllowWidthModifications())
				this.w = e.Value.Width;
			else
				this.w = width;
			
			if (AllowHeightModifications())
				this.h = e.Value.Height;
			else
				this.h = height;
			
			SizeChanged (this, new ValueChangedEventArgs<SizeF> (new SizeF(this.w, this.h)));
			EmitLayoutUpdate();
		}
		
		#endregion
		
		#region Geometry related properties
		
		#region IRectangle implementation
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "X")]
		public virtual float X
		{
			get { return x; }
			set { Move(value, y); }
		}
		
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Y")]
		public virtual float Y
		{
			get { return y; }
			set { Move(x, value); }
		}
		
		public virtual float AbsoluteX
		{
			get { return x; }
		}
		
		public virtual float AbsoluteY
		{
			get { return y; }
		}
		
		public virtual float Width
		{
			get { return w; }
			set { Resize (value, h); WidthChanged(this, EventArgs.Empty); }
		}
		
		public virtual float Height
		{
			get { return h; }
			set { Resize (w, value); HeightChanged(this, EventArgs.Empty); }
		}
		
		public virtual float ActualWidth
		{
			get { return Width; }
			set { Width = value; ActualWidthChanged(this, EventArgs.Empty); }
		}
		
		public virtual float ActualHeight
		{
			get { return Height; }
			set { Height = value; ActualHeightChanged(this, EventArgs.Empty); }
		}
		
		public virtual float Border
		{
			get { return 0; }
			set { }
		}
		
		public virtual float Padding
		{
			get { return 0; }
			set { }
		}
		
		public virtual IRectangle Container
		{
			get { return null; }
			set {}
		}

		#endregion

		#region Geometry Controllers
		protected virtual bool AllowXModifications ()
		{
			return true;
		}

		protected virtual bool AllowYModifications ()
		{
			return true;
		}
		
		protected virtual bool AllowWidthModifications ()
		{
			return true;
		}

		protected virtual bool AllowHeightModifications ()
		{
			return true;
		}
		#endregion
		
		#endregion
		
		#endregion
		
		#region Graphics
		
		Graphics g;
		
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		public static readonly Brush ShadowBrush = new SolidBrush(Color.FromArgb(64,0,0,0));
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		public static readonly Font MemberFont = new Font (FontFamily.GenericSansSerif, 11, FontStyle.Regular, GraphicsUnit.Pixel);
		
		/// <summary>
		/// Draws the item decorators to the given graphics object.
		/// Inheriters of this class should override this method and call the base implementation
		/// at the end of their implementation, or call DrawDecorators instead.
		/// </summary>
		/// <param name="graphics"></param>
		public virtual void DrawToGraphics (Graphics graphics)
		{
			DrawDecorators(graphics);
		}
		
		/// <summary>
		/// Returns an initialized <see cref="System.Drawing.Graphics">System.Drawing.Graphics</see>
		/// object, that should ONLY be used where a Graphics object is needed for initialization
		/// of other objects, like to create a font for text measurment.
		/// </summary>
		protected Graphics Graphics // TODO - try to abandon this
		{
			get { return g; }
		}
		
		#endregion
		
		#region Decorators
		
		List <RectangleDecorator> decorators = new List<RectangleDecorator>();
		
		public void AddDecorator (RectangleDecorator decorator)
		{
			decorators.Add(decorator);
			decorator.RedrawNeeded += HandleDecoratorRedrawRequest;
		}
		
		public void RemoveDecorator (RectangleDecorator decorator)
		{
			decorator.RedrawNeeded -= HandleDecoratorRedrawRequest;
			decorators.Remove(decorator);
		}
		
		protected void DrawDecorators (Graphics graphics)
		{
			foreach (RectangleDecorator decorator in decorators)
			{
				if (decorator.Active)
					decorator.DrawToGraphics(graphics);
			}
		}
		
		#endregion
		
		public virtual System.Windows.Forms.Control GetEditingControl() { return null; }
		public virtual bool StartEditing() { return false; }
		public virtual void StopEditing() {}
		
		#region Mouse Events
		
		#region Dragging Variables
		
		private PointF dragPos = new Point(0, 0);
		private PointF dragOfst = new Point(0, 0);
		private bool dragged;
		
		#endregion
		
		public virtual void HandleMouseClick (PointF pos)
		{
			foreach (RectangleDecorator decorator in decorators)
			{
				if (decorator.Active)
					decorator.HandleMouseClick(pos);
			}
		}
		
		public virtual void HandleMouseDown (PointF pos)
		{
			foreach (RectangleDecorator decorator in decorators)
			{
				if (decorator.Active)
					decorator.HandleMouseDown(pos);
			}
			
			bool hit = HitTest(pos);
			
			if (DragAreaHitTest(pos))
			{
				dragPos = pos;
				dragOfst.X = X - dragPos.X;
				dragOfst.Y = Y - dragPos.Y;
				dragged = true;
			}
		}
		
		public virtual void HandleMouseMove (PointF pos)
		{
			foreach (RectangleDecorator decorator in decorators)
			{
				if (decorator.Active)
					decorator.HandleMouseMove(pos);
			}
			
			if (dragged)
			{
				Move (dragOfst.X + pos.X, dragOfst.Y + pos.Y);
			}
		}
		
		public virtual void HandleMouseUp (PointF pos)
		{
			foreach (RectangleDecorator decorator in decorators)
			{
				if (decorator.Active)
					decorator.HandleMouseUp(pos);
			}
			
			dragged = false;
		}
		
		public virtual void HandleMouseLeave ()
		{
			foreach (RectangleDecorator decorator in decorators)
			{
				if (decorator.Active)
					decorator.HandleMouseLeave();
			}
		}
		
		#endregion
		
		#region Interactivity
		private void HandleDecoratorRedrawRequest (object sender, EventArgs args)
		{
			EmitRedrawNeeded();
		}

		public virtual bool HitTest(PointF pos)
		{
			return HitTest(pos, true);
		}
		
		public virtual bool HitTest(PointF pos, bool includeDecorators)
		{
			bool ret = (pos.X > X && pos.Y > Y && pos.X < X + ActualWidth && pos.Y < Y + ActualHeight);
			if (includeDecorators)
			{
				foreach (RectangleDecorator decorator in decorators)
				{
					if (decorator.Active)
						ret |= decorator.HitTest(pos);
				}
			}
			return ret;
		}
		
		protected virtual bool DragAreaHitTest(PointF pos)
		{
			return (pos.X > X && pos.Y > Y && pos.X < X + ActualWidth && pos.Y < Y + ActualHeight);
		}
		
		#endregion
		
		public virtual float GetAbsoluteContentWidth()
		{
			throw new NotImplementedException();
		}
		
		public virtual float GetAbsoluteContentHeight()
		{
			throw new NotImplementedException();
		}
		
		#region Storage
		
		protected virtual XmlElement CreateXmlElement(XmlDocument doc)
		{
			return doc.CreateElement("CanvasItem");
		}
		
		protected virtual void FillXmlElement (XmlElement element, XmlDocument document)
		{
			element.SetAttribute("X", X.ToString(CultureInfo.InvariantCulture));
			element.SetAttribute("Y", Y.ToString(CultureInfo.InvariantCulture));
			element.SetAttribute("Width", Width.ToString(CultureInfo.InvariantCulture));
		}
		
		public virtual void WriteToXml(XmlDocument document)
		{
			XmlElement elem = CreateXmlElement(document);
			FillXmlElement (elem, document);
			document.DocumentElement.AppendChild(elem);
		}
		
		public virtual void LoadFromXml (XPathNavigator navigator)
		{
			X = float.Parse(navigator.GetAttribute("X", ""), CultureInfo.InvariantCulture);
			Y = float.Parse(navigator.GetAttribute("Y", ""), CultureInfo.InvariantCulture);
			Width = float.Parse(navigator.GetAttribute("Width", ""), CultureInfo.InvariantCulture);
		}

		#endregion
		
		public bool KeepAspectRatio
		{
			get { return false; }
			set {}
		}
		
		public event EventHandler WidthChanged = delegate {};
		public event EventHandler HeightChanged = delegate {};
		public event EventHandler ActualWidthChanged = delegate {};
		public event EventHandler ActualHeightChanged = delegate {};
	}
}
