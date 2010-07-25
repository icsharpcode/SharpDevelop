// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace NoGoop.ObjBrowser
{

	public interface IDragDropItem
	{
		void SelectThisItem();

		// This node can contain objects, it will create
		// objects that get dragged to it
		bool IsObjectContainer { get; set; }

		bool IsDragSource { get; set; }
		bool IsDropTarget { get; set; }

		void DoDrop(IDragDropItem dragNode);

	}

}
