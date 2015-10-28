/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 23.03.2014
 * Time: 17:56
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.Reporting.Addin.DesignableItems;
using ICSharpCode.Reporting.Addin.TypeProvider;

namespace ICSharpCode.Reporting.Addin.Designer
{
	/// <summary>
	/// Description of LineDesigner.
	/// </summary>
	class LineDesigner:AbstractDesigner
	{
		BaseLineItem baseLine;
		bool dragging;
		bool overToPoint;
		bool overFromPoint;
		
	
		public override void Initialize(IComponent component){
			if (component == null) {
				throw new ArgumentNullException("component");
			}
			base.Initialize(component);
			baseLine = (BaseLineItem)component;
		}
		
		
		protected override void PostFilterProperties(System.Collections.IDictionary properties){
			TypeProviderHelper.RemoveProperties(properties);
			base.PostFilterProperties(properties);
		}
		
		
		protected override void OnPaintAdornments(PaintEventArgs pe){
			var label = Control as BaseLineItem;
			
			if (SelectionService != null)
			{
				if (SelectionService.GetComponentSelected(label))
				{
					// Paint grab handles.
					var grapRectangle = GetHandle(label.FromPoint);
					ControlPaint.DrawGrabHandle(pe.Graphics, grapRectangle, true, true);
					grapRectangle = GetHandle(label.ToPoint);
					ControlPaint.DrawGrabHandle(pe.Graphics, grapRectangle, true, true);
					
				}
			}
		}
	
		
		static Rectangle GetHandle(Point pt){
			var handle = new Rectangle(pt, new Size(7, 7));
			handle.Offset(-3, -3);
			return handle;
		}
		
		
		protected override void OnSetCursor(){
			// Get mouse cursor position relative to
			// the control's coordinate space.
			
			var label = (BaseLineItem) Control;
			var p = label.PointToClient(Cursor.Position);
			
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
		
		protected override void OnMouseDragBegin(int x, int y){
			var point = this.baseLine.PointToClient(new Point(x, y));
			overFromPoint = GetHandle(baseLine.FromPoint).Contains(point);
			overToPoint = GetHandle(baseLine.ToPoint).Contains(point);
			if (overFromPoint || overToPoint )
			{
				dragging = true;
			}
			else
			{
				dragging = false;
				base.OnMouseDragBegin(x, y);
			}
		}
		
		
		protected override void OnMouseDragMove(int x, int y){
			if (dragging)
			{
				Point p = baseLine.PointToClient(new Point(x, y));
				if (overToPoint) {
					baseLine.ToPoint = p;
				} else {
					baseLine.FromPoint = p;
				}
				
//				this.baseLine.Invalidate();
//				this.dragOffset = p;
			}
			else
			{
				base.OnMouseDragMove(x, y);
			}
		}
		
		
		protected override void OnMouseDragEnd(bool cancel){
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
				baseLine.Invalidate();
			}
			
			// Always call base class.
			base.OnMouseDragEnd(cancel);

		}
		
		#endregion	
	}
}
