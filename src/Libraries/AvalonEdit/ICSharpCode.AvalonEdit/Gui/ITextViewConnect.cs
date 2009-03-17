// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.AvalonEdit.Gui
{
	/// <summary>
	/// Allows <see cref="VisualLineElementGenerator"/>s, <see cref="IVisualLineTransformer"/>s and
	/// <see cref="IBackgroundRenderer"/>s to be notified when they are added or removed from a text view.
	/// </summary>
	public interface ITextViewConnect
	{
		/// <summary>
		/// Called when added to a text view.
		/// </summary>
		void AddToTextView(TextView textView);
		
		/// <summary>
		/// Called when removed from a text view.
		/// </summary>
		void RemoveFromTextView(TextView textView);
	}
}
