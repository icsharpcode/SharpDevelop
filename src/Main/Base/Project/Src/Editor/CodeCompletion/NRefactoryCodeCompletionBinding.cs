// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;
using CSTokens = ICSharpCode.NRefactory.Parser.CSharp.Tokens;
using VBTokens = ICSharpCode.NRefactory.Parser.VB.Tokens;

namespace ICSharpCode.SharpDevelop.Editor.CodeCompletion
{
	/// <summary>
	/// Base class for C# and VB Code Completion Binding.
	/// </summary>
	public abstract class NRefactoryCodeCompletionBinding : DefaultCodeCompletionBinding
	{
		readonly SupportedLanguage language;
		readonly LanguageProperties languageProperties;
		
		protected NRefactoryCodeCompletionBinding(SupportedLanguage language)
		{
			this.language = language;
			if (language == SupportedLanguage.CSharp) {
				languageProperties = LanguageProperties.CSharp;
			} else {
				languageProperties = LanguageProperties.VBNet;
			}
			
			insightHandler = new NRefactoryInsightWindowHandler(language);
		}
		
		public override bool CtrlSpace(ITextEditor editor)
		{
			NRefactoryCtrlSpaceCompletionItemProvider provider = new NRefactoryCtrlSpaceCompletionItemProvider(languageProperties);
			provider.AllowCompleteExistingExpression = true;
			// on Ctrl+Space, include items (e.g. types / extension methods) from all namespaces, regardless of imports
			provider.ShowItemsFromAllNamespaces = true;
			provider.ShowCompletion(editor);
			return true;
		}
		
		protected bool ProvideContextCompletion(ITextEditor editor, IReturnType expected, char charTyped)
		{
			if (expected == null) return false;
			IClass c = expected.GetUnderlyingClass();
			if (c == null) return false;
			if (c.ClassType == ClassType.Enum) {
				CtrlSpaceCompletionItemProvider cdp = new NRefactoryCtrlSpaceCompletionItemProvider(languageProperties);
				var ctrlSpaceList = cdp.GenerateCompletionList(editor);
				if (ctrlSpaceList == null) return false;
				ContextCompletionItemList contextList = new ContextCompletionItemList();
				contextList.Items.AddRange(ctrlSpaceList.Items);
				contextList.activationKey = charTyped;
				foreach (CodeCompletionItem item in contextList.Items.OfType<CodeCompletionItem>()) {
					IClass itemClass = item.Entity as IClass;
					if (itemClass != null && c.FullyQualifiedName == itemClass.FullyQualifiedName && c.TypeParameters.Count == itemClass.TypeParameters.Count) {
						contextList.SuggestedItem = item;
						break;
					}
				}
				if (contextList.SuggestedItem != null) {
					if (charTyped != ' ') contextList.InsertSpace = true;
					editor.ShowCompletionWindow(contextList);
					return true;
				}
			}
			return false;
		}
		
		private class ContextCompletionItemList : DefaultCompletionItemList
		{
			internal char activationKey;
			
			public override CompletionItemListKeyResult ProcessInput(char key)
			{
				if (key == '=' && activationKey == '=')
					return CompletionItemListKeyResult.BeforeStartKey;
				activationKey = '\0';
				return base.ProcessInput(key);
			}
		}
		
		protected IMember GetCurrentMember(ITextEditor editor)
		{
			var caret = editor.Caret;
			NRefactoryResolver r = new NRefactoryResolver(languageProperties);
			if (r.Initialize(ParserService.GetParseInformation(editor.FileName), caret.Line, caret.Column)) {
				return r.CallingMember;
			} else {
				return null;
			}
		}
	}
}
