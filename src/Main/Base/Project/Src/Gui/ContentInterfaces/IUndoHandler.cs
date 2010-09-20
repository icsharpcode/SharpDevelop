// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// This interface is meant for Windows-Forms AddIns to preserve the undo handling functionality as in SharpDevelop 3.0.
	/// It works only for controls inside a <see cref="SDWindowsFormsHost"/>.
	/// WPF AddIns should handle the routed commands 'Undo' and 'Redo' instead.
	/// </summary>
	public interface IUndoHandler
	{
		bool EnableUndo {
			get;
		}
		
		bool EnableRedo {
			get;
		}
		
		void Undo();
		void Redo();
	}
}
