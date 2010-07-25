// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;

namespace NoGoop.ObjBrowser
{
	internal interface IDragDropSourceTarget
	{
		Point PointToClient(Point point);
		IDragDropItem GetItemAt(Point point);
		// An array of type types of nodes that this drag target can
		// accept.  This is used to get the node being dragged.
		Type[] DragSourceTypes { get; }
	}
}
