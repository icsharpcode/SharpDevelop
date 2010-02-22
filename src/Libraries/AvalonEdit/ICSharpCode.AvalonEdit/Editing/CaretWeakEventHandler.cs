// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.AvalonEdit.Utils;
using System;

namespace ICSharpCode.AvalonEdit.Editing
{
	/// <summary>
	/// Contains classes for handling weak events on the Caret class.
	/// </summary>
	public static class CaretWeakEventManager
	{
		/// <summary>
		/// Handles the Caret.PositionChanged event.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public sealed class PositionChanged : WeakEventManagerBase<PositionChanged, Caret>
		{
			/// <inheritdoc/>
			protected override void StartListening(Caret source)
			{
				source.PositionChanged += DeliverEvent;
			}
			
			/// <inheritdoc/>
			protected override void StopListening(Caret source)
			{
				source.PositionChanged -= DeliverEvent;
			}
		}
	}
}
