// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// If a IViewContent object is from the type IPositionable it signals
	/// that it's a texteditor which could set the caret to a position inside
	/// a file.
	/// </summary>
	[ViewContentService]
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
