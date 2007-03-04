// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
		readonly ReadOnlyCollection<PlacementInformation> placedItems;
		readonly PlacementType type;
		readonly DesignItem oldContainer;
		readonly IPlacementBehavior oldContainerBehavior;
		DesignItem currentContainer;
		IPlacementBehavior currentContainerBehavior;
		bool isAborted, isCommitted;
		
		#region Properties
		/// <summary>
		/// The items being placed.
		/// </summary>
		public ReadOnlyCollection<PlacementInformation> PlacedItems {
			get { return placedItems; }
		}
		
		/// <summary>
		/// The type of the placement being done.
		/// </summary>
		public PlacementType Type {
			get { return type; }
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
		/// Gets the current container for the placement operation.
		/// </summary>
		public DesignItem CurrentContainer {
			get { return currentContainer; }
		}
		
		/// <summary>
		/// Gets the placement behavior for the current container.
		/// </summary>
		public IPlacementBehavior CurrentContainerBehavior {
			get { return currentContainerBehavior; }
		}
		
		#endregion
		
		#region Container changing
		/// <summary>
		/// Make the placed item switch the container.
		/// This method assumes that you already have checked if changing the container is possible.
		/// </summary>
		public void ChangeContainer(DesignItem newContainer)
		{
			if (newContainer == null)
				throw new ArgumentNullException("newContainer");
			if (currentContainer == newContainer)
				return;
			
			try {
				currentContainerBehavior.LeaveContainer(this);
				
				System.Windows.Media.GeneralTransform transform = currentContainer.View.TransformToVisual(newContainer.View);
				foreach (PlacementInformation info in placedItems) {
					info.OriginalBounds = transform.TransformBounds(info.OriginalBounds);
					info.Bounds = transform.TransformBounds(info.Bounds);
				}
				
				currentContainer = newContainer;
				currentContainerBehavior = newContainer.GetBehavior<IPlacementBehavior>();
				
				Debug.Assert(currentContainerBehavior != null);
				currentContainerBehavior.EnterContainer(this);
			} catch (Exception ex) {
				Debug.WriteLine(ex.ToString());
				Abort();
				throw;
			}
		}
		#endregion
		
		#region Start
		/// <summary>
		/// Starts a new placement operation that changes the placement of <paramref name="placedItem"/>.
		/// </summary>
		/// <param name="placedItems">The items to be placed.</param>
		/// <param name="type">The type of the placement.</param>
		/// <returns>A PlacementOperation object.</returns>
		/// <remarks>
		/// You MUST call either <see cref="Abort"/> or <see cref="Commit"/> on the returned PlacementOperation
		/// once you are done with it, otherwise a ChangeGroup will be left open and Undo/Redo will fail to work!
		/// </remarks>
		public static PlacementOperation Start(ICollection<DesignItem> placedItems, PlacementType type)
		{
			if (placedItems == null)
				throw new ArgumentNullException("placedItems");
			if (type == null)
				throw new ArgumentNullException("type");
			DesignItem[] items = Func.ToArray(placedItems);
			if (items.Length == 0)
				throw new ArgumentException("placedItems.Length must be > 0");
			
			PlacementOperation op = new PlacementOperation(items, type);
			try {
				if (op.currentContainerBehavior == null)
					throw new InvalidOperationException("Starting the operation is not supported");
				
				op.currentContainerBehavior.BeginPlacement(op);
				foreach (PlacementInformation info in op.placedItems) {
					info.OriginalBounds = op.currentContainerBehavior.GetPosition(op, info.Item);
					info.Bounds = info.OriginalBounds;
				}
			} catch {
				op.changeGroup.Abort();
				throw;
			}
			return op;
		}
		private PlacementOperation(DesignItem[] items, PlacementType type)
		{
			PlacementInformation[] information = new PlacementInformation[items.Length];
			for (int i = 0; i < information.Length; i++) {
				information[i] = new PlacementInformation(items[i], this);
			}
			this.placedItems = new ReadOnlyCollection<PlacementInformation>(information);
			this.type = type;
			
			this.oldContainer = items[0].Parent;
			this.oldContainerBehavior = GetPlacementBehavior(items);
			
			this.currentContainer = oldContainer;
			this.currentContainerBehavior = oldContainerBehavior;
			this.changeGroup = items[0].Context.OpenGroup(type.ToString(), items);
		}
		
		/// <summary>
		/// Gets the placement behavior associated with the specified items.
		/// </summary>
		public static IPlacementBehavior GetPlacementBehavior(ICollection<DesignItem> items)
		{
			if (items == null)
				throw new ArgumentNullException("items");
			if (items.Count == 0)
				return null;
			DesignItem parent = Func.First(items).Parent;
			foreach (DesignItem item in Func.Skip(items, 1)) {
				if (item.Parent != parent)
					return null;
			}
			if (parent != null)
				return parent.GetBehavior<IPlacementBehavior>();
			else if (items.Count == 1)
				return Func.First(items).GetBehavior<IRootPlacementBehavior>();
			else
				return null;
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
				currentContainerBehavior.EndPlacement(this);
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
			currentContainerBehavior.EndPlacement(this);
			changeGroup.Commit();
		}
		#endregion
	}
}
