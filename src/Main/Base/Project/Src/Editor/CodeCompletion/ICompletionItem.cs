// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Editor.CodeCompletion
{
	public interface ICompletionItem
	{
		string Text { get; }
		string Description { get; }
		IImage Image { get; }
		
		/// <summary>
		/// Performs code completion for the item.
		/// </summary>
		void Complete(CompletionContext context);
		
		/// <summary>
		/// Gets a priority value for the completion data item.
		/// When selecting items by their start characters, the item with the highest
		/// priority is selected first.
		/// </summary>
		double Priority {
			get;
		}
	}
	
	public class DefaultCompletionItem : ICompletionItem
	{
		public string Text { get; private set; }
		public virtual string Description { get; set; }
		public virtual IImage Image { get; set; }
		
		public virtual double Priority { get { return 0; } }
		
		public DefaultCompletionItem(string text)
		{
			this.Text = text;
		}
		
		public virtual void Complete(CompletionContext context)
		{
			context.Editor.Document.Replace(context.StartOffset, context.Length, this.Text);
			
			context.EndOffset = context.StartOffset + this.Text.Length;
		}
	}
}
