// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace HexEditor.Util
{
	/// <summary>
	/// Used for EventArgs in the ActionUndone and ActionRedone events.
	/// </summary>
	public class UndoEventArgs : EventArgs
	{
		UndoStep undostep;
		bool isRedo;
		
		public UndoEventArgs(UndoStep step, bool redo)
		{
			undostep = step;
			isRedo = redo;
		}
		
		public UndoStep UndoStep {
			get { return undostep; }
		}
		
		public bool IsRedo {
			get { return isRedo; }
		}
	}
}
