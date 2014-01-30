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

using System;

namespace ICSharpCode.SharpDevelop.Editor.CodeCompletion
{
	/// <summary>
	/// Base interface for IInsightWindow and ICompletionListWindow.
	/// </summary>
	public interface ICompletionWindow
	{
		/// <summary>
		/// Closes the window.
		/// </summary>
		void Close();
		
		/// <summary>
		/// Occurs after the window was closed.
		/// </summary>
		event EventHandler Closed;
		
		/// <summary>
		/// Gets/Sets the width of the window.
		/// double.NaN is used to represent automatic width.
		///
		/// For the completion list window default width is a fixed number - using automatic width
		/// will reduce performance when a large number of items is shown.
		/// </summary>
		double Width { get; set; }
		
		/// <summary>
		/// Gets/Sets the height of the window.
		/// double.NaN is used to represent automatic height.
		/// </summary>
		double Height { get; set; }
		
		/// <summary>
		/// Gets/Sets whether the window should close automatically.
		/// The default value is true.
		/// </summary>
		bool CloseAutomatically { get; set; }
		
		/// <summary>
		/// Gets/Sets the start of the text range in which the window stays open.
		/// Has no effect if CloseAutomatically is false.
		/// </summary>
		int StartOffset { get; set; }
		
		/// <summary>
		/// Gets/Sets the end of the text range in which the window stays open.
		/// Has no effect if CloseAutomatically is false.
		/// </summary>
		int EndOffset { get; set; }
	}
}
