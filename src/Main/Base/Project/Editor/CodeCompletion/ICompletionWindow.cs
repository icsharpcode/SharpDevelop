// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
