/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 04.12.2007
 * Zeit: 10:05
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace ICSharpCode.Reports.Addin
{
	/// <summary>
	/// Description of LineDesigner.
	/// </summary>
	public class LineDesigner:ControlDesigner
	{
		private BaseLineItem baseLine;
		private ISelectionService selectionService;
		private IComponentChangeService componentChangeService;
		private bool dragging;
		private bool overToPoint;
		private bool overFromPoint;
		
	
		
		public override void Initialize(IComponent component)
		{
			if (component == null) {
				throw new ArgumentNullException("component");
			}
			base.Initialize(component);
			this.baseLine = (BaseLineItem)component;
			
			this.componentChangeService = (IComponentChangeService)component.Site.GetService(typeof(IComponentChangeService));
			if (componentChangeService != null) {
				componentChangeService.ComponentChanging += OnComponentChanging;
				componentChangeService.ComponentChanged += OnComponentChanged;
			}
			
			selectionService = GetService(typeof(ISelectionService)) as ISelectionService;
			if (selectionService != null)
			{
				selectionService.SelectionChanged += OnSelectionChanged;
			}
			
		}
		
		#region events
		
		private void OnComponentChanging (object sender,ComponentChangingEventArgs e)
		{
//			System.Console.WriteLine("changing");
//			System.Console.WriteLine("{0}",this.baseLine.ClientRectangle);
		}
		
		
		private void OnComponentChanged(object sender,ComponentChangedEventArgs e)
		{
			System.Console.WriteLine("changed");
			System.Console.WriteLine("{0}",this.baseLine.ClientRectangle);
		}
		
		
		private void OnSelectionChanged(object sender, EventArgs e)
		{
			Control.Invalidate(  );
		}


		#endregion
	
		
		protected override void OnPaintAdornments(PaintEventArgs pe)
		{
			BaseLineItem label = (BaseLineItem) Control;
			
			if (selectionService != null)
			{
				if (selectionService.GetComponentSelected(label))
				{
					// Paint grab handles.
					Rectangle grapRectangle = GetHandle(label.FromPoint);
					ControlPaint.DrawGrabHandle(pe.Graphics, grapRectangle, true, true);
					grapRectangle = GetHandle(label.ToPoint);
					ControlPaint.DrawGrabHandle(pe.Graphics, grapRectangle, true, true);
					
				}
			}
		}
	
		
		private static Rectangle GetHandle(Point pt)
		{
			Rectangle handle = new Rectangle(pt, new Size(7, 7));
			handle.Offset(-3, -3);
			return handle;
		}
		
		
		protected override void OnSetCursor(  )
		{
			// Get mouse cursor position relative to
			// the control's coordinate space.
			
			BaseLineItem label = (BaseLineItem) Control;
			Point p = label.PointToClient(Cursor.Position);
			
			// Display a resize cursor if the mouse is
			// over a grab handle; otherwise show a
			// normal arrow.
			if (GetHandle(label.FromPoint).Contains(p) ||
			    GetHandle(label.ToPoint).Contains(p))
			{
				Cursor.Current = Cursors.SizeAll;
			}
			else
			{
				Cursor.Current = Cursors.Default;
			}
		}

		
		#region Drag handling state and methods
		
		protected override void OnMouseDragBegin(int x, int y)
		{
			System.Console.WriteLine("DragBegib");
			Point p = this.baseLine.PointToClient(new Point(x, y));
			overFromPoint = GetHandle(this.baseLine.FromPoint).Contains(p);
			this.overToPoint = GetHandle(this.baseLine.ToPoint).Contains(p);
			if (overFromPoint || overToPoint )
			{
				dragging = true;
//					PropertyDescriptor pd =
//						TypeDescriptor.GetProperties(this.baseLine)["FromPoint"];
//					pd.SetValue(this.baseLine, p);
//				dragDirection = overToPoint;
				//            Point current = dragDirection ?
				//                (label.Origin + label.Direction) :
				//                label.Origin;
				//            dragOffset = current - new Size(p);
			}
			else
			{
				dragging = false;
				base.OnMouseDragBegin(x, y);
			}
		}
		
		
		protected override void OnMouseDragMove(int x, int y)
		{
			if (dragging)
			{
				Point p = this.baseLine.PointToClient(new Point(x, y));
				if (this.overToPoint) {
					this.baseLine.ToPoint = p;
				} else {
					this.baseLine.FromPoint = p;
				}
				
//				this.baseLine.Invalidate();
//				this.dragOffset = p;
			}
			else
			{
				base.OnMouseDragMove(x, y);
			}
		}
		
		
		protected override void OnMouseDragEnd(bool cancel)
		{
			if (dragging)
			{
				// Update property via PropertyDescriptor to
				// make sure that VS.NET notices.
			
//				PropertyDescriptor pd =
//						TypeDescriptor.GetProperties(this.baseLine)["ToPoint"];
//					pd.SetValue(this.baseLine, this.dragOffset);
				/*
				DirectionalLabel label = (DirectionalLabel) Control;
				if (dragDirection)
				{
					Size d = label.Direction;
					PropertyDescriptor pd =
						TypeDescriptor.GetProperties(label)["Direction"];
					pd.SetValue(label, d);
				}
				else
				{
					Point o = label.Origin;
					PropertyDescriptor pd =
						TypeDescriptor.GetProperties(label)["Origin"];
					pd.SetValue(label, o);
				}
				*/				
				dragging = false;
				this.baseLine.Invalidate();
			}
			
			// Always call base class.
			base.OnMouseDragEnd(cancel);

		}
		//
		
		#endregion
		
		
		protected override void Dispose(bool disposing)
		{
		
			if (this.componentChangeService != null) {
				componentChangeService.ComponentChanging -= OnComponentChanging;
				componentChangeService.ComponentChanged -= OnComponentChanged;
			}
			if (this.selectionService != null) {
				selectionService.SelectionChanged -= OnSelectionChanged;
			}
			
			base.Dispose(disposing);
		}
	}
}
