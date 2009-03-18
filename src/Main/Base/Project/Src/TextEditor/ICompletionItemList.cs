// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop
{
	public interface ICompletionItemList
	{
		IEnumerable<ICompletionItem> Items { get; }
	}
	
	public class DefaultCompletionItemList : ICompletionItemList
	{
		IList<ICompletionItem> items;
		
		public DefaultCompletionItemList()
			: this(new List<ICompletionItem>())
		{
		}
		
		public DefaultCompletionItemList(IList<ICompletionItem> items)
		{
			this.items = items;
		}
		
		public IList<ICompletionItem> Items {
			get { return items; }
		}
		
		IEnumerable<ICompletionItem> ICompletionItemList.Items {
			get { return items; }
		}
	}
}
