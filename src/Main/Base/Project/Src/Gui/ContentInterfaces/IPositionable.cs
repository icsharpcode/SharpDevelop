// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

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
		/// Sets the 'caret' to the position pos, where pos.Y is the line (starting from 0).
		/// And pos.X is the column (starting from 0 too).
		/// </summary>
		void JumpTo(int line, int column);
	}
}
