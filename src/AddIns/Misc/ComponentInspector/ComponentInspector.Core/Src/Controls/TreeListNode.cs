// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Windows.Forms;

namespace NoGoop.Controls
{
	internal class TreeListNode : TreeNode, IComparable
	{
		// This replaces the Text member; this is used for multi-column
		// trees
		protected ArrayList         _columnData;

		internal TreeListNode() : base()
		{
			_columnData = new ArrayList();
		}

		public ArrayList ColumnData {
			get {
				return _columnData;
			}
		}

		// Basic comparison, alphabetical
		public virtual int CompareTo(Object other)
		{
			if (other is TreeListNode)
			{
				return ((TreeListNode)other).
					Text.CompareTo(Text);
			}
			return -1;
		}
	}
}
