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
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace ICSharpCode.WpfDesign.Designer.Services
{
	/// <summary>
	/// Base class for mouse gestures that should start dragging only after a minimum drag distance.
	/// </summary>
	public abstract class ClickOrDragMouseGesture : MouseGestureBase
	{
		protected Point startPoint;
		protected bool hasDragStarted;
		protected IInputElement positionRelativeTo;
		
		const double MinimumDragDistance = 3;
		
		protected sealed override void OnStarted(MouseButtonEventArgs e)
		{
			Debug.Assert(positionRelativeTo != null);
			hasDragStarted = false;
			startPoint = e.GetPosition(positionRelativeTo);
		}
		
		protected override void OnMouseMove(object sender, MouseEventArgs e)
		{
			if (!hasDragStarted) {
				Vector v = e.GetPosition(positionRelativeTo) - startPoint;
				if (Math.Abs(v.X) >= SystemParameters.MinimumHorizontalDragDistance
				    || Math.Abs(v.Y) >= SystemParameters.MinimumVerticalDragDistance) {
					hasDragStarted = true;
					OnDragStarted(e);
				}
			}
		}
		
		protected override void OnStopped()
		{
			hasDragStarted = false;
		}
		
		protected virtual void OnDragStarted(MouseEventArgs e) {}
	}
}
