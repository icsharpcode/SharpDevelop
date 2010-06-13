// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Konicek" email="martin.konicek@gmail.com"/>
//     <version>$Revision: $</version>
// </file>
using System;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace ICSharpCode.SharpDevelop.Editor.CodeCompletion
{
	/// <summary>
	/// <see cref="ICompletionItemList" /> created by <see cref="NRefactoryCtrlSpaceCompletionItemProvider" />.
	/// </summary>
	public class NRefactoryCompletionItemList : DefaultCompletionItemList
	{
		/// <summary>
		/// <see cref="NRefactoryCtrlSpaceCompletionItemProvider" /> sets this to true if this list contains items
		/// from all namespaces, regardless of current imports.
		/// </summary>
		public bool ContainsItemsFromAllNamespaces { get; set; }
		
		/// <inheritdoc />
		public override bool ContainsAllAvailableItems
		{
			get { return ContainsItemsFromAllNamespaces; }
		}
	}
}
