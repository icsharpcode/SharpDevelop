// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Editor
{
	/// <summary>
	/// Describes a set of insight items (e.g. multiple overloads of a method) to be displayed in the insight window.
	/// </summary>
	public interface IInsightWindow
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
		/// Gets/Sets whether the insight window should close automatically.
		/// The default value is true.
		/// </summary>
		bool CloseAutomatically { get; set; }
		
		/// <summary>
		/// Closes the insight window.
		/// </summary>
		void Close();
		
		/// <summary>
		/// Occurs after the insight window was closed.
		/// </summary>
		event EventHandler Closed;
		
		/// <summary>
		/// Gets/Sets the start of the text range in which the insight window stays open.
		/// Has no effect if CloseAutomatically is false.
		/// </summary>
		int StartOffset { get; set; }
		
		/// <summary>
		/// Gets/Sets the end of the text range in which the insight window stays open.
		/// Has no effect if CloseAutomatically is false.
		/// </summary>
		int EndOffset { get; set; }
	}
}
