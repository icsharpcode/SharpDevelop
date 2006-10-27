// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;

namespace NoGoop.ObjBrowser.TreeNodes
{
	internal class ComSearchMaterializer : ISearchMaterializer
	{
        protected Stack             _searchStack;
        internal ComSearchMaterializer(Stack stack)
        {
            _searchStack = stack;
        }
        // ISearchMaterializer interface
        
        // We find the lowest level BrowserTreeNode and assume that
        // under that are BasicInfos that are represented by
        // ComMemberTreeNodes.  For example, a typical
        // case is a ComTypeLibTreeNode will be the first entry.  Under
        // that is a ComClassInfo.  We assume the tree node type
        // that can represent a ComClassInfo is a ComMemberTreeNode,
        // and look through all of the children of the 
        // ComTypeLibTreeNode (as a BrowserTreeNode) to find the
        // matching ComClassInfo (as a BasicInfo), and then point
        // to the found node.
        //
        public virtual void PointToNode()
        {
            // The array of a stack is returned in the order
            // of the pops
            Object[] nodes = _searchStack.ToArray();
            for (int i = nodes.Length - 1; i >= 0; i--)
            {
                // The lowest level BrowserTreeNode
                if (nodes[i] is BrowserTreeNode &&
                    (i == 0 || !(nodes[i - 1] is BrowserTreeNode)))
                {
                    BrowserTreeNode node = (BrowserTreeNode)nodes[i];
                    // Make sure we add the typelib node to the 
                    // recently used section and deal with that node
                    if (node is ComTypeLibTreeNode)
                    {
                        node = ComSupport.AddTypeLib
                            (((ComTypeLibTreeNode)node).TypeLib);
                    }
                    // Assume what's beneath here are ComMemberTreeNodes
                    if (i == 0)
                        ((BrowserTreeNode)node).PointToNode();
                    else
                        Materialize((BrowserTreeNode)node, nodes, --i);
                    return;
                }
            }
        }
        // Points to the next node in the array when materializing
        // a node from a search.  index points to the childNode.
        protected void Materialize(BrowserTreeNode node,
                                   Object[] nodes,
                                   int index)
        {
            node.ExpandNode();
            foreach (ComMemberTreeNode childNode in node.LogicalNodes)
            {
                if (childNode.MemberInfo.Equals(nodes[index]))
                {
                    if (index == 0)
                        childNode.PointToNode();
                    else
                        Materialize(childNode, nodes, --index);
                    return;
                }
            }
        }
	}
}
