// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision: 3731 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.XmlEditor;

namespace ICSharpCode.XamlBinding
{
	public enum XamlContext {
		/// <summary>
		/// After '&lt;'
		/// </summary>
		AtTag,
		/// <summary>
		/// Inside '&lt;TagName &gt;'
		/// </summary>
		InTag,
		/// <summary>
		/// Inside '="Value"'
		/// </summary>
		InAttributeValue
	}
	
	public class XamlCodeCompletionBinding : ICodeCompletionBinding
	{
		public CodeCompletionKeyPressResult HandleKeyPress(ITextEditor editor, char ch)
		{
			XamlResolver resolver = new XamlResolver();
			int offset;
			switch (ch) {
				case '<':
					int prevLTCharPos = GetPreviousLTCharPos(editor.Document.Text, editor.Caret.Offset) + 1;
					XmlElementPath elPath = XmlParser.GetActiveElementStartPathAtIndex(editor.Document.Text, prevLTCharPos);
					if (elPath != null && elPath.Elements.Count > 0) {
						ICompletionItemList list = CreateListForContext(editor, XamlContext.AtTag, elPath, null);
						editor.ShowCompletionWindow(list);
						return CodeCompletionKeyPressResult.Completed;
					}
					break;
				case '>': // XML tag completion
					offset = editor.Caret.Offset;
					if (offset > 0) {
						int searchOffset = offset - 1;
						char c = editor.Document.GetCharAt(searchOffset);
						while (char.IsWhiteSpace(c)) {
							searchOffset--;
							c = editor.Document.GetCharAt(searchOffset);
						}
						if (c != '/') {
							string document = editor.Document.Text.Insert(offset, ">");
							XmlElementPath path = XmlParser.GetActiveElementStartPathAtIndex(document, offset);
							
							if (path != null && path.Elements.Count > 0) {
								QualifiedName last = path.Elements[path.Elements.Count - 1];
								
								if (!Utils.HasMatchingEndTag(last.Name, document, offset)) {
									editor.Document.Insert(offset, "></" + last.Name + ">");
									editor.Caret.Offset = offset + 1;
									return CodeCompletionKeyPressResult.EatKey;
								}
							}
						}
					}
					break;
				case ' ':
					XmlElementPath ePath = XmlParser.GetActiveElementStartPathAtIndex(editor.Document.Text, editor.Caret.Offset);
					if (ePath != null && ePath.Elements.Count > 0) {
						ICompletionItemList list = CreateListForContext(editor, XamlContext.InTag, ePath, null);
						editor.ShowCompletionWindow(list);
						return CodeCompletionKeyPressResult.Completed;
					}
					break;
				default:
					if (char.IsLetter(ch)) {
						offset = editor.Caret.Offset;
						if (offset > 0) {
							char c = editor.Document.GetCharAt(offset - 1);
							if (c == ' ' || c == '\t') {
								XmlElementPath path = XmlParser.GetActiveElementStartPathAtIndex(editor.Document.Text, offset);
								if (path != null && path.Elements.Count > 0) {
									ICompletionItemList list = CreateListForContext(editor, XamlContext.InTag, path, null);
									editor.ShowCompletionWindow(list);
									return CodeCompletionKeyPressResult.CompletedIncludeKeyInCompletion;
								}
							}
						}
					}
					break;
			}
			
			return CodeCompletionKeyPressResult.None;
		}
		
		int GetPreviousLTCharPos(string text, int startIndex)
		{
			if (text == null)
				throw new ArgumentNullException("text");
			
			if (startIndex < 0)
				return -1;
			if (startIndex >= text.Length)
				startIndex = text.Length - 1;
			
			while (startIndex > -1 && text[startIndex] != '<')
				startIndex--;
			
			return startIndex;
		}
		
		static readonly List<ICompletionItem> standardItems = new List<ICompletionItem> {
			new DefaultCompletionItem("!--"),
			new DefaultCompletionItem("![CDATA["),
			new DefaultCompletionItem("?")
		};
		
		public static ICompletionItemList CreateListForContext(ITextEditor editor, XamlContext context, XmlElementPath path, IEntity entity)
		{
			XamlCompletionItemList list = new XamlCompletionItemList();
			ParseInformation info = ParserService.GetParseInformation(editor.FileName);
			
			switch (context) {
				case XamlContext.AtTag:
					list.Items.AddRange(standardItems);
					list.Items.AddRange(CtrlSpaceForElement(info, editor.Document.Text, editor.Caret.Line, editor.Caret.Column));
					break;
				case XamlContext.InTag:
					list.Items.AddRange(CtrlSpaceForAttributeName(info, editor.Document.Text, new XamlExpressionContext(path, null, false)));
					break;
				case XamlContext.InAttributeValue:
					if (entity is IProperty) {
						IProperty prop = entity as IProperty;
						IClass c = prop.ReturnType.GetUnderlyingClass();
						if (c.ClassType == ClassType.Enum) {
							foreach (IField f in c.Fields) {
								list.Items.Add(new XamlCompletionItem(f));
							}
						}
					} else if (entity is IEvent) {
						
					}
					break;
			}
			
			list.SortItems();
			
			return list;
		}

		public bool CtrlSpace(ICSharpCode.SharpDevelop.ITextEditor editor)
		{
//			XamlCompletionItemProvider provider = new XamlCompletionItemProvider(XamlExpressionContext.Empty);
//			provider.ShowCompletion(editor);
			return true;
		}
		
//		public ArrayList CtrlSpace(int caretLineNumber, int caretColumn, ParseInformation parseInfo, string fileContent, ExpressionContext expressionContext)
//		{
//			var callingClass = parseInfo.BestCompilationUnit.GetInnermostClass(caretLineNumber, caretColumn);
//			var context = expressionContext as XamlExpressionContext;
//			if (context == null) {
//				return null;
//			}
//
//			if (context.AttributeName == null) {
//				return CtrlSpaceForElement(fileContent);
//			}
//			else if (context.InAttributeValue) {
//				return CtrlSpaceForAttributeValue(parseInfo, fileContent, context);
//			}
//			else {
//				return CtrlSpaceForAttributeName(parseInfo, fileContent, context);
//			}
//		}
//
		static List<ICompletionItem> CtrlSpaceForAttributeName(ParseInformation parseInfo, string fileContent, XamlExpressionContext context)
		{
			if (context.ElementPath.Elements.Count == 0)
				return null;
			QualifiedName lastElement = context.ElementPath.Elements[context.ElementPath.Elements.Count - 1];
			XamlCompilationUnit cu = parseInfo.BestCompilationUnit as XamlCompilationUnit;
			if (cu == null)
				return null;
			IReturnType rt = cu.CreateType(lastElement.Namespace, lastElement.Name);
			if (rt == null)
				return null;
			var list = new List<ICompletionItem>();
			foreach (IProperty p in rt.GetProperties()) {
				if (p.IsPublic && p.CanSet) {
					list.Add(new XamlCompletionItem(p));
				}
			}
			foreach (IEvent e in rt.GetEvents()) {
				if (e.IsPublic) {
					list.Add(new XamlCompletionItem(e));
				}
			}
			return list;
		}
//
//		ArrayList CtrlSpaceForAttributeValue(ParseInformation parseInfo, string fileContent, XamlExpressionContext context)
//		{
//			ArrayList attributes = CtrlSpaceForAttributeName(parseInfo, fileContent, context);
//			if (attributes != null) {
//				foreach (IProperty p in attributes.OfType<IProperty>()) {
//					if (p.Name == context.AttributeName && p.ReturnType != null) {
//						IClass c = p.ReturnType.GetUnderlyingClass();
//						if (c != null && c.ClassType == ClassType.Enum) {
//							return EnumCompletion(c);
//						}
//					}
//				}
//			}
//			return null;
//		}
//
//		ArrayList EnumCompletion(IClass enumClass)
//		{

//		}
//
		static bool IsReaderAtTarget(XmlTextReader r, int caretLine, int caretColumn)
		{
			if (r.LineNumber > caretLine)
				return true;
			else if (r.LineNumber == caretLine)
				return r.LinePosition >= caretColumn;
			else
				return false;
		}

		static List<ICompletionItem> CtrlSpaceForElement(ParseInformation parseInfo, string fileContent, int caretLine, int caretColumn)
		{
			using (XmlTextReader r = new XmlTextReader(new StringReader(fileContent))) {
				try {
					r.WhitespaceHandling = WhitespaceHandling.Significant;
					// move reader to correct position
					while (r.Read() && !IsReaderAtTarget(r, caretLine, caretColumn)) { }
				}
				catch (XmlException) {
				}
				var result = new List<ICompletionItem>();
				IProjectContent pc = parseInfo.BestCompilationUnit.ProjectContent;

				TypeResolveResult rr = new XamlResolver().Resolve(new ExpressionResult(r.Name), parseInfo, fileContent) as TypeResolveResult;
//				if (rr != null) {
//					AddPropertiesForType(result, r, rr);
//					AddEventsForType(result, r, rr);
//				}

				foreach (var ns in r.GetNamespacesInScope(XmlNamespaceScope.ExcludeXml)) {
					var list = XamlCompilationUnit.GetNamespaceMembers(pc, ns.Value);
					if (list != null) {
						foreach (IClass c in list.OfType<IClass>()) {
							if (c.ClassType != ClassType.Class)
								continue;
							if (c.IsAbstract && c.IsStatic)
								continue;
							if (c.ClassInheritanceTree.Any(b => b.FullyQualifiedName == "System.Attribute"))
								continue;
							if (!c.Methods.Any(m => m.IsConstructor && m.IsPublic))
								continue;
							if (string.IsNullOrEmpty(ns.Key))
								result.Add(new XamlCompletionItem(c));
							else
								result.Add(new XamlCompletionItem(c, ns.Key));
						}
					}
				}

				return result;
			}
		}
	}
	
	class XamlCompletionItem : DefaultCompletionItem
	{
		IEntity entity;
		
		public IEntity Entity {
			get { return entity; }
		}
		
		public XamlCompletionItem(IEntity entity, string prefix)
			: base(prefix + ":" + entity.Name)
		{
			this.entity = entity;
		}
		
		public XamlCompletionItem(IEntity entity)
			: base(entity.Name)
		{
			this.entity = entity;
		}
	}
}
