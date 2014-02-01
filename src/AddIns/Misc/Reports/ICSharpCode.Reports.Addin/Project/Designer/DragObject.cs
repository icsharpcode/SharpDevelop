// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;

namespace ICSharpCode.Reports.Addin.Designer
{
	public class DragObject
	{
		private ReportRootDesigner m_designer;
		private Control m_dragControl;
		private ArrayList m_dragShapes;
		private object m_hitObject;
		private int m_initialX;
		private int m_initialY;
		private Point m_screenOffset;
		private Point m_mouseOffset;
		private bool m_removeOldDrag;

		public DragObject(ReportRootDesigner designer, Control dragControl, object hitTestObject, int initialX, int initialY)
		{
			m_designer = designer;
			m_dragControl = dragControl;
			m_hitObject = hitTestObject;
			m_initialX = initialX;
			m_initialY = initialY;
			m_mouseOffset = new Point(0, 0);
			m_screenOffset = new Point(0, 0);
			m_screenOffset = dragControl.PointToScreen(m_screenOffset);

			// The drag actually consists of all objects that are currently selected.
			//
			ISelectionService ss = designer.SelectionService;
			IDesignerHost host = designer.Host;

			m_dragShapes = new ArrayList();

			if (ss != null && host != null)
			{
				ICollection selectedObjects = ss.GetSelectedComponents();
				foreach(object o in selectedObjects)
				{
					IComponent comp = o as IComponent;
					if (comp != null)
					{
						ItemDesigner des = host.GetDesigner(comp) as ItemDesigner;
						if (des != null)
						{
							m_dragShapes.Add(des);
						}
					}
				}
			}
		}

		/// <summary>
		/// This is the control that initiated the drag.
		/// </summary>
		public Control DragControl
		{
			get
			{
				return m_dragControl;
			}
		}

		/// <summary>
		/// Called to clear any feedback drawing we have already done.
		/// </summary>
		public void ClearFeedback()
		{
			if (m_removeOldDrag)
			{
				m_removeOldDrag = false;
				foreach(ItemDesigner sd in m_dragShapes)
				{
					sd.DrawDragFeedback(m_hitObject, m_dragControl.BackColor, m_screenOffset, m_mouseOffset);
				}
			}
		}

		/// <summary>
		/// This performs the actual drop, which just moves all of our components
		/// to the drop location.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public void Drop(int x, int y)
		{
			System.Console.WriteLine("\tDragObject:Drop");
			ClearFeedback();
			Point pt = new Point(x, y);
			pt = m_dragControl.PointToClient(pt);
			x = pt.X - m_initialX;
			y = pt.Y - m_initialY;

			// If we have more than one shape, we need to wrap
			// in a designer transaction.
			//
			if (m_dragShapes.Count > 1)
			{
				IDesignerHost host = this.m_designer.Host;
				if (host != null)
				{
					using(DesignerTransaction trans = host.CreateTransaction("Drag " + m_dragShapes.Count + " components"))
					{
						foreach(ItemDesigner sd in m_dragShapes)
						{
							sd.Drag(m_hitObject, x, y);
						}

						trans.Commit();
					}
				}
			}
			else
			{
				foreach(ItemDesigner sd in m_dragShapes)
				{
					sd.Drag(m_hitObject, x, y);
				}
			}
		}

		/// <summary>
		/// Called during dragging so we can update our feedback.
		/// </summary>
		public void GiveFeedback()
		{
			// First, if we had prior feedback, clear it.
			ClearFeedback();

			// Now, draw new feedback.  The feedback coordinates
			// are global and designed in such a way so that they
			// can be added to the shape coordinates directly.
			// To do this we take the global mouse position and
			// subtract off this position in client coordinates
			// of the drag control.
			//
			Point currentPosition = Control.MousePosition;
			m_mouseOffset.X = currentPosition.X - m_screenOffset.X - m_initialX;
			m_mouseOffset.Y = currentPosition.Y - m_screenOffset.Y - m_initialY;

			foreach(ItemDesigner sd in m_dragShapes)
			{
				sd.DrawDragFeedback(m_hitObject, m_dragControl.BackColor, m_screenOffset, m_mouseOffset);
			}

			m_removeOldDrag = true;
		}
	}
}
