// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;

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
