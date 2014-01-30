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
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace ICSharpCode.WpfDesign.Designer.Services
{
	/// <summary>
	/// Mouse gesture for moving elements inside a container or between containers.
	/// Belongs to the PointerTool.
	/// </summary>
	sealed class DragMoveMouseGesture : ClickOrDragMouseGesture
	{
		bool isDoubleClick;
		bool setSelectionIfNotMoving;
		MoveLogic moveLogic;
		
		internal DragMoveMouseGesture(DesignItem clickedOn, bool isDoubleClick, bool setSelectionIfNotMoving = false)
		{
			Debug.Assert(clickedOn != null);
			
			this.isDoubleClick = isDoubleClick;
			this.setSelectionIfNotMoving = setSelectionIfNotMoving;
			this.positionRelativeTo = clickedOn.Services.DesignPanel;

			moveLogic = new MoveLogic(clickedOn);
		}
		
		protected override void OnDragStarted(MouseEventArgs e)
		{
			moveLogic.Start(startPoint);
		}
		
		protected override void OnMouseMove(object sender, MouseEventArgs e)
		{
			base.OnMouseMove(sender, e); // call OnDragStarted if min. drag distace is reached
			moveLogic.Move(e.GetPosition(positionRelativeTo));
		}
		
		protected override void OnMouseUp(object sender, MouseButtonEventArgs e)
		{
			if (!hasDragStarted) {
				if (isDoubleClick) {
					// user made a double-click
					Debug.Assert(moveLogic.Operation == null);
					moveLogic.HandleDoubleClick();
				} else if (setSelectionIfNotMoving) {
					services.Selection.SetSelectedComponents(new DesignItem[] { moveLogic.ClickedOn }, SelectionTypes.Auto);
				}
			}
			moveLogic.Stop();
			Stop();
		}
		
		protected override void OnStopped()
		{
			moveLogic.Cancel();
		}
	}
}
