// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace NoGoop.ObjBrowser
{

    // Used to that classes can report the name they want to be
    // used when being searched
	public interface ISearchMaterializer
	{
        // Make the node appear
        void PointToNode();

	}

}
