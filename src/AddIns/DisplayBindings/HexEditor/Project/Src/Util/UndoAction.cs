// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
