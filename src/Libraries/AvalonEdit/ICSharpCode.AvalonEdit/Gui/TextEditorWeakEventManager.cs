// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.AvalonEdit.Utils;
using System;

namespace ICSharpCode.AvalonEdit
{
	/// <summary>
	/// Contains weak event managers for the TexEditor events.
	/// </summary>
	public static class TextEditorWeakEventManager
	{
		/// <summary>
		/// Weak event manager for the <see cref="TextEditor.DocumentChanged"/> event.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public sealed class DocumentChanged : WeakEventManagerBase<DocumentChanged, TextEditor>
		{
			/// <inheritdoc/>
			protected override void StartListening(TextEditor source)
			{
				source.DocumentChanged += DeliverEvent;
			}
			
			/// <inheritdoc/>
			protected override void StopListening(TextEditor source)
			{
				source.DocumentChanged -= DeliverEvent;
			}
		}
	}
}
