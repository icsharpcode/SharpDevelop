// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
