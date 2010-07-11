// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com"/>
//     <version>$Revision: 6077 $</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;

using ICSharpCode.NRefactory.Parser.VB;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace ICSharpCode.VBNetBinding
{
	public class VBNetCodeCompletionDataProvider : NRefactoryCtrlSpaceCompletionItemProvider
	{
		ExpressionResult result;
		
		public VBNetCodeCompletionDataProvider(ExpressionResult result)
			: base(LanguageProperties.VBNet, result.Context)
		{
			this.result = result;
		}
		
		protected override List<ICompletionEntry> CtrlSpace(ITextEditor editor, ExpressionContext context)
		{
			var list = base.CtrlSpace(editor, context);
			
			BitArray expectedSet = result.Tag as BitArray;
			
			if (expectedSet != null)
				AddVBNetKeywords(list, expectedSet);
			
			return list;
		}
		
		static void AddVBNetKeywords(List<ICompletionEntry> ar, BitArray keywords)
		{
			for (int i = 0; i < keywords.Length; i++) {
				if (keywords[i] && i >= Tokens.AddHandler) {
					ar.Add(new KeywordEntry(Tokens.GetTokenString(i)));
				}
			}
		}
	}
}
