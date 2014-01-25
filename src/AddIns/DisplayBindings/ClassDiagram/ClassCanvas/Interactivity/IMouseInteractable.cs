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
