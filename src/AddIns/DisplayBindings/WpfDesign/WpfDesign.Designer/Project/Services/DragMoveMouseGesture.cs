// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
