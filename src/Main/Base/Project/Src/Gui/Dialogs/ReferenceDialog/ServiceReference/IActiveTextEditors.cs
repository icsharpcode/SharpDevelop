// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.ReferenceDialog.ServiceReference
{
	public interface IActiveTextEditors
	{
		string GetTextForOpenFile(string fileName);
		void UpdateTextForOpenFile(string fileName, string text);
		bool IsFileOpen(string fileName);
	}
}
