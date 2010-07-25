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

	internal interface IBrowserNode : IDragDropItem
	{

        void AddChildren();

        void AddDummy();
        void RemoveDummy();

        String GetName();
        void GetDetailText();

        // Display the title of the node in the status panel
        void ShowTitle();

        // Cause the children of this node to be recomputed
        void InvalidateNode();

        // Cause the children of this node to be invalidated
        void InvalidateAll();

        // Called when the node is selected
        void Select();

        // Called when an intermediate node that is a child of
        // this node has been selected.
        void IntermediateChildSelect();

        // Called inside of a select if the node is to be invoked.
        // This is called after everything is setup for the
        // invocation, but before the detail panel is setup so
        // that the results of the invocation can be seen.
        void DoSelectInvoke();

        // Called when the node is expanded, return true to
        // cancel the expansion.  Its important that this do nothing
        // with the GUI, as it is used internally to add children
        // so that things can be found
        bool ExpandNode();

        // Ensures that the specified node is visible, selected
        // and has the focus
        void PointToNode();

        // Adds a node as a child of the given node.  If there are
        // intermediate nodes; they are created as needed.
        void AddLogicalNode(IBrowserNode child);

        // Removes the node; allows any processing for removal to be
        // done before the node is actually removed
        void RemoveLogicalNode();

        // The intemediate node types to be used for this node, if any
        ArrayList IntermediateNodeTypes { get; }

        bool Paste(IBrowserNode node, bool isCopy);

	}

}
