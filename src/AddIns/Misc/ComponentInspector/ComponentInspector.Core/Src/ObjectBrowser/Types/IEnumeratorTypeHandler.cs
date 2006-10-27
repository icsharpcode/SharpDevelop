// <file>
//	 <copyright see="prj:///doc/copyright.txt"/>
//	 <license see="prj:///doc/license.txt"/>
//	 <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//	 <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using NoGoop.Obj;
using NoGoop.ObjBrowser.TreeNodes;
using NoGoop.Util;

namespace NoGoop.ObjBrowser.Types
{
	// Represents a type that returns an IEnumerator
	internal class IEnumeratorTypeHandler : BaseTypeHandler
	{

		protected ArrayList		 _previousValues;

		internal IEnumeratorTypeHandler(TypeHandlerManager.TypeHandlerInfo info, ObjectTreeNode node) : base(info, node)
		{
		}

		public override bool IsCurrent()
		{
			ArrayList currentValues = (ArrayList)GetChildren();

			bool current = Utils.IsArrayListEqual(_previousValues, currentValues);

			_previousValues = currentValues;
			return current;
		}

		// Returns ObjectInfo objects for the children
		public override ICollection GetChildren()
		{
			return GetChildren((IEnumerator)_node.ObjectInfo.Obj);
		}
		
		// Returns ObjectInfo objects for the children
		protected ICollection GetChildren(IEnumerator e)
		{
			ArrayList retVal = new ArrayList();

			if (e != null) {
				e.Reset();
				while (e.MoveNext()) {
					if (e.Current == null)
						continue;
					ObjectInfo newObjInfo = ObjectInfoFactory.GetObjectInfo(_node.IsComNode, e.Current);
					retVal.Add(newObjInfo);
				}
			}
			return retVal;

		}

		public override bool HasChildren()
		{
			if (_node.ObjectInfo.Obj != null) {
				IEnumerator e = (IEnumerator)_node.ObjectInfo.Obj;
				e.Reset();
				if (e.MoveNext())
					return true;
			}
			return false;
		}

		// Allocates the correct type of node
		// Expects an ObjectInfo object
		public override BrowserTreeNode AllocateChildNode(ObjectInfo objInfo)
		{
			return new ObjectTreeNode(_node.IsComNode, objInfo);
		}
	}
}
