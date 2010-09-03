// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// If a IViewContent object is from the type IPositionable it signals
	/// that it's a texteditor which could set the caret to a position inside
	/// a file.
	/// </summary>
	public interface IPositionable
	{
		/// <summary>
		/// Sets the 'caret' to the position pos, where pos.Y is the line (starting from 1).
		/// And pos.X is the column (starting from 1 too).
		/// </summary>
		void JumpTo(int line, int column);
		
		/// <summary>
		/// gets the 'caret' position line (starting from 1)
		/// </summary>
		int Line {
			get;
		}

		/// <summary>
		/// gets the 'caret' position column (starting from 1)
		/// </summary>
		int Column {
			get;
		}
	}
}
