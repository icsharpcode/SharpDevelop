// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision: 3494 $</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.TextEditor.Gui.CompletionWindow;

namespace ICSharpCode.XamlBinding
{
	sealed class XamlCompletionItemProvider : CtrlSpaceCompletionItemProvider
	{
		public XamlCompletionItemProvider()
		{
		}
		
		public XamlCompletionItemProvider(XamlExpressionContext context)
			: base(context)
		{
		}
		
		public override ICompletionItemList GenerateCompletionListForCompletionData(ArrayList arr, ExpressionContext context)
		{
			DefaultCompletionItemList list = new DefaultCompletionItemList();
			list.Items.AddRange(base.GenerateCompletionListForCompletionData(arr, context).Items);
			
			if (context is XamlExpressionContext) {
				XamlExpressionContext xContext = context as XamlExpressionContext;
				
				if (string.IsNullOrEmpty(xContext.AttributeName) && !xContext.InAttributeValue) {
					list.Items.Add(new DefaultCompletionItem("!--"));
					list.Items.Add(new DefaultCompletionItem("![CDATA["));
					list.Items.Add(new DefaultCompletionItem("?"));
				}
			}
			
			list.SortItems();
			
			return list;
		}
	}
}
