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
		bool CanPlace(IEnumerable<DesignItem> childItems, PlacementType type, PlacementAlignment position);
		
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
		/// Is called before SetPosition is called for the placed items.
		/// This may update the bounds on the placement operation (e.g. when snaplines are enabled).
		/// </summary>
		void BeforeSetPosition(PlacementOperation operation);
		
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
		bool CanEnterContainer(PlacementOperation operation, bool shouldAlwaysEnter);
		
		/// <summary>
		/// Let the placed children enter this container.
		/// </summary>
		void EnterContainer(PlacementOperation operation);

		/// <summary>
		/// Place Point.
		/// </summary>
		Point PlacePoint(Point point);
	}
	
	/// <summary>
	/// Behavior interface for root elements (elements where item.Parent is null).
	/// Is used instead of <see cref="IPlacementBehavior"/> to support resizing the root element.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces",
	                                                 Justification = "The root component might have both a PlacementBehavior and a RootPlacementBehavior, which must be distinguished by DesignItem.GetBehavior")]
	public interface IRootPlacementBehavior : IPlacementBehavior
	{
	}
}
