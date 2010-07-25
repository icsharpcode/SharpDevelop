// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>


using System;
using System.Drawing;
using System.Windows.Forms;

using NoGoop.ObjBrowser.GuiDesigner;
using NoGoop.Util;

namespace NoGoop.ObjBrowser
{
	public class DragDropSupport
	{
		// The control associated this this handler
		IDragDropSourceTarget       _control;
		
		internal DragDropSupport(IDragDropSourceTarget control)
		{
			_control = control;

			if (_control is TreeView) {
				((TreeView)_control).ItemDrag += new ItemDragEventHandler(ItemDragEvent);
			}

			((Control)_control).DragEnter += new DragEventHandler(DragEnterEvent);
			((Control)_control).DragDrop += new DragEventHandler(DragDropEvent);
			((Control)_control).DragOver += new DragEventHandler(DragOverEvent);
		}

		// Used to start a drag
		protected void ItemDragEvent(object sender, ItemDragEventArgs e)
		{
			try {
				if (e.Item is IDragDropItem) {
					IDragDropItem node = (IDragDropItem)e.Item;
					node.SelectThisItem();

					if (node.IsDragSource) {
						// Not too elegant, but we might be dragging
						// controls to the design surface
						DesignerHost.Host.AddingControls = true;
						((Control)_control).DoDragDrop(node, DragDropEffects.Copy);
						DesignerHost.Host.AddingControls = false;
					}
				}
			} catch (Exception ex) {
				TraceUtil.WriteLineWarning(this, "ItemDragEvent exception: " + ex);
			}
		}

		protected IDropTarget FindAncestorDropControl(DragEventArgs e)
		{
			Control con = ((Control)_control).Parent;
			
			while (con != null)	{
				if (con is IDropTarget && ((IDropTarget)con).CanDrop(e)) {
					return (IDropTarget)con;
				}
				con = con.Parent;
			}
			return null;
		}

		internal IDragDropItem DragEventCommon(object sender, DragEventArgs e, ref IDragDropItem targetNode)
		{
			Object dragControl = ((IDragDropSourceTarget)_control).GetItemAt(_control.PointToClient(new Point(e.X, e.Y)));
			if (dragControl is IDragDropItem) {
				IDragDropItem node = (IDragDropItem)dragControl;
				if (node.IsDropTarget) {
					foreach (Type t in _control.DragSourceTypes) {
						targetNode = (IDragDropItem)e.Data.GetData(t);
						if (targetNode != null) {
							break;
						}
					}

					if (targetNode != null) {
						e.Effect = DragDropEffects.Copy;
						return node;
					} else {
						return null;
					}
			   }
			}
			e.Effect = DragDropEffects.None;
			return null;
		}

		protected void DragEnterEvent(object sender, DragEventArgs e)
		{
			try {
				IDragDropItem targetNode = null;
				IDragDropItem node = DragEventCommon(sender, e, ref targetNode);

				// If we are not handling this, see of there is an
				// ancestor control that's interested
				if (e.Effect == DragDropEffects.None) {
					IDropTarget con = FindAncestorDropControl(e);
					if (con != null)
						con.DragEnterEvent(sender, e);
				}
			} catch (Exception ex) {
				TraceUtil.WriteLineWarning(this, "Exception in event handler: " + ex);
			}
		}

		protected void DragOverEvent(object sender, DragEventArgs e)
		{
			try {
				IDragDropItem targetNode = null;
				IDragDropItem node = 
					DragEventCommon(sender, e, ref targetNode);

				// If we are not handling this, see of there is an
				// ancestor control that's interested
				if (e.Effect == DragDropEffects.None)
				{
					IDropTarget con = FindAncestorDropControl(e);
					if (con != null)
						con.DragOverEvent(sender, e);
				}
			} catch (Exception ex) {
				TraceUtil.WriteLineWarning(this, "Exception in event handler: " + ex);
			}
		}

		protected void DragDropEvent(object sender, DragEventArgs e)
		{
			try {
				IDragDropItem targetNode = null;
				IDragDropItem node = 
					DragEventCommon(sender, e, ref targetNode);

				if (node != null) {
					Cursor save = Cursor.Current;
					Cursor.Current = Cursors.WaitCursor;
					node.DoDrop(targetNode);
					Cursor.Current = save;
				} else {
					// If we are not handling this, see of there is an
					// ancestor control that's interested
					IDropTarget con = FindAncestorDropControl(e);
					if (con != null)
						con.DragDropEvent(sender, e);
				}
			} catch (Exception ex) {
				TraceUtil.WriteLineWarning(this, "Exception in event handler: " + ex);
			}
		}
	}
}

