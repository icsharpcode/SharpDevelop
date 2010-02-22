// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

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
