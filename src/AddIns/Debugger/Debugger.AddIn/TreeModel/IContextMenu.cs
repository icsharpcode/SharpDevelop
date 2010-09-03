// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System.Windows.Forms;

namespace Debugger.AddIn.TreeModel
{
	public interface IContextMenu
	{
		ContextMenuStrip GetContextMenu();
	}
}
