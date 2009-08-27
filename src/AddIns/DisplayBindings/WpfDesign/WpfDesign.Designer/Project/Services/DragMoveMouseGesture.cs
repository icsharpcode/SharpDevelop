// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

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
		MoveLogic moveLogic;
		
		internal DragMoveMouseGesture(DesignItem clickedOn, bool isDoubleClick)
		{
			Debug.Assert(clickedOn != null);
			
			this.isDoubleClick = isDoubleClick;
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
			if (!hasDragStarted && isDoubleClick) {
				// user made a double-click
				Debug.Assert(moveLogic.Operation == null);
				moveLogic.HandleDoubleClick();
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
