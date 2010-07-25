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

namespace NoGoop.ObjBrowser
{

	internal interface ITypeTreeHandler
	{
        TypeHandlerManager.TypeHandlerInfo Info { get; }

        bool Enabled { get; }

        // Returns true if the data associated with this type handler
        // is unchanged since the last time IsCurrent() was called.
        // This is used to determine whether or not to invalidate
        // the object tree if the underlying data changed.
        bool IsCurrent();

        // Returns a collection of ObjectInfo nodes
        ICollection GetChildren();

        BrowserTreeNode AllocateChildNode(ObjectInfo objInfo);

        bool HasChildren();

	}

}
