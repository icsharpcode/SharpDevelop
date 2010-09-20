// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Forms;

namespace ICSharpCode.Core.WinForms
{
	public interface ISubmenuBuilder
	{
		ToolStripItem[] BuildSubmenu(Codon codon, object owner);
	}
}
