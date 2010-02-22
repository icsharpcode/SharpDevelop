// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

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
	}
}
