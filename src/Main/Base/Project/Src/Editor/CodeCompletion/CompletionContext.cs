// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.SharpDevelop.Editor.CodeCompletion
{
	/// <summary>
	/// Container class for the parameters available to the Complete function.
	/// </summary>
	public class CompletionContext
	{
		/// <summary>
		/// Gets/Sets the editor in which completion is performed.
		/// </summary>
		public ITextEditor Editor { get; set; }
		
		/// <summary>
		/// Gets/Sets the start offset of the completion range.
		/// </summary>
		public int StartOffset { get; set; }
		
		/// <summary>
		/// Gets/Sets the end offset of the completion range.
		/// </summary>
		public int EndOffset { get; set; }
		
		/// <summary>
		/// Gets the length between EndOffset and StartOffset.
		/// </summary>
		public int Length { get { return EndOffset - StartOffset; } }
		
		/// <summary>
		/// Gets/Sets the character that triggered completion.
		/// This property is '\0' when completion was triggered using the mouse.
		/// </summary>
		public char CompletionChar { get; set; }
		
		/// <summary>
		/// Gets/Sets whether the CompletionChar was already inserted.
		/// </summary>
		public bool CompletionCharHandled { get; set; }
	}
}
