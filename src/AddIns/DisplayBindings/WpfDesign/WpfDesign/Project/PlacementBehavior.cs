// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows;

namespace ICSharpCode.WpfDesign
{
	/// <summary>
	/// Behavior interface implemented by container elements to support resizing
	/// child elements.
	/// </summary>
	public interface IPlacementBehavior
	{
		/// <summary>
		/// Gets if the child element can be resized.
		/// </summary>
		bool CanPlace(ICollection<DesignItem> childItems, PlacementType type, PlacementAlignment position);
		
		/// <summary>
		/// Starts placement mode for this container.
		/// </summary>
		void BeginPlacement(PlacementOperation operation);
		
		/// <summary>
		/// Ends placement mode for this container.
		/// </summary>
		void EndPlacement(PlacementOperation operation);
		
		/// <summary>
		/// Gets the original position of the child item.
		/// </summary>
		Rect GetPosition(PlacementOperation operation, DesignItem child);
		
		/// <summary>
		/// Updates the placement of the element specified in the placement operation.
		/// </summary>
		void SetPosition(PlacementInformation info);
		
		/// <summary>
		/// Gets if leaving this container is allowed for the specified operation.
		/// </summary>
		bool CanLeaveContainer(PlacementOperation operation);
		
		/// <summary>
		/// Remove the placed children from this container.
		/// </summary>
		void LeaveContainer(PlacementOperation operation);
		
		/// <summary>
		/// Gets if entering this container is allowed for the specified operation.
		/// </summary>
		bool CanEnterContainer(PlacementOperation operation);
		
		/// <summary>
		/// Let the placed children enter this container.
		/// </summary>
		void EnterContainer(PlacementOperation operation);
	}
	
	/// <summary>
	/// Behavior interface for root elements (elements where item.Parent is null).
	/// Is used instead of <see cref="IPlacementBehavior"/> to support resizing the root element.
	/// </summary>
	public interface IRootPlacementBehavior : IPlacementBehavior
	{
	}
}
