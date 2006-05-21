// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace NoGoop.ObjBrowser
{

	// Used for node that have a target type, the type to be
	// used when creating an object for example.
	internal interface ITargetType
	{
		// The target type
		Type Type { get; }

		// Is this node really a member node and not a type
		// node?
		bool IsMember { get; }
	}

}
