// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace NoGoop.ObjBrowser
{
	internal interface IConvertableTreeNode : IMenuTreeNode
	{
		void DoConvert();
		void DoRegister();
		void DoUnregister();
	}
}
