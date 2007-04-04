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

using Tools.Diagrams;
using Tools.Diagrams.Drawables;

namespace ClassDiagram
{
	public class InteractiveItemsStack : DrawableItemsStack<IDrawableRectangle>, IMouseInteractable
	{
		public InteractiveItemsStack() {}
		
		public InteractiveItemsStack(bool recursive)
		{
			this.recursive = recursive;
		}
		
		bool recursive = true;
		
		public bool Recursive {
			get { return recursive; }
			set { recursive = value; }
		}
		
		public void HandleMouseClick(PointF pos)
		{
			if (!recursive) return;
			foreach (IMouseInteractable mi in this)
				mi.HandleMouseClick(pos);
		}
		
		public void HandleMouseDown(PointF pos)
		{
			if (!recursive) return;
			foreach (IMouseInteractable mi in this)
				mi.HandleMouseDown(pos);
		}
		
		public void HandleMouseMove(PointF pos)
		{
			if (!recursive) return;
			foreach (IMouseInteractable mi in this)
				mi.HandleMouseMove(pos);
		}
		
		public void HandleMouseUp(PointF pos)
		{
			if (!recursive) return;
			foreach (IMouseInteractable mi in this)
				mi.HandleMouseUp(pos);
		}
		
		public void HandleMouseLeave()
		{
			if (!recursive) return;
			foreach (IMouseInteractable mi in this)
				mi.HandleMouseLeave();
		}
	}
}
