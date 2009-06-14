/*
 * Created by SharpDevelop.
 * User: Daniel
 * Date: 06.06.2009
 * Time: 13:08
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
 
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
