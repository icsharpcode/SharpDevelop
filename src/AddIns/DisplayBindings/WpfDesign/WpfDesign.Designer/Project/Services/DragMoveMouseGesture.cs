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
		readonly DesignItem clickedOn;
		PlacementOperation operation;
		ICollection<DesignItem> selectedItems;
		bool isDoubleClick;
		
		internal DragMoveMouseGesture(DesignItem clickedOn, bool isDoubleClick)
		{
			Debug.Assert(clickedOn != null);
			
			this.clickedOn = clickedOn;
			this.isDoubleClick = isDoubleClick;
			
			if (clickedOn.Parent != null)
				this.positionRelativeTo = clickedOn.Parent.View;
			else
				this.positionRelativeTo = clickedOn.View;
			
			selectedItems = clickedOn.Services.Selection.SelectedItems;
			if (!selectedItems.Contains(clickedOn))
				selectedItems = SharedInstances.EmptyDesignItemArray;
		}
		
		protected override void OnDragStarted(MouseEventArgs e)
		{
			IPlacementBehavior b = PlacementOperation.GetPlacementBehavior(selectedItems);
			if (b != null && b.CanPlace(selectedItems, PlacementType.Move, PlacementAlignment.TopLeft)) {
				List<DesignItem> sortedSelectedItems = new List<DesignItem>(selectedItems);
				sortedSelectedItems.Sort(ModelTools.ComparePositionInModelFile);
				selectedItems = sortedSelectedItems;
				operation = PlacementOperation.Start(selectedItems, PlacementType.Move);
			}
		}
		
		protected override void OnMouseMove(object sender, MouseEventArgs e)
		{
			base.OnMouseMove(sender, e); // call OnDragStarted if min. drag distace is reached
			if (operation != null) {
				UIElement currentContainer = operation.CurrentContainer.View;
				Point p = e.GetPosition(currentContainer);
				
				// try to switch the container
				if (operation.CurrentContainerBehavior.CanLeaveContainer(operation)) {
					if (ChangeContainerIfPossible(e)) {
						return;
					}
				}
				
				Vector v = e.GetPosition(positionRelativeTo) - startPoint;
				foreach (PlacementInformation info in operation.PlacedItems) {
					info.Bounds = new Rect(info.OriginalBounds.Left + v.X,
					                       info.OriginalBounds.Top + v.Y,
					                       info.OriginalBounds.Width,
					                       info.OriginalBounds.Height);
					operation.CurrentContainerBehavior.SetPosition(info);
				}
			}
		}
		
		// Perform hit testing on the design panel and return the first model that is not selected
		DesignPanelHitTestResult HitTestUnselectedModel(MouseEventArgs e)
		{
			DesignPanelHitTestResult result = DesignPanelHitTestResult.NoHit;
			ISelectionService selection = services.Selection;
			designPanel.HitTest(
				e.GetPosition(designPanel), false, true,
				delegate(DesignPanelHitTestResult r) {
					if (r.ModelHit == null)
						return true; // continue hit testing
					if (selection.IsComponentSelected(r.ModelHit))
						return true; // continue hit testing
					result = r;
					return false; // finish hit testing
				});
			return result;
		}
		
		bool ChangeContainerIfPossible(MouseEventArgs e)
		{
			DesignPanelHitTestResult result = HitTestUnselectedModel(e);
			if (result.ModelHit == null) return false;
			if (result.ModelHit == operation.CurrentContainer) return false;
			
			// check that we don't move an item into itself:
			DesignItem tmp = result.ModelHit;
			while (tmp != null) {
				if (tmp == clickedOn) return false;
				tmp = tmp.Parent;
			}
			
			IPlacementBehavior b = result.ModelHit.GetBehavior<IPlacementBehavior>();
			if (b != null && b.CanEnterContainer(operation)) {
				operation.ChangeContainer(result.ModelHit);
				return true;
			}
			return false;
		}
		
		protected override void OnMouseUp(object sender, MouseButtonEventArgs e)
		{
			if (!hasDragStarted && isDoubleClick) {
				// user made a double-click
				Debug.Assert(operation == null);
				HandleDoubleClick();
			}
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
		
		void HandleDoubleClick()
		{
			if (selectedItems.Count == 1) {
				IEventHandlerService ehs = services.GetService<IEventHandlerService>();
				if (ehs != null) {
					DesignItemProperty defaultEvent = ehs.GetDefaultEvent(clickedOn);
					if (defaultEvent != null) {
						ehs.CreateEventHandler(defaultEvent);
					}
				}
			}
		}
	}
}
