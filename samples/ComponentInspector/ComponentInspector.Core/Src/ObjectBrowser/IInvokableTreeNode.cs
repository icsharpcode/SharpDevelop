// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace NoGoop.ObjBrowser
{

	internal interface IInvokableTreeNode : IMenuTreeNode
	{

        void Invoke(bool setMember, 
                    bool autoInvoke,
                    bool ignoreException);

        bool IsAutoInvoked(bool ignoreException);
	}

}
