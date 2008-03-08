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
