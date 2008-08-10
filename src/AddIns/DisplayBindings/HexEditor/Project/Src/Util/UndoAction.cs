// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace HexEditor.Util
{
	/// <summary>
	/// Used to tell the UndoManager what to do with the changes.
	/// </summary>
	public enum UndoAction
	{
		Insert,
		Remove,
		Overwrite
	}
}
