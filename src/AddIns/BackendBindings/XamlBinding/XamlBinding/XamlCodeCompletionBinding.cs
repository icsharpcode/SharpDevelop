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
			XmlElementPath path;
			int offset;
			switch (ch) {
				case '<':
					int prevLTCharPos = GetPreviousLTCharPos(editor.Document.Text, editor.Caret.Offset) + 1;
					path = XmlParser.GetActiveElementStartPathAtIndex(editor.Document.Text, prevLTCharPos);
					if (path != null && path.Elements.Count > 0) {
						ICompletionItemList list = CreateListForContext(editor, XamlContext.AtTag, path, null);
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
							path = XmlParser.GetActiveElementStartPathAtIndex(document, offset);
							
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
				case '"':
					offset = editor.Caret.Offset;
					path = XmlParser.GetActiveElementStartPathAtIndex(editor.Document.Text, offset);
					if (!XmlParser.IsInsideAttributeValue(editor.Document.Text, editor.Caret.Offset)) {
						editor.Document.Insert(offset, "\"");
						editor.Caret.Offset = offset;
					}
					break;
				case '{': // starting point for Markup Extension Completion
					offset = editor.Caret.Offset;
					if (offset > 0) {
						int searchOffset = offset - 1;
						char c = editor.Document.GetCharAt(searchOffset);
						while (char.IsWhiteSpace(c)) {
							searchOffset--;
							c = editor.Document.GetCharAt(searchOffset);
						}
						if (XmlParser.IsInsideAttributeValue(editor.Document.Text, editor.Caret.Offset) && c == '"') {
							editor.Document.Insert(offset, "}");
							editor.Caret.Offset = offset;
						}
					}
					break;
				case ' ':
					path = XmlParser.GetActiveElementStartPathAtIndex(editor.Document.Text, editor.Caret.Offset);
					if (path != null && path.Elements.Count > 0) {
						if (!XmlParser.IsInsideAttributeValue(editor.Document.Text, editor.Caret.Offset)) {
							ICompletionItemList list = CreateListForContext(editor, XamlContext.InTag, path, null);
							editor.ShowCompletionWindow(list);
							return CodeCompletionKeyPressResult.Completed;
						}
					}
					break;
				default:
					if (char.IsLetter(ch)) {
						offset = editor.Caret.Offset;
						if (offset > 0) {
							char c = editor.Document.GetCharAt(offset - 1);
							if (c == ' ' || c == '\t') {
								path = XmlParser.GetActiveElementStartPathAtIndex(editor.Document.Text, offset);
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
					list.Items.AddRange(CreateListForElement(info, editor.Document.Text, editor.Caret.Line, editor.Caret.Column));
					break;
				case XamlContext.InTag:
					list.Items.AddRange(CreateListForAttributeName(info, editor.Document.Text, new XamlExpressionContext(path, null, false)));
					break;
				case XamlContext.InAttributeValue:
					if (entity is IProperty) {
						IProperty prop = entity as IProperty;
						IReturnType type = prop.ReturnType;
						if (type != null) {
							TypeCompletion(type.GetUnderlyingClass(), list);
						}
					} else if (entity is IEvent) {
						IEvent e = entity as IEvent;
						IMethod invoker = GetInvokeMethod(e.ReturnType);
						if (invoker == null)
							break;
						var item = path.Elements[path.Elements.Count - 1];
						string name = Utils.GetNameAttributeValue(editor.Document.Text, editor.Caret.Offset);
						list.Items.Add(new NewEventCompletionItem(e, (string.IsNullOrEmpty(name)) ? item.Name : name));
						AddMatchingEventHandlers(editor, invoker, list.Items);
					}
					break;
			}
			
			list.SortItems();
			
			return list;
		}
		
		static void AddMatchingEventHandlers(ITextEditor editor, IMethod delegateInvoker, List<ICompletionItem> list)
		{
			ParseInformation p = ParserService.GetParseInformation(editor.FileName);
			var unit = p.MostRecentCompilationUnit;
			var loc = editor.Document.OffsetToPosition(editor.Caret.Offset);
			IClass c = unit.GetInnermostClass(loc.Line, loc.Column);
			CompoundClass compound = c.GetCompoundClass() as CompoundClass;
			if (compound != null) {
				foreach (IClass part in compound.Parts) {
					foreach (IMethod m in part.Methods) {
						if (m.Parameters.Count != delegateInvoker.Parameters.Count)
							continue;
						
						if ((m.ReturnType != null && delegateInvoker.ReturnType != null) && m.ReturnType.DotNetName != delegateInvoker.ReturnType.DotNetName)
							continue;
						
						bool equal = true;
						for (int i = 0; i < m.Parameters.Count; i++) {
							equal &= CompareParameter(m.Parameters[i], delegateInvoker.Parameters[i]);
							if (!equal)
								break;
						}
						if (equal) {
							list.Add(new XamlCompletionItem(m));
						}
					}
				}
			}
		}
		
		static bool CompareParameter(IParameter p1, IParameter p2)
		{
			bool result = p1.ReturnType.DotNetName == p2.ReturnType.DotNetName;
			
			result &= (p1.IsOut == p2.IsOut);
			result &= (p1.IsParams == p2.IsParams);
			result &= (p1.IsRef == p2.IsRef);
			
			return result;
		}
		
		static IMethod GetInvokeMethod(IReturnType type)
		{
			if (type == null)
				return null;
			
			foreach (IMethod method in type.GetMethods()) {
				if (method.Name == "Invoke")
					return method;
			}
			
			return null;
		}
		
		static bool TypeCompletion(IClass type, XamlCompletionItemList list)
		{
			switch (type.ClassType) {
				case ClassType.Enum:
					foreach (IField f in type.Fields) {
						list.Items.Add(new XamlCompletionItem(f));
					}
					return true;
				case ClassType.Struct:
					if (type.FullyQualifiedName == "System.Boolean") {
						list.Items.Add(new DefaultCompletionItem("True"));
						list.Items.Add(new DefaultCompletionItem("False"));
						return true;
					}
					break;
			}
			
			return false;
		}

		public bool CtrlSpace(ICSharpCode.SharpDevelop.ITextEditor editor)
		{
//			XamlCompletionItemProvider provider = new XamlCompletionItemProvider(XamlExpressionContext.Empty);
//			provider.ShowCompletion(editor);
			return true;
		}

		static List<ICompletionItem> CreateListForAttributeName(ParseInformation parseInfo, string fileContent, XamlExpressionContext context)
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

		static bool IsReaderAtTarget(XmlTextReader r, int caretLine, int caretColumn)
		{
			if (r.LineNumber > caretLine)
				return true;
			else if (r.LineNumber == caretLine)
				return r.LinePosition >= caretColumn;
			else
				return false;
		}

		static List<ICompletionItem> CreateListForElement(ParseInformation parseInfo, string fileContent, int caretLine, int caretColumn)
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
}
