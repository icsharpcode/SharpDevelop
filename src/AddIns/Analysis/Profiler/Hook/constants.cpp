// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#include "global.h"

WCHAR *assemblyInjectionNameList[ASSEMBLY_INJECTION_NAME_LIST_LENGTH] = {
	L"mscorlib",
	L"System.Windows.Forms",
	L"PresentationCore"
};

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