// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision: 3925 $</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Editor
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
	}
}
