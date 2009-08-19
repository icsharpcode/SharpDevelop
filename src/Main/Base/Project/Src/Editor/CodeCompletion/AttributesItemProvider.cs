// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Linq;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.SharpDevelop.Editor.CodeCompletion
{
	/// <summary>
	/// Provides code completion for attribute names.
	/// </summary>
	public class AttributesItemProvider : AbstractCompletionItemProvider
	{
		AbstractCompletionItemProvider baseProvider;
		
		public AttributesItemProvider(AbstractCompletionItemProvider baseProvider)
		{
			if (baseProvider == null)
				throw new ArgumentNullException("baseProvider");
			this.baseProvider = baseProvider;
			this.RemoveAttributeSuffix = true;
		}
		
		public bool RemoveAttributeSuffix { get; set; }
		
		public override ICompletionItemList GenerateCompletionList(ITextEditor editor)
		{
			ICompletionItemList list = baseProvider.GenerateCompletionList(editor);
			if (this.RemoveAttributeSuffix && list != null) {
				foreach (CodeCompletionItem d in list.Items.OfType<CodeCompletionItem>()) {
					if (d.Text.EndsWith("Attribute")) {
						d.Text = d.Text.Substring(0, d.Text.Length - 9);
					}
				}
			}
			return list;
		}
	}
}
