// <file>
//	 <copyright see="prj:///doc/copyright.txt"/>
//	 <license see="prj:///doc/license.txt"/>
//	 <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//	 <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using NoGoop.ObjBrowser.TreeNodes;

namespace NoGoop.ObjBrowser.Types
{
	// Represents a type that returns an IList
	internal class IListTypeHandler : IEnumeratorTypeHandler
	{

		internal IListTypeHandler(TypeHandlerManager.TypeHandlerInfo info, ObjectTreeNode node) : base(info, node)
		{
		}

		// Returns ObjectInfo objects for the children
		public override ICollection GetChildren()
		{
			if (_node.ObjectInfo.Obj != null)
				return GetChildren(((IList)_node.ObjectInfo.Obj).GetEnumerator());
			return new ArrayList();
		}


		public override bool HasChildren()
		{
			if (_node.ObjectInfo.Obj != null) {
				IList e = (IList)_node.ObjectInfo.Obj;
				if (e.Count > 0)
					return true;
			}
			return false;
		}

	}

}
