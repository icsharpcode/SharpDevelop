// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using NoGoop.Obj;
using NoGoop.ObjBrowser.TreeNodes;

namespace NoGoop.ObjBrowser.Types
{
	internal class BaseTypeHandler : ITypeTreeHandler
	{
		protected TypeHandlerManager.TypeHandlerInfo    _info;
		protected ObjectTreeNode                        _node;

		public TypeHandlerManager.TypeHandlerInfo Info {
			get {
				return _info;
			}
		}

		public bool Enabled {
			get {
				return _info.Enabled;
			}
		}

		internal BaseTypeHandler(TypeHandlerManager.TypeHandlerInfo info, ObjectTreeNode node)
		{
			_info = info;
			_node = node;
		}

		public virtual bool IsCurrent()
		{
			return false;
		}

		public virtual ICollection GetChildren()
		{
			throw new Exception("Must override me");

		}

		public virtual bool HasChildren()
		{
			throw new Exception("Must override me");

		}

		public virtual BrowserTreeNode AllocateChildNode(ObjectInfo objInfo)
		{
			throw new Exception("Must override me");
		}
	}
}
