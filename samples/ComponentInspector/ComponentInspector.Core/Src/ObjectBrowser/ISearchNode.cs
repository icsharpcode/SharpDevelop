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

    // Used to that classes can report the name they want to be
    // used when being searched
	public interface ISearchNode
	{

        // This returns the value that can be used to point to 
        // the search node.
        ISearchMaterializer GetSearchMaterializer(ISearcher searcher);

        // Get the icon used to represent this node
        int GetImageIndex();

        // Get the name to be considered for the search
        String GetSearchNameString();

        // Get the node's value to be considered for the search
        String GetSearchValueString();

        // Indicates if this node has children to be searched,
        // This is always called before GetSearchChildren, it may
        // be used to set things up before the children are searched
        bool HasSearchChildren(ISearcher searcher);

        // Returns the child nodes if any
        ICollection GetSearchChildren();
	}

}
