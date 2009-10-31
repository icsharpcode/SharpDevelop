// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com"/>
//     <version>$Revision$</version>
// </file>

#include "global.h"

WCHAR *consoleGroupList[CONSOLE_GROUP_LENGTH] = {
	L"System.Console.Write",
	L"System.Console.WriteLine"
};

WCHAR *winFormsGroupList[WINFORMS_GROUP_LENGTH] = {
	L"System.Windows.Forms.Control.OnClick",
	L"System.Windows.Forms.Control.OnDoubleClick",
	L"System.Windows.Forms.Control.OnMouseWheel",
	L"System.Windows.Forms.Control.OnKeyDown"
};

WCHAR *wpfGroupList[WPF_GROUP_LENGTH] = {
	L"System.Windows.UIElement.OnLeftMouseButtonDown"
};