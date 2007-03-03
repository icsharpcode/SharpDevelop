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
	/// Behavior interface implemented by container elements to support resizing
	/// child elements.
	/// </summary>
	public interface IPlacementBehavior
	{
		/// <summary>
		/// Gets if the child element can be resized.
		/// </summary>
		bool CanPlace(DesignItem child, PlacementType type, PlacementAlignment position);
		
		/// <summary>
		/// Starts placement mode of the child element specified in the placement operation.
		/// </summary>
		void StartPlacement(PlacementOperation operation);
		
		/// <summary>
		/// Updates the placement of the element specified in the placement operation.
		/// </summary>
		void UpdatePlacement(PlacementOperation operation);
		
		/// <summary>
		/// Gets if leaving this container is allowed for the specified operation.
		/// </summary>
		bool CanLeaveContainer(PlacementOperation operation);
		
		/// <summary>
		/// Remove the placed child from this container.
		/// </summary>
		void LeaveContainer(PlacementOperation operation);
		
		/// <summary>
		/// Gets if entering this container is allowed for the specified operation.
		/// </summary>
		bool CanEnterContainer(PlacementOperation operation);
		
		/// <summary>
		/// Let the placed child enter this container.
		/// </summary>
		void EnterContainer(PlacementOperation operation);
		
		/// <summary>
		/// Finishes placement of a child element. This method is called both for aborted
		/// and committed placement operations.
		/// </summary>
		void FinishPlacement(PlacementOperation operation);
	}
	
	/// <summary>
	/// Behavior interface for root elements (elements where item.Parent is null).
	/// Is used instead of <see cref="IPlacementBehavior"/> to support resizing the root element.
	/// </summary>
	public interface IRootPlacementBehavior : IPlacementBehavior
	{
	}
}
