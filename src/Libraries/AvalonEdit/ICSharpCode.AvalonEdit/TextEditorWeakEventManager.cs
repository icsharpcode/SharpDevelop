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
	/// Contains weak event managers for <see cref="ITextEditorComponent"/>.
	/// </summary>
	public static class TextEditorWeakEventManager
	{
		/// <summary>
		/// Weak event manager for the <see cref="ITextEditorComponent.DocumentChanged"/> event.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public sealed class DocumentChanged : WeakEventManagerBase<DocumentChanged, ITextEditorComponent>
		{
			/// <inheritdoc/>
			protected override void StartListening(ITextEditorComponent source)
			{
				source.DocumentChanged += DeliverEvent;
			}
			
			/// <inheritdoc/>
			protected override void StopListening(ITextEditorComponent source)
			{
				source.DocumentChanged -= DeliverEvent;
			}
		}
		
		/// <summary>
		/// Weak event manager for the <see cref="ITextEditorComponent.OptionChanged"/> event.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public sealed class OptionChanged : WeakEventManagerBase<OptionChanged, ITextEditorComponent>
		{
			/// <inheritdoc/>
			protected override void StartListening(ITextEditorComponent source)
			{
				source.OptionChanged += DeliverEvent;
			}
			
			/// <inheritdoc/>
			protected override void StopListening(ITextEditorComponent source)
			{
				source.OptionChanged -= DeliverEvent;
			}
		}
	}
}
