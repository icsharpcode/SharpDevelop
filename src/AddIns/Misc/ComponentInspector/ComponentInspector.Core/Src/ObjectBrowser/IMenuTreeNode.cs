// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace NoGoop.ObjBrowser
{
	// This node changes the context menu
	internal interface IMenuTreeNode : IBrowserNode 
	{
		bool HasGetProp();
		bool HasSetProp();
		bool HasSetField();
		bool HasInvokeMeth();
		bool HasCreateObj();
		bool HasCast();
		bool HasCut();
		bool HasCopy();
		bool HasCopyText0();
		bool HasCopyText1();
		bool HasPaste();
		bool HasDesignSurface();
		bool HasEventLogging();
		bool HasDelete();
		bool HasRename();
		bool HasClose();
		bool HasConvert();
		bool HasRegister();
		bool HasUnregister();
		bool HasRemoveFavorite();
	}
}
