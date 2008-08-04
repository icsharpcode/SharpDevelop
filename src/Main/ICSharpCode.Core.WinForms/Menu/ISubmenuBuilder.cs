// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;

namespace ICSharpCode.Core.WinForms
{
	public interface ISubmenuBuilder
	{
		ToolStripItem[] BuildSubmenu(Codon codon, object owner);
	}
}
