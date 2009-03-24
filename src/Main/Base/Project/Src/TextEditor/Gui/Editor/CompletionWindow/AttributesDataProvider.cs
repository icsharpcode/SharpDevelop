// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Linq;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor
{
	/// <summary>
	/// Provides code completion for attribute names.
	/// </summary>
	public class AttributesItemProvider : CtrlSpaceCompletionItemProvider
	{
		public AttributesItemProvider(IProjectContent pc)
			: this(ExpressionContext.Attribute)
		{
		}
		
		public AttributesItemProvider(ExpressionContext context) : base(context)
		{
			this.RemoveAttributeSuffix = true;
		}
		
		public bool RemoveAttributeSuffix { get; set; }
		
		public override ICompletionItemList GenerateCompletionList(ITextEditor editor)
		{
			ICompletionItemList list = base.GenerateCompletionList(editor);
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
