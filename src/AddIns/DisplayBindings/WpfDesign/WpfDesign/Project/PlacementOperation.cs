// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows;
using System.Windows.Input;

using ICSharpCode.WpfDesign.Adorners;

namespace ICSharpCode.WpfDesign
{
	/// <summary>
	/// Stores data about a placement operation.
	/// </summary>
	public sealed class PlacementOperation
	{
		readonly ChangeGroup changeGroup;
		readonly DesignItem placedItem;
		readonly PlacementType type;
		readonly DesignItem oldContainer;
		readonly IPlacementBehavior oldContainerBehavior;
		DesignItem currentContainer;
		IPlacementBehavior currentContainerBehavior;
		bool supportsRemoveFromContainer;
		bool isAborted, isCommitted;
		
		//DesignItem newContainer;
		
		#region Properties
		/// <summary>
		/// The item being placed.
		/// </summary>
		public DesignItem PlacedItem {
			get { return placedItem; }
		}
		
		/// <summary>
		/// The type of the placement being done.
		/// </summary>
		public PlacementType Type {
			get { return type; }
		}
		
		/// <summary>
		/// Gets if removing the placed item from the container is supported.
		/// </summary>
		public bool SupportsRemoveFromContainer {
			get { return supportsRemoveFromContainer; }
		}
		
		/// <summary>
		/// Gets if the placement operation was aborted.
		/// </summary>
		public bool IsAborted {
			get { return isAborted; }
		}
		
		/// <summary>
		/// Gets if the placement operation was committed.
		/// </summary>
		public bool IsCommitted {
			get { return isCommitted; }
		}
		
		/// <summary>
		/// The position of the left/right/top/bottom side in the coordinate system of the parent container.
		/// These values must be set by IPlacementBehavior.StartPlacement and are updated by the drag operation.
		/// </summary>
		public double Left, Right, Top, Bottom;
		
		#endregion
		
		public void UpdatePlacement()
		{
			currentContainerBehavior.UpdatePlacement(this);
		}
		
		#region Start
		/// <summary>
		/// Starts a new placement operation that changes the placement of <paramref name="placedItem"/>.
		/// </summary>
		/// <param name="placedItem">The item to be placed.</param>
		/// <param name="type">The type of the placement.</param>
		/// <returns>A PlacementOperation object.</returns>
		/// <remarks>
		/// You MUST call either <see cref="Abort"/> or <see cref="Commit"/> on the returned PlacementOperation
		/// once you are done with it, otherwise a ChangeGroup will be left open and Undo/Redo will fail to work!
		/// </remarks>
		public static PlacementOperation Start(DesignItem placedItem, PlacementType type)
		{
			if (placedItem == null)
				throw new ArgumentNullException("placedItem");
			if (type == null)
				throw new ArgumentNullException("type");
			PlacementOperation op = new PlacementOperation(placedItem, type);
			try {
				if (op.currentContainerBehavior == null)
					throw new InvalidOperationException("Starting the operation is not supported");
				op.Left = op.Top = op.Bottom = op.Right = double.NaN;
				op.currentContainerBehavior.StartPlacement(op, out op.supportsRemoveFromContainer);
				if (double.IsNaN(op.Left) || double.IsNaN(op.Top) || double.IsNaN(op.Bottom) || double.IsNaN(op.Right))
					throw new InvalidOperationException("IPlacementBehavior.StartPlacement must set Left,Top,Right+Bottom to non-NAN values");
			} catch {
				op.changeGroup.Abort();
				throw;
			}
			return op;
		}
		private PlacementOperation(DesignItem placedItem, PlacementType type)
		{
			this.placedItem = placedItem;
			this.type = type;
			
			this.oldContainer = placedItem.Parent;
			this.oldContainerBehavior = GetPlacementBehavior(placedItem);
			
			this.currentContainer = oldContainer;
			this.currentContainerBehavior = oldContainerBehavior;
			this.changeGroup = placedItem.OpenGroup(type.ToString());
		}
		
		/// <summary>
		/// Gets the placement behavior associated with the specified item.
		/// </summary>
		public static IPlacementBehavior GetPlacementBehavior(DesignItem item)
		{
			if (item == null)
				throw new ArgumentNullException("item");
			if (item.Parent != null) {
				return item.Parent.GetBehavior<IPlacementBehavior>();
			} else {
				return item.GetBehavior<IRootPlacementBehavior>();
			}
		}
		#endregion
		
		#region ChangeGroup handling
		/// <summary>
		/// Gets/Sets the description of the underlying change group.
		/// </summary>
		public string Description {
			get { return changeGroup.Title; }
			set { changeGroup.Title = value; }
		}
		
		/// <summary>
		/// Aborts the operation.
		/// This aborts the underlying change group, reverting all changes done while the operation was running.
		/// </summary>
		public void Abort()
		{
			if (!isAborted) {
				if (isCommitted)
					throw new InvalidOperationException("PlacementOperation is committed.");
				isAborted = true;
				currentContainerBehavior.FinishPlacement(this);
				changeGroup.Abort();
			}
		}
		
		/// <summary>
		/// Commits the operation.
		/// This commits the underlying change group.
		/// </summary>
		public void Commit()
		{
			if (isAborted || isCommitted)
				throw new InvalidOperationException("PlacementOperation is already aborted/committed.");
			isCommitted = true;
			currentContainerBehavior.FinishPlacement(this);
			changeGroup.Commit();
		}
		#endregion
	}
}
