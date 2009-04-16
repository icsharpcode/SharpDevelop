// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.XmlEditor;

namespace ICSharpCode.XamlBinding
{
	public static class CompletionDataHelper
	{
		#region Pre-defined lists
		static readonly List<ICompletionItem> standardElements = new List<ICompletionItem> {
			new DefaultCompletionItem("!--"),
			new DefaultCompletionItem("![CDATA["),
			new DefaultCompletionItem("?")
		};
		
		static readonly List<ICompletionItem> standardAttributes = new List<ICompletionItem> {
			new DefaultCompletionItem("xmlns:")
		};
		#endregion
		
		public const string XamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml";
		
		static List<ICompletionItem> CreateListForAttributeName(ParseInformation parseInfo, string fileContent, XamlExpressionContext context, string[] existingItems)
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
				if (p.IsPublic && p.CanSet && !existingItems.Contains(p.Name)) {
					list.Add(new XamlCompletionItem(p));
				}
			}
			foreach (IEvent e in rt.GetEvents()) {
				if (e.IsPublic && !existingItems.Contains(e.Name)) {
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
		
		public static List<ICompletionItem> CreateListForElement(ParseInformation parseInfo, string fileContent, int caretLine, int caretColumn)
		{
			var items = GetClassesFromContext(parseInfo, fileContent, caretLine, caretColumn);
			var result = new List<ICompletionItem>();
			
			foreach (var ns in items) {
				result.AddRange(from c in ns.Value
				                where (c.ClassType == ClassType.Class &&
				                       !c.IsAbstract && !c.IsStatic &&
				                       !c.ClassInheritanceTree.Any(b => b.FullyQualifiedName == "System.Attribute") &&
				                       c.Methods.Any(m => m.IsConstructor && m.IsPublic))
				                select (new XamlCompletionItem(c, ns.Key) as ICompletionItem)
				               );
			}
			
			return result;
		}
		
		static bool IsCollectionType(IReturnType rt)
		{
			if (rt == null)
				return false;
			return rt.GetMethods().Any(m => m.Name == "Add" && m.IsPublic);
		}
		
		public static List<ICompletionItem> CreateListOfMarkupExtensions(ParseInformation parseInfo, string fileContent, int caretLine, int caretColumn)
		{
			var list = CreateListForElement(parseInfo, fileContent, caretLine, caretColumn);
			
			var neededItems = list
				.Where(i => ((i as XamlCompletionItem).Entity as IClass).ClassInheritanceTree
				       .Any(item => item.FullyQualifiedName == "System.Windows.Markup.MarkupExtension"))
				.Select(
					selItem => {
						var it = selItem as XamlCompletionItem;
						string text = it.Text;
						if (it.Text.EndsWith("Extension"))
							text = text.Remove(it.Text.Length - "Extension".Length);
						return new XamlCompletionItem(text, it.Entity) as ICompletionItem;
					}
				);
			
			return neededItems.ToList();
		}

		public static ICompletionItemList CreateListForContext(ITextEditor editor, XamlContext context, XmlElementPath path, IEntity entity)
		{
			XamlCompletionItemList list = new XamlCompletionItemList();
			ParseInformation info = ParserService.GetParseInformation(editor.FileName);
			
			switch (context) {
				case XamlContext.AtTag:
					list.Items.AddRange(standardElements);
					list.Items.AddRange(CreateListForElement(info, editor.Document.Text, editor.Caret.Line, editor.Caret.Column));
					break;
				case XamlContext.InTag:
					list.Items.AddRange(CreateListForAttributeName(info, editor.Document.Text, new XamlExpressionContext(path, null, false), Utils.GetListOfExistingAttributeNames(editor.Document.Text, editor.Caret.Offset)));
					
					QualifiedName last = path.Elements[path.Elements.Count - 1];
					
					TypeResolveResult trr = new XamlResolver().Resolve(new ExpressionResult(last.Name, new XamlExpressionContext(path, null, false)), info, editor.Document.Text) as TypeResolveResult;
					
					if (trr != null && trr.ResolvedType != null && trr.ResolvedType.GetUnderlyingClass() != null) {
						if (trr.ResolvedType.GetUnderlyingClass().ClassInheritanceTree.Any(i => i.FullyQualifiedName == "System.Windows.DependencyObject")) {
							list.Items.AddRange(GetListOfAttachedProperties(info, editor.Document.Text, editor.Caret.Line, editor.Caret.Column));
							list.Items.AddRange(GetListOfAttachedEvents(info, editor.Document.Text, editor.Caret.Line, editor.Caret.Column));
						}
					}
					list.Items.AddRange(standardAttributes);
					break;
				case XamlContext.InAttributeValue:
					XamlCodeCompletionBinding.Instance.CtrlSpace(editor);
					break;
			}
			
			list.SortItems();
			
			if (list.Items.Count >= 1)
				list.SuggestedItem = list.Items[0];
			
			return list;
		}
		
		public static ICompletionItemList CreateMarkupExtensionCompletion(MarkupExtensionInfo markup, ParseInformation info, ITextEditor editor, char ch)
		{
			var list = new XamlCompletionItemList();
			var path = XmlParser.GetActiveElementStartPathAtIndex(editor.Document.Text, editor.Caret.Offset);
			
			var innerMarkup = GetInnermostMarkup(markup);
			
			var trr = ResolveMarkupExtensionType(innerMarkup, info, editor, path);
			
			if (trr == null) {
				list.Items.AddRange(CreateListOfMarkupExtensions(info, editor.Document.Text, editor.Caret.Line, editor.Caret.Column));
				list.PreselectionLength = innerMarkup.Type.Length;
				list.SuggestedItem = list.Items.FirstOrDefault(item => item.Text.StartsWith(innerMarkup.Type));
			} else {
				if (markup.NamedArguments.Count == 0) {
					if (trr.ResolvedType != null) {
						switch (trr.ResolvedType.FullyQualifiedName) {
							case "System.Windows.Markup.ArrayExtension":
							case "System.Windows.Markup.NullExtension":
								// x:Null/x:Array does not need completion, ignore it
								break;
							case "System.Windows.Markup.StaticExtension":
								if (markup.PositionalArguments.Count == 1 && ch == ' ')
									break;
								if (markup.PositionalArguments.Count <= 1) {
									if (ch == '.') {
										// TODO : enum and field completion
									} else {
										var items = GetClassesFromContext(info, editor.Document.Text, editor.Caret.Line, editor.Caret.Column);
										foreach (var ns in items) {
											list.Items.AddRange(ns.Value.Where(c => c.Fields.Any(f => f.IsStatic) || c.Properties.Any(p => p.IsStatic)).Select(c => new XamlCompletionItem(c, ns.Key) as ICompletionItem));
										}
										
										AttributeValue selItem = markup.PositionalArguments.LastOrDefault();
										
										if (selItem != null && selItem.IsString) {
											string s = selItem.StringValue;
											list.PreselectionLength = s.Length;
											list.SuggestedItem = list.Items.FirstOrDefault(item => item.Text.StartsWith(s, StringComparison.OrdinalIgnoreCase));
										}
									}
								}
								break;
							case "System.Windows.Markup.TypeExtension":
								if (markup.PositionalArguments.Count == 1 && ch == ' ')
									break;
								if (markup.PositionalArguments.Count <= 1) {
									list.Items.AddRange(CreateListForElement(info, editor.Document.Text, editor.Caret.Line, editor.Caret.Column));
									AttributeValue selItem = markup.PositionalArguments.LastOrDefault();
									
									
									
									if (selItem != null && selItem.IsString) {
										string s = selItem.StringValue;
										list.PreselectionLength = s.Length;
										list.SuggestedItem = list.Items.FirstOrDefault(item => item.Text.StartsWith(s, StringComparison.OrdinalIgnoreCase));
									}
								}
								break;
							default:
								var ctors = trr.ResolvedType
									.GetMethods()
									.Where(m => m.IsPublic && m.IsConstructor && m.Parameters.Count >= markup.PositionalArguments.Count + 1)
									.OrderBy(m => m.Parameters.Count);
								
								//var ctor = FindCompletableCtor(ctors, markup.PositionalArguments.Count)
								break;
						}
					}
				} else {
					// TODO : named args completion
				}
			}
			
			list.SortItems();
			if (list.SuggestedItem == null)
				list.SuggestedItem = list.Items.FirstOrDefault();
			
			return list;
		}
		
		static IMethod FindCompletableCtor(IList<IMethod> ctors, int index)
		{
			return null;
		}
		
		static MarkupExtensionInfo GetInnermostMarkup(MarkupExtensionInfo markup)
		{
			var lastPair = markup.NamedArguments.LastOrDefault();
			var last = markup.PositionalArguments.LastOrDefault();
			
			if (markup.NamedArguments.Count > 0)
				last = lastPair.Value;
			
			if (last != null) {
				if (!last.IsString) {
					return GetInnermostMarkup(last.ExtensionValue);
				}
			}
			
			return markup;
		}

		static TypeResolveResult ResolveMarkupExtensionType(MarkupExtensionInfo markup, ParseInformation info, ITextEditor editor, XmlElementPath path)
		{
			XamlResolver resolver = new XamlResolver();
			TypeResolveResult trr = resolver.Resolve(new ExpressionResult(markup.Type, new XamlExpressionContext(path, null, false)), info, editor.Document.Text) as TypeResolveResult;
			if (trr == null) trr = resolver.Resolve(new ExpressionResult(markup.Type + "Extension", new XamlExpressionContext(path, null, false)), info, editor.Document.Text) as TypeResolveResult;
			
			return trr;
		}
		
		public static void AddMatchingEventHandlers(ITextEditor editor, IMethod delegateInvoker, List<ICompletionItem> list)
		{
			ParseInformation p = ParserService.GetParseInformation(editor.FileName);
			var unit = p.MostRecentCompilationUnit;
			var loc = editor.Document.OffsetToPosition(editor.Caret.Offset);
			IClass c = unit.GetInnermostClass(loc.Line, loc.Column);
			if (c == null)
				return;
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
		
		static IDictionary<string, IEnumerable<IClass>> GetClassesFromContext(ParseInformation parseInfo, string fileContent, int caretLine, int caretColumn)
		{
			using (XmlTextReader r = new XmlTextReader(new StringReader(fileContent))) {
				try {
					r.WhitespaceHandling = WhitespaceHandling.Significant;
					// move reader to correct position
					while (r.Read() && !IsReaderAtTarget(r, caretLine, caretColumn)) { }
				} catch (XmlException) {}
				
				IProjectContent pc = parseInfo.BestCompilationUnit.ProjectContent;
				
				var result = new Dictionary<string, IEnumerable<IClass>>();
				
				foreach (var ns in r.GetNamespacesInScope(XmlNamespaceScope.ExcludeXml)) {
					result.Add(ns.Key, XamlCompilationUnit.GetNamespaceMembers(pc, ns.Value));
				}
				
				return result;
			}
		}
		
		static List<ICompletionItem> GetListOfAttachedProperties(ParseInformation parseInfo, string fileContent, int caretLine, int caretColumn)
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
							
							var attachedProperties = c.Fields
								.Where(f =>
								       f.IsPublic &&
								       f.IsStatic &&
								       f.IsReadonly &&
								       f.ReturnType != null &&
								       f.ReturnType.FullyQualifiedName == "System.Windows.DependencyProperty" &&
								       f.Name.Length > "Property".Length &&
								       f.Name.EndsWith("Property", StringComparison.Ordinal) &&
								       c.Methods.Any(m =>
								                     m.IsPublic &&
								                     m.IsStatic &&
								                     m.Name.Length > 3 &&
								                     (m.Name.StartsWith("Get", StringComparison.Ordinal) || m.Name.StartsWith("Set", StringComparison.Ordinal)) &&
								                     m.Name.Remove(0, 3) == f.Name.Remove(f.Name.Length - "Property".Length)
								                    )
								      );
							
							result.AddRange(attachedProperties
							                .Select(item => {
							                        	string name = (!string.IsNullOrEmpty(ns.Key)) ? ns.Key + ":" : "";
							                        	string property = item.Name.Remove(item.Name.Length - "Property".Length);
							                        	name += c.Name + "." + item.Name.Remove(item.Name.Length - "Property".Length);
							                        	return new XamlCompletionItem(name, new DefaultProperty(c, property) { ReturnType = GetAttachedPropertyType(item, c) } ) as ICompletionItem;
							                        }));
						}
					}
				}
				
				return result;
			}
		}
		
		static List<ICompletionItem> GetListOfAttachedEvents(ParseInformation parseInfo, string fileContent, int caretLine, int caretColumn)
		{
			var items = GetClassesFromContext(parseInfo, fileContent, caretLine, caretColumn);
			var result = new List<ICompletionItem>();
			
			foreach (var ns in items) {
				foreach (IClass c in ns.Value) {
					if (c.ClassType != ClassType.Class)
						continue;
					if (c.IsAbstract && c.IsStatic)
						continue;
					if (c.ClassInheritanceTree.Any(b => b.FullyQualifiedName == "System.Attribute"))
						continue;
					if (!c.Methods.Any(m => m.IsConstructor && m.IsPublic))
						continue;
					
					var attachedEvents = c.Fields
						.Where(f =>
						       f.IsPublic &&
						       f.IsStatic &&
						       f.IsReadonly &&
						       f.ReturnType != null &&
						       f.ReturnType.FullyQualifiedName == "System.Windows.RoutedEvent" &&
						       f.Name.Length > "Event".Length &&
						       f.Name.EndsWith("Event", StringComparison.Ordinal) &&
						       c.Methods.Any(m =>
						                     m.IsPublic &&
						                     m.IsStatic &&
						                     m.Name.Length > 3 &&
						                     (m.Name.StartsWith("Add", StringComparison.Ordinal) || m.Name.StartsWith("Remove", StringComparison.Ordinal)) &&
						                     m.Name.EndsWith("Handler", StringComparison.Ordinal) &&
						                     IsMethodFromEvent(f, m)
						                    )
						      );
					
					result.AddRange(attachedEvents
					                .Select(
					                	item => new XamlCompletionItem(
					                		(string.IsNullOrEmpty(ns.Key) ? "" : ns.Key + ":") + c.Name + "." + item.Name.Remove(item.Name.Length - "Event".Length),
					                		new DefaultEvent(c, GetEventNameFromField(item)) {
					                			ReturnType = GetAttachedEventDelegateType(item, c)
					                		}
					                	) as ICompletionItem
					                )
					               );
				}
			}
			
			return result;
		}
		
		static IReturnType GetAttachedEventDelegateType(IField field, IClass c)
		{
			if (c == null || field == null)
				return null;
			
			string eventName = field.Name.Remove(field.Name.Length - "Event".Length);
			
			IMethod method = c.Methods
				.Where(m =>
				       m.IsPublic &&
				       m.IsStatic &&
				       m.Parameters.Count == 2 &&
				       (m.Name == "Add" + eventName + "Handler" ||
				        m.Name == "Remove" + eventName + "Handler"))
				.FirstOrDefault();
			
			if (method == null)
				return null;
			
			return method.Parameters[1].ReturnType;
		}
		
		static IReturnType GetAttachedPropertyType(IField field, IClass c)
		{
			if (c == null || field == null)
				return null;
			
			string propertyName = field.Name.Remove(field.Name.Length - "Property".Length);
			
			IMethod method = c.Methods
				.Where(m =>
				       m.IsPublic &&
				       m.IsStatic &&
				       m.Name == "Get" + propertyName)
				.FirstOrDefault();
			
			if (method == null)
				return null;
			
			return method.ReturnType;
		}
		
		static string GetEventNameFromMethod(IMethod m)
		{
			string mName = m.Name;
			if (mName.StartsWith("Add", StringComparison.Ordinal))
				mName = mName.Remove(0, 3);
			else if (mName.StartsWith("Remove", StringComparison.Ordinal))
				mName = mName.Remove(0, 6);
			if (mName.EndsWith("Handler", StringComparison.Ordinal))
				mName = mName.Remove(mName.Length - "Handler".Length);
			
			return mName;
		}
		
		static string GetEventNameFromField(IField f)
		{
			string fName = f.Name;
			if (fName.EndsWith("Event", StringComparison.Ordinal))
				fName = fName.Remove(fName.Length - "Event".Length);
			
			return fName;
		}
		
		static bool IsMethodFromEvent(IField f, IMethod m)
		{
			return GetEventNameFromField(f) == GetEventNameFromMethod(m);
		}
	}
}
