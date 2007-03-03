// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
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
		DesignItem clickedOn;
		PlacementOperation operation;
		
		internal DragMoveMouseGesture(DesignItem clickedOn)
		{
			Debug.Assert(clickedOn != null);
			
			this.clickedOn = clickedOn;
			
			if (clickedOn.Parent != null)
				this.positionRelativeTo = clickedOn.Parent.View;
			else
				this.positionRelativeTo = clickedOn.View;
		}
		
		double startLeft, startRight, startTop, startBottom;
		
		protected override void OnDragStarted()
		{
			IPlacementBehavior b = PlacementOperation.GetPlacementBehavior(clickedOn);
			if (b != null && b.CanPlace(clickedOn, PlacementType.Move, PlacementAlignments.TopLeft)) {
				operation = PlacementOperation.Start(clickedOn, PlacementType.Move);
				startLeft = operation.Left;
				startRight = operation.Right;
				startTop = operation.Top;
				startBottom = operation.Bottom;
			}
		}
		
		protected override void OnMouseMove(object sender, MouseEventArgs e)
		{
			base.OnMouseMove(sender, e); // call OnDragStarted if min. drag distace is reached
			if (operation != null) {
				Vector v = e.GetPosition(positionRelativeTo) - startPoint;
				operation.Left = startLeft + v.X;
				operation.Right = startRight + v.X;
				operation.Top = startTop + v.Y;
				operation.Bottom = startBottom + v.Y;
				operation.UpdatePlacement();
			}
		}
		
		protected override void OnMouseUp(object sender, MouseButtonEventArgs e)
		{
			if (operation != null) {
				operation.Commit();
				operation = null;
			}
			Stop();
		}
		
		protected override void OnStopped()
		{
			if (operation != null) {
				operation.Abort();
				operation = null;
			}
		}
	}
}
