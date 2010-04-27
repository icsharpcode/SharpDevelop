// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.NRefactory;

namespace ICSharpCode.SharpDevelop.Editor
{
	public class ToolTipRequestEventArgs : EventArgs
	{
		/// <summary>
		/// Gets whether the tool tip request was handled.
		/// </summary>
		public bool Handled { get; set; }
		
		/// <summary>
		/// Gets the editor causing the request.
		/// </summary>
		public ITextEditor Editor { get; private set; }
		
		/// <summary>
		/// Gets whether the mouse was inside the document bounds.
		/// </summary>
		public bool InDocument { get; set; }
		
		/// <summary>
		/// The mouse position, in document coordinates.
		/// </summary>
		public Location LogicalPosition { get; set; }
		
		/// <summary>
		/// Gets/Sets the content to show as a tooltip.
		/// </summary>
		public object ContentToShow { get; set; }
		
		/// <summary>
		/// Sets the tooltip to be shown.
		/// </summary>
		public void SetToolTip(object content)
		{
			if (content == null)
				throw new ArgumentNullException("content");
			this.Handled = true;
			this.ContentToShow = content;
		}
		
		public ToolTipRequestEventArgs(ITextEditor editor)
		{
			if (editor == null)
				throw new ArgumentNullException("editor");
			this.Editor = editor;
			this.InDocument = true;
		}
	}
}
