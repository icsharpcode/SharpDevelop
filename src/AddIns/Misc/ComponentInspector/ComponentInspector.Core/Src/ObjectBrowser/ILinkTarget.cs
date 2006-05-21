// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace NoGoop.ObjBrowser
{

	// Used when something is capable of being the target
	// of a link
	internal interface ILinkTarget
	{

		// The name to be displayed for the link
		String GetLinkName(Object linkModifier);

		// To show the target when the link is clicked
		void ShowTarget(Object linkModifier);
	}

}
