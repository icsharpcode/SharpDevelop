// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Snippets
{
	/// <summary>
	/// A snippet element that has sub-elements.
	/// </summary>
	[Serializable]
	public class SnippetContainerElement : SnippetElement
	{
		NullSafeCollection<SnippetElement> elements = new NullSafeCollection<SnippetElement>();
		
		/// <summary>
		/// Gets the list of child elements.
		/// </summary>
		public IList<SnippetElement> Elements {
			get { return elements; }
		}
		
		/// <inheritdoc/>
		public override void Insert(InsertionContext context)
		{
			foreach (SnippetElement e in this.Elements) {
				e.Insert(context);
			}
		}
	}
}
