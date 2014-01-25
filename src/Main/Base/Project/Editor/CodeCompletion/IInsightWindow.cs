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
using System.Collections.Generic;
using ICSharpCode.NRefactory.Editor;

namespace ICSharpCode.SharpDevelop.Editor.CodeCompletion
{
	/// <summary>
	/// Describes a set of insight items (e.g. multiple overloads of a method) to be displayed in the insight window.
	/// </summary>
	public interface IInsightWindow : ICompletionWindow
	{
		/// <summary>
		/// Gets the items to display.
		/// </summary>
		IList<IInsightItem> Items { get; }
		
		/// <summary>
		/// Gets/Sets the item that is currently selected.
		/// </summary>
		IInsightItem SelectedItem { get; set; }
		
		/// <summary>
		/// Occurs when the document is changed while the insight window is open.
		/// Use this event to close the insight window or adjust <see cref="EndOffset"/>.
		/// </summary>
		/// <remarks>
		/// Unlike directly attaching to <see cref="IDocument.TextChanged"/>, using the event does not require handlers to unsubscribe
		/// when the insight window is closed. This makes it easier to avoid memory leaks.
		/// </remarks>
		event EventHandler<TextChangeEventArgs> DocumentChanged;
		
		event EventHandler SelectedItemChanged;
		
		event EventHandler CaretPositionChanged;
	}
}
