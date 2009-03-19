// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.AvalonEdit.CodeCompletion
{
	/// <summary>
	/// Describes an entry in the <see cref="CompletionList"/>.
	/// </summary>
	public interface ICompletionData
	{
		/// <summary>
		/// Gets the text. This property is used to filter the list of visible elements.
		/// </summary>
		string Text { get; }
		
		/// <summary>
		/// The displayed content. This can be the same as 'Text', or a WPF UIElement if
		/// you want to display rich content.
		/// </summary>
		object Content { get; }
		
		/// <summary>
		/// Gets the description.
		/// </summary>
		object Description { get; }
	}
}
