// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.SharpDevelop.Editor.CodeCompletion
{
	/// <summary>
	/// Represents the completion window showing a ICompletionItemList.
	/// </summary>
	public interface ICompletionListWindow : ICompletionWindow
	{
		/// <summary>
		/// Gets/Sets the currently selected item.
		/// </summary>
		ICompletionItem SelectedItem { get; set; }
	}
}
