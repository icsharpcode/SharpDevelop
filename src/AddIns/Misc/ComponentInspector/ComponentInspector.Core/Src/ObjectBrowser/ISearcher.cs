// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;

namespace NoGoop.ObjBrowser
{

    // Represents the object doing the search

	public interface ISearcher
	{

        // The stack of the ISearchNodes, top node is the lowest
        // in the tree
        Stack SearchStack { get; } 

        // Reports the current state of the search, for example,
        // if opening a type library
        void ReportStatus(String action,
                          String obj);
	}

}
