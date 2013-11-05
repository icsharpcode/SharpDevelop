// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;

using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.XamlBinding
{
	public class XamlCompletionItemList : DefaultCompletionItemList
	{
		XamlContext context;
		
		public XamlCompletionItemList(XamlContext context)
		{
			this.context = context;
		}
		
		public override CompletionItemListKeyResult ProcessInput(char key)
		{
			if (key == ':' || key == '/')
				return CompletionItemListKeyResult.NormalKey;
			
			if (key == '.' && (context.InAttributeValueOrMarkupExtension && context.Attribute.Name.StartsWith("xmlns")))
				return CompletionItemListKeyResult.NormalKey;
			
			return base.ProcessInput(key);
		}
		
		static int CountWhiteSpacesAtEnd(string text)
		{
			if (string.IsNullOrEmpty(text))
				return 0;
			
			int i = text.Length - 1;
			
			while (i >= 0) {
				if (!char.IsWhiteSpace(text[i]))
					break;
				i--;
			}
			
			return text.Length - i - 1;
		}
		
		public override void Complete(CompletionContext context, ICompletionItem item)
		{
			using (context.Editor.Document.OpenUndoGroup()) {
				base.Complete(context, item);
				
				XamlCompletionContext xamlContext = XamlContextResolver.ResolveCompletionContext(context.Editor, context.CompletionChar);
				
				if (xamlContext.Description == XamlContextDescription.None && (context.StartOffset <= 0 || context.Editor.Document.GetCharAt(context.StartOffset - 1) != '<')) {
					context.Editor.Document.Insert(context.StartOffset, "<");
					context.EndOffset++;
				}
				
				if (item is XamlCompletionItem && !item.Text.EndsWith(":", StringComparison.Ordinal)) {
					XamlCompletionItem cItem = item as XamlCompletionItem;
					
					if (xamlContext.Description == XamlContextDescription.InTag) {
						context.Editor.Document.Insert(context.EndOffset, "=\"\"");
						context.CompletionCharHandled = context.CompletionChar == '=';
						context.Editor.Caret.Offset--;
						new XamlCodeCompletionBinding().CtrlSpace(context.Editor);
					} else if (xamlContext.Description == XamlContextDescription.InMarkupExtension && !string.IsNullOrEmpty(xamlContext.RawAttributeValue)) {
						string valuePart = xamlContext.RawAttributeValue.Substring(0, xamlContext.ValueStartOffset);
						AttributeValue value = MarkupExtensionParser.ParseValue(valuePart);
						
						if (value != null && !value.IsString) {
							var markup = Utils.GetMarkupExtensionAtPosition(value.ExtensionValue, context.Editor.Caret.Offset);
							if (markup.NamedArguments.Count > 0 || markup.PositionalArguments.Count > 0) {
								int oldOffset = context.Editor.Caret.Offset;
								context.Editor.Caret.Offset = context.StartOffset;
								string word = context.Editor.GetWordBeforeCaret().TrimEnd();
								int spaces = CountWhiteSpacesAtEnd(context.Editor.GetWordBeforeCaret());
								int typeNameStart = markup.ExtensionType.IndexOf(':') + 1;
								
								if (!(word == "." || word == "," || word == ":") && markup.ExtensionType.Substring(typeNameStart, markup.ExtensionType.Length - typeNameStart) != word) {
									context.Editor.Document.Replace(context.Editor.Caret.Offset - spaces, spaces, ", ");
									oldOffset += (2 - spaces);
								}
								
								context.Editor.Caret.Offset = oldOffset;
							}
						}
						
						if (cItem.Text.EndsWith("=", StringComparison.OrdinalIgnoreCase))
							new XamlCodeCompletionBinding().CtrlSpace(context.Editor);
					}
				}
				
				if (item is NewEventCompletionItem) {
					CreateEventHandlerCode(xamlContext, item as NewEventCompletionItem);
				}
				
				if (item is XmlnsCompletionItem) {
					context.Editor.Caret.Offset++;
				}
				
				switch (item.Text) {
					case "![CDATA[":
						context.Editor.Document.Insert(context.Editor.Caret.Offset, "]]>");
						context.Editor.Caret.Offset -= 3;
						break;
					case "?":
						context.Editor.Document.Insert(context.Editor.Caret.Offset, "?>");
						context.Editor.Caret.Offset -= 2;
						break;
					case "!--":
						context.Editor.Document.Insert(context.Editor.Caret.Offset, "  -->");
						context.Editor.Caret.Offset -= 4;
						break;
				}
				
				if (item.Text.StartsWith("/", StringComparison.OrdinalIgnoreCase)) {
					context.Editor.Document.Insert(context.EndOffset, ">");
					context.CompletionCharHandled = context.CompletionChar == '>';
					context.Editor.Caret.Offset++;
				}
			}
		}
		
		static bool CreateEventHandlerCode(XamlCompletionContext context, NewEventCompletionItem completionItem)
		{
			IProject project = SD.ProjectService.FindProjectContainingFile(context.Editor.FileName);
			if (project == null) return false;
			var unresolved = context.ParseInformation.UnresolvedFile.TypeDefinition;
			if (unresolved == null) return false;
			var compilation = SD.ParserService.GetCompilationForFile(context.Editor.FileName);
			var definition = unresolved.Resolve(new SimpleTypeResolveContext(compilation.MainAssembly)).GetDefinition();
			project.LanguageBinding.CodeGenerator.InsertEventHandler(definition, completionItem.HandlerName, completionItem.EventType, true);
			return true;
		}
	}
}
