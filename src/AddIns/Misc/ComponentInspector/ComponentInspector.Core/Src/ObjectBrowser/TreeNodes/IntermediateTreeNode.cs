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

	// An intermediate node presents categories for child nodes in
	// a tree, and are not visible in logical parent/child hierarchy of
	// the browser nodes
	internal class IntermediateTreeNode : BrowserTreeNode
	{

		protected IntermediateNodeType  _nodeType;


		internal IntermediateNodeType NodeType
		{
			get
				{
					return _nodeType;
				}
		}



		internal IntermediateTreeNode(IntermediateNodeType nodeType,
									  BrowserTreeNode logicalParent) : base()
		{
			_nodeType = nodeType;
			_logicalParent = logicalParent;

			// These are always setup with their children added
			_childrenAdded = true;

			PostConstructor();
		}

		// We use the physical nodes since we are an intermediate.
		// The logical nodes below us will always be present, as will
		// any interveaning physical nodes
		public override ICollection GetSearchChildren()
		{
			return Nodes;
		}
		public override bool HasSearchChildren(ISearcher searcher)
		{
			return Nodes.Count > 0;
		}
		

		public override void Select()
		{
			// Go the non-intermediate parent and tell them an
			// intermediate child was selected, so it can update the
			// detail panel or something
			BrowserTreeNode parent = (BrowserTreeNode)Parent;
			while (parent is IntermediateTreeNode)
				parent = (BrowserTreeNode)parent.Parent;
			parent.IntermediateChildSelect();
			base.Select();
		}


		public override String GetName()
		{
			return _nodeType.Name;
		}

		public override void GetDetailText()
		{
			base.GetDetailText();
		}

	}

}
