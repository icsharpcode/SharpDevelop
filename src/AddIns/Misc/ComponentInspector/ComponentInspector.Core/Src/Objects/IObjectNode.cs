// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace NoGoop.Obj
{

	// This allows a members parent ObjectInfo to be found.  It
	// abstracts just getting the parent from the rest of the
	// object tree stuff so that it can be used at the ObjectInfo
	// layer.
	// This provides a means for the code on the Obj part of
	// the product to access the object tree
	internal interface IObjectNode
	{

		IObjectNode ParentObjectNode { get; }

		ObjectInfo ObjectInfo { get; }

		// Gets the type of the object associated with this node
		// Use this instead of ObjectInfo.ObjType because there 
		// might be a cast on this node
		Type ObjType { get; }

		// Gets the object associated with this node
		Object Obj { get; }

		// The property index values that correspond to the object
		// that is populating this node, this is used only for 
		// ObjectTreeNodes, but its not worth making a separate
		// interface for.
		Object[] CurrentPropIndexValues { get; set; }

		// Display the value of this object, since it might have
		// changed
		void DoDisplayValue();
	}
}
