// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;

namespace NoGoop.ObjBrowser
{
	// This is used for a control that is an ancestor to
	// other controls that consume events.  The DragDropSupport
	// class looks for controls implementing this interface
	// in its ancestors to propogate the drop event handling.
	public interface IDropTarget
	{
		bool CanDrop(DragEventArgs e);
		void DragEnterEvent(object sender, 
							DragEventArgs e);
		void DragOverEvent(object sender, 
							DragEventArgs e);
		void DragDropEvent(object sender, 
						  DragEventArgs e);
	}
}
