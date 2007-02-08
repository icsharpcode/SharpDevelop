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

using Tools.Diagrams;

namespace ClassDiagram
{
	public interface IMouseInteractable
	{
		/// <summary>
		/// Called by the canvas when the user clicks inside the item.
		/// </summary>
		/// <remarks>
		/// The given point is relative to the canvas origin.
		/// Subtruct the item's X and Y values to get a position relative to the item's origin.
		/// </remarks>
		/// <param name="pos">
		/// The click position relative to the canvas origin.
		/// </param>
		void HandleMouseClick (PointF pos);
		
		/// <summary>
		/// Called by the canvas when the user presses a mouse button inside the item.
		/// </summary>
		/// <remarks>
		/// The given point is relative to the canvas origin.
		/// Subtruct the item's X and Y values to get a position relative to the item's origin.
		/// </remarks>
		/// <param name="pos">
		/// The mouse button press position relative to the canvas origin.
		/// </param>
		void HandleMouseDown (PointF pos);
		
		/// <summary>
		/// Called by the canvas when the user moves the mouse cursor inside the item.
		/// </summary>
		/// <remarks>
		/// The given point is relative to the canvas origin.
		/// Subtruct the item's X and Y values to get a position relative to the item's origin.
		/// </remarks>
		/// <param name="pos">
		/// The mouse cursor position relative to the canvas origin.
		/// </param>
		void HandleMouseMove (PointF pos);
		
		/// <summary>
		/// Called by the canvas when the user releases a mouse button inside the item.
		/// </summary>
		/// <remarks>
		/// The given point is relative to the canvas origin.
		/// Subtruct the item's X and Y values to get a position relative to the item's origin.
		/// </remarks>
		/// <param name="pos">
		/// The mouse button release position relative to the canvas origin.
		/// </param>
		void HandleMouseUp (PointF pos);
		
		/// <summary>
		/// Called by the canvas whenever the mouse cursor leaves the item (i.e. the HitTest
		/// method returns false after it returned true).
		/// </summary>
		void HandleMouseLeave ();
	}
	
	public delegate void PositionDelegate (object sender, PointF pos);
}
