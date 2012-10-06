// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media;

using ICSharpCode.AvalonEdit.Xml;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.XamlBinding
{
	public static class CompletionDataHelper
	{
		static readonly List<ICompletionItem> standardElements = new List<ICompletionItem> {
			new SpecialCompletionItem("!--"),
			new SpecialCompletionItem("![CDATA["),
			new SpecialCompletionItem("?")
		};
		
		// [XAML 2009]
		static readonly List<string> xamlBuiltInTypes = new List<string> {
			"Object", "Boolean", "Char", 	"String", "Decimal", "Single", "Double",
			"Int16", "Int32", "Int64", "TimeSpan", "Uri", 	"Byte", "Array", "List", "Dictionary",
			// This is no built in type, but a markup extension
			"Reference"
		};
		
		static readonly List<ICompletionItem> standardAttributes = new List<ICompletionItem> {
			new SpecialCompletionItem("xmlns:"),
			new XamlCompletionItem("xml", "", "space"),
			new XamlCompletionItem("xml", "", "lang")
		};
		
		public static readonly ReadOnlyCollection<string> XamlNamespaceAttributes = new List<string> {
			"Class", "ClassModifier", "FieldModifier", "Name", "Subclass", "TypeArguments", "Uid", "Key"
		}.AsReadOnly();
		
		public static readonly ReadOnlyCollection<string> RootOnlyElements = new List<string> {
			"Class", "ClassModifier", "Subclass"
		}.AsReadOnly();
		
		public static readonly ReadOnlyCollection<string> ChildOnlyElements = new List<string> {
			"FieldModifier"
		}.AsReadOnly();
		
		static readonly List<ICompletionItem> emptyList = new List<ICompletionItem>();
		
		/// <summary>
		/// value: http://schemas.microsoft.com/winfx/2006/xaml
		/// </summary>
		public const string XamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml";
		
		/// <summary>
		/// values: http://schemas.microsoft.com/winfx/2006/xaml/presentation,
		/// http://schemas.microsoft.com/netfx/2007/xaml/presentation
		/// </summary>
		public static readonly string[] WpfXamlNamespaces = new[] {
			"http://schemas.microsoft.com/winfx/2006/xaml/presentation",
			"http://schemas.microsoft.com/netfx/2007/xaml/presentation"
		};
		
		/// <summary>
		/// value: http://schemas.openxmlformats.org/markup-compatibility/2006
		/// </summary>
		public const string MarkupCompatibilityNamespace = "http://schemas.openxmlformats.org/markup-compatibility/2006";
		
		public const bool EnableXaml2009 = true;
		
		public static XamlContext ResolveContext(string text, string fileName, int offset)
		{
			return ResolveContext(new StringTextBuffer(text), fileName, offset);
		}
		
		public static XamlContext ResolveContext(ITextBuffer fileContent, string fileName, int offset)
		{
			//using (new DebugTimerObject("ResolveContext")) {
			XamlParser parser = string.IsNullOrEmpty(fileName) ? new XamlParser() : ParserService.GetParser(fileName) as XamlParser;
			ParseInformation info = string.IsNullOrEmpty(fileName) ? null : ParserService.GetParseInformation(fileName);

			using (parser.ParseAndLock(fileContent)) {
				
				AXmlDocument document = parser.LastDocument;
				AXmlObject currentData = document.GetChildAtOffset(offset);
				
				string attribute = string.Empty, attributeValue = string.Empty;
				bool inAttributeValue = false;
				AttributeValue value = null;
				bool isRoot = false;
				bool wasAXmlElement = false;
				int offsetFromValueStart = -1;
				
				List<AXmlElement> ancestors = new List<AXmlElement>();
				Dictionary<string, string> xmlns = new Dictionary<string, string>();
				List<string> ignored = new List<string>();
				string xamlNamespacePrefix = string.Empty;
				
				var item = currentData;
				
				while (item != document) {
					if (item is AXmlElement) {
						AXmlElement element = item as AXmlElement;
						ancestors.Add(element);
						foreach (var attr in element.Attributes) {
							if (attr.IsNamespaceDeclaration) {
								string prefix = (attr.Name == "xmlns") ? "" : attr.LocalName;
								if (!xmlns.ContainsKey(prefix))
									xmlns.Add(prefix, 	attr.Value);
							}
							
							if (attr.LocalName == "Ignorable" && attr.Namespace == MarkupCompatibilityNamespace)
								ignored.AddRange(attr.Value.Split(' ', '\t'));
							
							if (string.IsNullOrEmpty(xamlNamespacePrefix) && attr.Value == XamlNamespace)
								xamlNamespacePrefix = attr.LocalName;
						}
						
						if (!wasAXmlElement && item.Parent is AXmlDocument)
							isRoot = true;
						
						wasAXmlElement = true;
					}
					
					item = item.Parent;
				}
				
				XamlContextDescription description = XamlContextDescription.None;
				
				AXmlElement active = null;
				AXmlElement parent = null;
				
				if (currentData.Parent is AXmlTag) {
					AXmlTag tag = currentData.Parent as AXmlTag;
					if (tag.IsStartOrEmptyTag)
						description = XamlContextDescription.InTag;
					else if (tag.IsComment)
						description = XamlContextDescription.InComment;
					else if (tag.IsCData)
						description = XamlContextDescription.InCData;
					active = tag.Parent as AXmlElement;
				}
				
				if (currentData is AXmlAttribute) {
					AXmlAttribute a = currentData as AXmlAttribute;
					int valueStartOffset = a.StartOffset + (a.Name ?? "").Length + (a.EqualsSign ?? "").Length + 1;
					attribute = a.Name;
					attributeValue = a.Value;
					value = MarkupExtensionParser.ParseValue(attributeValue);
					
					inAttributeValue = offset >= valueStartOffset && offset < a.EndOffset;
					if (inAttributeValue) {
						offsetFromValueStart = offset - valueStartOffset;
						
						description = XamlContextDescription.InAttributeValue;
						
						if (value != null && !value.IsString)
							description = XamlContextDescription.InMarkupExtension;
						if (attributeValue.StartsWith("{}", StringComparison.Ordinal) && attributeValue.Length > 2)
							description = XamlContextDescription.InAttributeValue;
					} else
						description = XamlContextDescription.InTag;
				}
				
				if (currentData is AXmlTag) {
					AXmlTag tag = currentData as AXmlTag;
					if (tag.IsStartOrEmptyTag || tag.IsEndTag)
						description = XamlContextDescription.AtTag;
					else if (tag.IsComment)
						description = XamlContextDescription.InComment;
					else if (tag.IsCData)
						description = XamlContextDescription.InCData;
					active = tag.Parent as AXmlElement;
				}
				
				if (active != ancestors.FirstOrDefault())
					parent = ancestors.FirstOrDefault();
				else
					parent = ancestors.Skip(1).FirstOrDefault();
				
				if (active == null)
					active = parent;
				
				var xAttribute = currentData as AXmlAttribute;
				
				var context = new XamlContext() {
					Description         = description,
					ActiveElement       = (active == null) ? null : active.ToWrapper(),
					ParentElement       = (parent == null) ? null : parent.ToWrapper(),
					Ancestors           = ancestors.Select(ancestor => ancestor.ToWrapper()).ToList(),
					Attribute           = (xAttribute != null) ? xAttribute.ToWrapper() : null,
					InRoot              = isRoot,
					AttributeValue      = value,
					RawAttributeValue   = attributeValue,
					ValueStartOffset    = offsetFromValueStart,
					XmlnsDefinitions    = xmlns,
					ParseInformation    = info,
					IgnoredXmlns        = ignored.AsReadOnly(),
					XamlNamespacePrefix = xamlNamespacePrefix
				};
				
				return context;
			}
			//}
		}
		
		public static XamlCompletionContext ResolveCompletionContext(ITextEditor editor, char typedValue)
		{
			var binding = editor.GetService(typeof(XamlLanguageBinding)) as XamlLanguageBinding;
			
			if (binding == null)
				throw new InvalidOperationException("Can only use ResolveCompletionContext with a XamlLanguageBinding.");
			
			var context = new XamlCompletionContext(ResolveContext(editor.Document.CreateSnapshot(), editor.FileName, editor.Caret.Offset)) {
				PressedKey = typedValue,
				Editor = editor
			};
			
			return context;
		}
		
		public static XamlCompletionContext ResolveCompletionContext(ITextEditor editor, char typedValue, int offset)
		{
			var binding = editor.GetService(typeof(XamlLanguageBinding)) as XamlLanguageBinding;
			
			if (binding == null)
				throw new InvalidOperationException("Can only use ResolveCompletionContext with a XamlLanguageBinding.");
			
			var context = new XamlCompletionContext(ResolveContext(editor.Document.CreateSnapshot(), editor.FileName, offset)) {
				PressedKey = typedValue,
				Editor = editor
			};
			
			return context;
		}
		
		static List<ICompletionItem> CreateAttributeList(XamlCompletionContext context, bool includeEvents)
		{
			ElementWrapper lastElement = context.ActiveElement;
			if (context.ParseInformation == null)
				return emptyList;
			XamlCompilationUnit cu = context.ParseInformation.CompilationUnit as XamlCompilationUnit;
			if (cu == null)
				return emptyList;
			IReturnType rt = cu.CreateType(lastElement.Namespace, lastElement.LocalName.Trim('.'));
			
			var list = new List<ICompletionItem>();
			
			string xamlPrefix = context.XamlNamespacePrefix;
			string xKey = string.IsNullOrEmpty(xamlPrefix) ? "" : xamlPrefix + ":";
			
			if (xamlBuiltInTypes.Concat(XamlNamespaceAttributes).Select(s => xKey + s).Contains(lastElement.Name))
				return emptyList;
			
			if (lastElement.LocalName.EndsWith(".", StringComparison.OrdinalIgnoreCase) || context.PressedKey == '.') {
				if (rt == null)
					return list;
				
				string key = string.IsNullOrEmpty(lastElement.Prefix) ? "" : lastElement.Prefix + ":";
				
				if (context.ParentElement != null && context.ParentElement.LocalName.StartsWith(lastElement.LocalName.TrimEnd('.'), StringComparison.OrdinalIgnoreCase)) {
					AddAttributes(rt, list, includeEvents);
					AddAttachedProperties(rt.GetUnderlyingClass(), list, key, lastElement.Name.Trim('.'));
				} else
					AddAttachedProperties(rt.GetUnderlyingClass(), list, key, lastElement.Name.Trim('.'));
			} else {
				if (rt == null) {
					list.Add(new XamlCompletionItem(xamlPrefix, XamlNamespace, "Uid"));
				} else {
					AddAttributes(rt, list, includeEvents);
					list.AddRange(GetListOfAttached(context, string.Empty, string.Empty, includeEvents, true));
					foreach (string item in XamlNamespaceAttributes.Where(item => AllowedInElement(context.InRoot, item)))
						list.Add(new XamlCompletionItem(xamlPrefix, XamlNamespace, item));
				}
			}
			
			return list;
		}
		
		static void AddAttributes(IReturnType rt, IList<ICompletionItem> list, bool includeEvents)
		{
			if (rt == null)
				return;
			
			foreach (IProperty p in rt.GetProperties()) {
				if (p.IsPublic && (p.IsPubliclySetable() || p.ReturnType.IsCollectionReturnType()))
					list.Add(new XamlCodeCompletionItem(p));
			}
			
			if (includeEvents) {
				foreach (IEvent e in rt.GetEvents()) {
					if (e.IsPublic) {
						list.Add(new XamlCodeCompletionItem(e));
					}
				}
			}
		}
		
		static bool AllowedInElement(bool inRoot, string item)
		{
			return inRoot ? !ChildOnlyElements.Contains(item) : !RootOnlyElements.Contains(item);
		}
		
		public static IEnumerable<ICompletionItem> CreateListForXmlnsCompletion(IProjectContent projectContent)
		{
			List<XmlnsCompletionItem> list = new List<XmlnsCompletionItem>();
			
			foreach (IProjectContent content in projectContent.ThreadSafeGetReferencedContents()) {
				foreach (IAttribute att in content.GetAssemblyAttributes()) {
					if (att.PositionalArguments.Count == 2
					    && att.AttributeType.FullyQualifiedName == "System.Windows.Markup.XmlnsDefinitionAttribute") {
						list.Add(new XmlnsCompletionItem(att.PositionalArguments[0] as string, true));
					}
				}
				
				foreach (string @namespace in content.NamespaceNames) {
					if (!string.IsNullOrEmpty(@namespace))
						list.Add(new XmlnsCompletionItem(@namespace, content.AssemblyName));
				}
			}
			
			foreach (string @namespace in projectContent.NamespaceNames) {
				if (!string.IsNullOrEmpty(@namespace))
					list.Add(new XmlnsCompletionItem(@namespace, false));
			}
			
			list.Add(new XmlnsCompletionItem(MarkupCompatibilityNamespace, true));
			
			return list
				.Distinct(new XmlnsEqualityComparer())
				.OrderBy(item => item, new XmlnsComparer());
		}
		
		static string GetContentPropertyName(IReturnType type)
		{
			if (type == null)
				return string.Empty;
			
			IClass c = type.GetUnderlyingClass();
			
			if (c == null)
				return string.Empty;
			
			IAttribute contentProperty = c.Attributes
				.FirstOrDefault(attribute => attribute.AttributeType.FullyQualifiedName == "System.Windows.Markup.ContentPropertyAttribute");
			if (contentProperty != null) {
				return contentProperty.PositionalArguments.FirstOrDefault() as string
					?? (contentProperty.NamedArguments.ContainsKey("Name") ? contentProperty.NamedArguments["Name"] as string : string.Empty);
			}
			
			return string.Empty;
		}
		
		public static IList<ICompletionItem> CreateElementList(XamlCompletionContext context, bool classesOnly, bool includeAbstract)
		{
			var items = GetClassesFromContext(context);
			var result = new List<ICompletionItem>();
			var last = context.ParentElement;
			
			if (context.ParseInformation == null)
				return emptyList;

			XamlCompilationUnit cu = context.ParseInformation.CompilationUnit as XamlCompilationUnit;
			
			IReturnType rt = null;

			if (last != null && cu != null) {
				if (!last.Name.Contains(".") || last.Name.EndsWith(".", StringComparison.OrdinalIgnoreCase)) {
					rt = cu.CreateType(last.Namespace, last.LocalName.Trim('.'));
					string contentPropertyName = GetContentPropertyName(rt);
					if (!string.IsNullOrEmpty(contentPropertyName)) {
						string fullName = last.Name + "." + contentPropertyName;
						MemberResolveResult mrr = XamlResolver.Resolve(fullName, context) as MemberResolveResult;
						
						if (mrr != null) {
							rt = mrr.ResolvedType;
						}
					}
				} else {
					MemberResolveResult mrr = XamlResolver.Resolve(last.Name, context) as MemberResolveResult;
					
					if (mrr != null) {
						rt = mrr.ResolvedType;
					}
				}
			}
			
			bool isList = rt != null && rt.IsListReturnType();
			
			bool parentAdded = false;
			
			foreach (var ns in items) {
				foreach (var c in ns.Value) {
					if (includeAbstract) {
						if (c.ClassType == ClassType.Class) {
							if (c.IsStatic || c.DerivesFrom("System.Attribute"))
								continue;
						} else if (c.ClassType == ClassType.Interface) {
						} else {
							continue;
						}
					} else {
						if (!(c.ClassType == ClassType.Class && c.IsAbstract == includeAbstract && !c.IsStatic &&
						      // TODO : use c.DefaultReturnType.GetConstructors(ctor => ctor.IsAccessible(context.ParseInformation.CompilationUnit.Classes.FirstOrDefault(), false)) after DOM rewrite
						      !c.DerivesFrom("System.Attribute") && (c.AddDefaultConstructorIfRequired || c.Methods.Any(m => m.IsConstructor && m.IsAccessible(context.ParseInformation.CompilationUnit.Classes.FirstOrDefault(), false)))))
							continue;
					}
					
					if (last != null && isList) {
						var possibleTypes = rt.GetMethods()
							.Where(a => a.Parameters.Count == 1 && a.Name == "Add")
							.Select(method => method.Parameters.First().ReturnType.GetUnderlyingClass()).Where(p => p != null);
						
						if (!possibleTypes.Any(t => c.ClassInheritanceTreeClassesOnly.Any(c2 => c2.FullyQualifiedName == t.FullyQualifiedName)))
							continue;
					}
					
					XamlCodeCompletionItem item = new XamlCodeCompletionItem(c, ns.Key);
					
					parentAdded = parentAdded || (last != null && item.Text == last.Name);
					
					result.Add(new XamlCodeCompletionItem(c, ns.Key));
				}
			}
			
			if (!parentAdded && last != null && !last.Name.Contains(".")) {
				IClass itemClass = cu.CreateType(last.Namespace, last.LocalName.Trim('.')).GetUnderlyingClass();
				if (itemClass != null)
					result.Add(new XamlCodeCompletionItem(itemClass, last.Prefix));
			}
			
			var xamlItems = XamlNamespaceAttributes.AsEnumerable();
			
			if (EnableXaml2009)
				xamlItems = xamlBuiltInTypes.Concat(XamlNamespaceAttributes);
			
			foreach (string item in xamlItems)
				result.Add(new XamlCompletionItem(context.XamlNamespacePrefix, XamlNamespace, item));
			
			return result;
		}
		
		public static IEnumerable<ICompletionItem> GetAllTypes(XamlCompletionContext context)
		{
			var items = GetClassesFromContext(context);
			
			foreach (var ns in items) {
				foreach (var c in ns.Value) {
					if (c.ClassType == ClassType.Class && !c.DerivesFrom("System.Attribute"))
						yield return new XamlCodeCompletionItem(c, ns.Key);
				}
			}
		}

		public static IEnumerable<ICompletionItem> CreateListOfMarkupExtensions(XamlCompletionContext context)
		{
			var list = CreateElementList(context, true, false);
			
			var neededItems = list.OfType<XamlCodeCompletionItem>()
				.Where(i => (i.Entity as IClass).DerivesFrom("System.Windows.Markup.MarkupExtension"));
			foreach (XamlCodeCompletionItem it in neededItems) {
				string text = it.Text;
				if (it.Text.EndsWith("Extension", StringComparison.Ordinal))
					text = text.Remove(it.Text.Length - "Extension".Length);
				it.Text = text;
			}
			
			return neededItems.Cast<ICompletionItem>().Add(new XamlCompletionItem(context.XamlNamespacePrefix, XamlNamespace, "Reference"));
		}

		public static XamlCompletionItemList CreateListForContext(XamlCompletionContext context)
		{
			XamlCompletionItemList list = new XamlCompletionItemList(context);
			
			ParseInformation info = context.ParseInformation;
			ITextEditor editor = context.Editor;
			
			switch (context.Description) {
				case XamlContextDescription.None:
					if (context.Forced) {
						list.Items.AddRange(standardElements);
						list.Items.AddRange(CreateElementList(context, false, false));
						AddClosingTagCompletion(context, list);
					}
					break;
				case XamlContextDescription.AtTag:
					if ((editor.Caret.Offset > 0 && editor.Document.GetCharAt(editor.Caret.Offset - 1) == '.') || context.PressedKey == '.') {
						list.Items.AddRange(CreateAttributeList(context, false));
					} else {
						list.Items.AddRange(standardElements);
						list.Items.AddRange(CreateElementList(context, false, false));
						AddClosingTagCompletion(context, list);
					}
					break;
				case XamlContextDescription.InTag:
					DebugTimer.Start();
					
					string word = context.Editor.GetWordBeforeCaretExtended();
					
					if (context.PressedKey == '.' || word.Contains(".")) {
						string ns = "";
						int pos = word.IndexOf(':');
						if (pos > -1)
							ns = word.Substring(0, pos);
						
						string element = word.Substring(pos + 1, word.Length - pos - 1);
						string className = word;
						int propertyStart = element.IndexOf('.');
						if (propertyStart != -1) {
							element = element.Substring(0, propertyStart).TrimEnd('.');
							className = className.Substring(0, propertyStart + pos + 1).TrimEnd('.');
						}
						TypeResolveResult trr = XamlResolver.Resolve(className, context) as TypeResolveResult;
						IClass typeClass = (trr != null && trr.ResolvedType != null) ? trr.ResolvedType.GetUnderlyingClass() : null;
						
						if (typeClass != null && typeClass.HasAttached(true, true))
							list.Items.AddRange(GetListOfAttached(context, element, ns, true, true));
					} else {
						QualifiedNameWithLocation last = context.ActiveElement.ToQualifiedName();
						TypeResolveResult trr = XamlResolver.Resolve(last.Name, context) as TypeResolveResult;
						IClass typeClass = (trr != null && trr.ResolvedType != null) ? trr.ResolvedType.GetUnderlyingClass() : null;
						list.Items.AddRange(CreateAttributeList(context, true));
						list.Items.AddRange(standardAttributes);
					}
					
					DebugTimer.Stop("CreateListForContext - InTag");
					break;
				case XamlContextDescription.InAttributeValue:
					new XamlCodeCompletionBinding().CtrlSpace(editor);
					break;
			}
			
			list.SortItems();
			
			return list;
		}
		
		static void AddClosingTagCompletion(XamlCompletionContext context, XamlCompletionItemList list)
		{
			if (context.ParentElement != null && !context.InRoot) {
				ResolveResult rr = XamlResolver.Resolve(context.ParentElement.Name, context);
				TypeResolveResult trr = rr as TypeResolveResult;
				MemberResolveResult mrr = rr as MemberResolveResult;

				if (trr != null) {
					if (trr.ResolvedClass == null)
						return;
					list.Items.Add(new XamlCodeCompletionItem("/" + context.ParentElement.Name, trr.ResolvedClass));
				} else if (mrr != null) {
					if (mrr.ResolvedMember == null)
						return;
					list.Items.Add(new XamlCodeCompletionItem("/" + context.ParentElement.Name, mrr.ResolvedMember));
				}
			}
		}

		public static IEnumerable<IInsightItem> CreateMarkupExtensionInsight(XamlCompletionContext context)
		{
			var markup = Utils.GetMarkupExtensionAtPosition(context.AttributeValue.ExtensionValue, context.ValueStartOffset);
			var type = ResolveType(markup.ExtensionType, context) ?? ResolveType(markup.ExtensionType + "Extension", context);
			
			if (type != null) {
				var ctors = type.GetMethods()
					.Where(m => m.IsPublic && m.IsConstructor && m.Parameters.Count >= markup.PositionalArguments.Count)
					.OrderBy(m => m.Parameters.Count);
				
				foreach (var ctor in ctors)
					yield return new MarkupExtensionInsightItem(ctor);
			}
		}

		public static ICompletionItemList CreateMarkupExtensionCompletion(XamlCompletionContext context)
		{
			var list = new XamlCompletionItemList(context);
			string visibleValue = context.RawAttributeValue.Substring(0, Utils.MinMax(context.ValueStartOffset, 0, context.RawAttributeValue.Length));
			if (context.PressedKey == '=')
				visibleValue += "=";
//			context.RawAttributeValue = visibleValue;
//			context.AttributeValue = MarkupExtensionParser.ParseValue(visibleValue);
			var markup = Utils.GetMarkupExtensionAtPosition(context.AttributeValue.ExtensionValue, context.ValueStartOffset);
			var type = ResolveType(markup.ExtensionType, context) ?? ResolveType(markup.ExtensionType + "Extension", context);
			
			if (type == null) {
				list.Items.AddRange(CreateListOfMarkupExtensions(context));
				list.PreselectionLength = markup.ExtensionType.Length;
			} else {
				if (markup.NamedArguments.Count == 0) {
					if (DoPositionalArgsCompletion(list, context, markup, type))
						DoNamedArgsCompletion(list, context, type, markup);
				} else
					DoNamedArgsCompletion(list, context, type, markup);
			}
			
			list.SortItems();
			
			return list;
		}

		static void DoNamedArgsCompletion(XamlCompletionItemList list, XamlCompletionContext context, IReturnType type, MarkupExtensionInfo markup)
		{
			if (markup.NamedArguments.Count > 0 && !context.Editor.GetWordBeforeCaret().StartsWith(",", StringComparison.OrdinalIgnoreCase)) {
				int lastStart = markup.NamedArguments.Max(i => i.Value.StartOffset);
				var item = markup.NamedArguments.First(p => p.Value.StartOffset == lastStart);
				
				if (context.RawAttributeValue.EndsWith("=", StringComparison.OrdinalIgnoreCase) ||
				    (item.Value.IsString && item.Value.StringValue.EndsWith(context.Editor.GetWordBeforeCaretExtended(), StringComparison.Ordinal))) {
					MemberResolveResult mrr = XamlResolver.ResolveMember(item.Key, context) as MemberResolveResult;
					if (mrr != null && mrr.ResolvedMember != null && mrr.ResolvedMember.ReturnType != null) {
						IReturnType memberType = mrr.ResolvedMember.ReturnType;
						list.Items.AddRange(MemberCompletion(context, memberType, string.Empty));
					}
					return;
				}
			}
			
			list.Items.AddRange(type.GetProperties().Where(p => p.CanSet && p.IsPublic).Select(p => new XamlCodeCompletionItem(p.Name + "=", p)));
		}

		/// <remarks>returns true if elements from named args completion should be added afterwards.</remarks>
		static bool DoPositionalArgsCompletion(XamlCompletionItemList list, XamlCompletionContext context, MarkupExtensionInfo markup, IReturnType type)
		{
			switch (type.FullyQualifiedName) {
				case "System.Windows.Markup.ArrayExtension":
				case "System.Windows.Markup.NullExtension":
					// x:Null/x:Array does not need completion, ignore it
					break;
				case "System.Windows.Markup.StaticExtension":
					if (context.AttributeValue.ExtensionValue.PositionalArguments.Count <= 1)
						return DoStaticExtensionCompletion(list, context);
					break;
				case "System.Windows.Markup.TypeExtension":
					if (context.AttributeValue.ExtensionValue.PositionalArguments.Count <= 1) {
						list.Items.AddRange(GetClassesFromContext(context).FlattenToList());
						AttributeValue selItem = Utils.GetMarkupExtensionAtPosition(context.AttributeValue.ExtensionValue, context.ValueStartOffset)
							.PositionalArguments.LastOrDefault();
						string word = context.Editor.GetWordBeforeCaret().TrimEnd();
						if (selItem != null && selItem.IsString && word == selItem.StringValue) {
							list.PreselectionLength = selItem.StringValue.Length;
						}
					}
					break;
				default:
					var ctors = type.GetMethods()
						.Where(m => m.IsPublic && m.IsConstructor && m.Parameters.Count >= markup.PositionalArguments.Count + 1);
					if (context.Forced)
						return true;
					if (ctors.Any() || markup.PositionalArguments.Count == 0)
						return false;
					break;
			}
			
			return true;
		}
		
		static IEnumerable<ICompletionItem> FlattenToList(this IDictionary<string, IEnumerable<IClass>> data)
		{
			foreach (var item in data) {
				foreach (IClass c in item.Value) {
					yield return new XamlCodeCompletionItem(c, item.Key);
				}
			}
		}

		public static IEnumerable<IInsightItem> MemberInsight(MemberResolveResult result)
		{
			switch (result.ResolvedType.FullyQualifiedName) {
				case "System.Windows.Thickness":
					yield return new MemberInsightItem(result.ResolvedMember, "uniformLength");
					yield return new MemberInsightItem(result.ResolvedMember, "left, top");
					yield return new MemberInsightItem(result.ResolvedMember, "left, top, right, bottom");
					break;
				case "System.Windows.Size":
					yield return new MemberInsightItem(result.ResolvedMember, "width, height");
					break;
				case "System.Windows.Point":
					yield return new MemberInsightItem(result.ResolvedMember, "x, y");
					break;
				case "System.Windows.Rect":
					yield return new MemberInsightItem(result.ResolvedMember, "x, y, width, height");
					break;
			}
		}
		
		public static string LookForTargetTypeValue(XamlCompletionContext context, out bool isExplicit, params string[] elementName) {
			var ancestors = context.Ancestors;
			
			isExplicit = false;
			
			for (int i = 0; i < ancestors.Count; i++) {
				if (ancestors[i].LocalName == "Style" && WpfXamlNamespaces.Contains(ancestors[i].Namespace)) {
					isExplicit = true;
					return ancestors[i].GetAttributeValue("TargetType") ?? string.Empty;
				}
				
				if (ancestors[i].Name.EndsWithAny(elementName.Select(s => "." + s + "s"), StringComparison.Ordinal)
				    && !ancestors[i].Name.StartsWith("Style.", StringComparison.Ordinal)) {
					return ancestors[i].Name.Remove(ancestors[i].Name.IndexOf('.'));
				}
			}
			
			return null;
		}
		
		public static string GetTypeNameFromTypeExtension(MarkupExtensionInfo info, XamlCompletionContext context)
		{
			IReturnType type = CompletionDataHelper.ResolveType(info.ExtensionType, context)
				?? CompletionDataHelper.ResolveType(info.ExtensionType + "Extension", context);
			
			if (type == null || type.FullyQualifiedName != "System.Windows.Markup.TypeExtension")
				return string.Empty;
			
			var item = info.PositionalArguments.FirstOrDefault();
			if (item != null && item.IsString) {
				return item.StringValue;
			} else {
				if (info.NamedArguments.TryGetValue("typename", out item)) {
					if (item.IsString)
						return item.StringValue;
				}
			}
			
			return string.Empty;
		}
		
		public static bool EndsWithAny(this string thisValue, IEnumerable<string> items, StringComparison comparison)
		{
			foreach (string item in items) {
				if (thisValue.EndsWith(item, comparison))
					return true;
			}
			
			return false;
		}
		
		public static bool EndsWithAny(this string thisValue, params char[] items)
		{
			foreach (char item in items) {
				if (thisValue.EndsWith(item.ToString()))
					return true;
			}
			
			return false;
		}
		
		static IReturnType GetType(XamlCompletionContext context, out bool isExplicit)
		{
			AttributeValue value = MarkupExtensionParser.ParseValue(LookForTargetTypeValue(context, out isExplicit, "Trigger", "Setter") ?? string.Empty);
			
			IReturnType typeName = null;
			string typeNameString = null;
			
			if (!value.IsString) {
				typeNameString = GetTypeNameFromTypeExtension(value.ExtensionValue, context);
				typeName = CompletionDataHelper.ResolveType(typeNameString, context);
			} else {
				typeNameString = value.StringValue;
				typeName = CompletionDataHelper.ResolveType(value.StringValue, context);
			}
			
			return typeName;
		}

		public static IEnumerable<ICompletionItem> MemberCompletion(XamlCompletionContext context, IReturnType type, string textPrefix)
		{
			if (type == null || type.GetUnderlyingClass() == null)
				yield break;

			var c = type.GetUnderlyingClass();
			
			if (type is ConstructedReturnType &&  type.TypeArgumentCount > 0 && c.FullyQualifiedName == "System.Nullable") {
				ConstructedReturnType rt = type as ConstructedReturnType;
				string nullExtensionName = "Null";
				if (!string.IsNullOrEmpty(context.XamlNamespacePrefix))
					nullExtensionName = context.XamlNamespacePrefix + ":" + nullExtensionName;
				yield return new SpecialCompletionItem("{" + nullExtensionName + "}");
				c = rt.TypeArguments.First().GetUnderlyingClass();
				if (c == null)
					yield break;
			}
			
			bool isExplicit, showFull = false;
			IReturnType typeName;
			
			string valueBeforeCaret = (context.ValueStartOffset > 0) ?
				context.RawAttributeValue.Substring(0, context.ValueStartOffset) : "";
			
			switch (c.ClassType) {
				case ClassType.Class:
					switch (c.FullyQualifiedName) {
						case "System.String":
							// return nothing
							break;
						case "System.Type":
							foreach (var item in CreateElementList(context, true, true))
								yield return item;
							break;
						case "System.Windows.PropertyPath":
							foreach (var item in CreatePropertyPathCompletion(context))
								yield return item;
							break;
						case "System.Windows.DependencyProperty":
							typeName = GetType(context, out isExplicit);
							
							bool isReadOnly = context.ActiveElement.Name.EndsWith("Trigger");
							
							if (!isExplicit && valueBeforeCaret.Contains("."))
								showFull = true;
							
							if (typeName != null) {
								foreach (var item in typeName.GetDependencyProperties(true, !isExplicit, !isReadOnly, showFull))
									yield return item;
							}
							break;
						case "System.Windows.RoutedEvent":
							typeName = GetType(context, out isExplicit);
							
							if (!isExplicit && valueBeforeCaret.Contains("."))
								showFull = true;
							
							if (typeName != null) {
								foreach (var item in typeName.GetRoutedEvents(true, !isExplicit, showFull))
									yield return item;
							}
							break;
						case "System.Windows.Media.FontFamily":
							foreach (var font in Fonts.SystemFontFamilies)
								yield return new SpecialValueCompletionItem(font.FamilyNames.First().Value);
							break;
						default:
							if (context.Description == XamlContextDescription.InMarkupExtension) {
								foreach (IField f in c.Fields)
									yield return new XamlCodeCompletionItem(textPrefix + f.Name, f);
								foreach (IProperty p in c.Properties.Where(pr => pr.IsPublic && pr.IsStatic && pr.CanGet))
									yield return new XamlCodeCompletionItem(textPrefix + p.Name, p);
							}
							break;
					}
					break;
				case ClassType.Enum:
					foreach (IField f in c.Fields)
						yield return new XamlCodeCompletionItem(textPrefix + f.Name, f);
					foreach (IProperty p in c.Properties.Where(pr => pr.IsPublic && pr.IsStatic && pr.CanGet))
						yield return new XamlCodeCompletionItem(textPrefix + p.Name, p);
					break;
				case ClassType.Struct:
					switch (c.FullyQualifiedName) {
						case "System.Boolean":
							yield return new SpecialValueCompletionItem("True");
							yield return new SpecialValueCompletionItem("False");
							break;
						case "System.Windows.GridLength":
							yield return new SpecialValueCompletionItem("Auto");
							yield return new SpecialValueCompletionItem("*");
							break;
					}
					break;
				case ClassType.Delegate:
					foreach (var item in CreateEventCompletion(context, c))
						yield return item;
					break;
			}
			
			var classes = c.ProjectContent.Classes.Where(
				cla => (cla.FullyQualifiedName == c.FullyQualifiedName + "s" ||
				        cla.FullyQualifiedName == c.FullyQualifiedName + "es"));
			foreach (var coll in classes) {
				foreach (var item in coll.Properties)
					yield return new SpecialValueCompletionItem(item.Name);
				foreach (var item in coll.Fields.Where(f => f.IsPublic && f.IsStatic && f.ReturnType.FullyQualifiedName == c.FullyQualifiedName))
					yield return new SpecialValueCompletionItem(item.Name);
			}
		}
		
		static IList<ICompletionItem> CreatePropertyPathCompletion(XamlCompletionContext context)
		{
			bool isExplicit;
			IReturnType typeName = GetType(context, out isExplicit);
			IList<ICompletionItem> list = new List<ICompletionItem>();
			
			string value = context.ValueStartOffset > -1 ? context.RawAttributeValue.Substring(0, Math.Min(context.ValueStartOffset + 1, context.RawAttributeValue.Length)) : "";
			
			if (value.EndsWithAny(']', ')'))
				return list;
			
			var segments = PropertyPathParser.Parse(value).ToList();
			
			int completionStart;
			bool isAtDot = false;
			
			IReturnType propertyPathType = ResolvePropertyPath(segments, context, typeName, out completionStart);
			if (completionStart < segments.Count) {
				PropertyPathSegment seg = segments[completionStart];
				switch (seg.Kind) {
					case SegmentKind.ControlChar:
						if (seg.Content == ".") {
							AddAttributes(propertyPathType, list, false);
							isAtDot = true;
						}
						break;
					case SegmentKind.AttachedProperty:
						AddAttributes(seg.Resolve(context, propertyPathType), list, false);
						isAtDot = seg.Content.Contains(".");
						break;
					case SegmentKind.PropertyOrType:
						AddAttributes(propertyPathType, list, false);
						isAtDot = true;
						break;
				}
			} else if (typeName != null) {
				AddAttributes(typeName, list, false);
			}
			
			if (!isAtDot) {
				foreach (var item in GetAllTypes(context))
					list.Add(item);
			}
			
			return list;
		}
		
		static IReturnType ResolvePropertyPath(IList<PropertyPathSegment> segments, XamlCompletionContext context, IReturnType parentType, out int lastIndex)
		{
			IReturnType type = parentType;
			
			for (lastIndex = 0; lastIndex < segments.Count - 1; lastIndex++) {
				PropertyPathSegment segment = segments[lastIndex];
				switch (segment.Kind) {
					case SegmentKind.AttachedProperty:
						// do we need to take account of previous results?
						type = segment.Resolve(context, null);
						break;
					case SegmentKind.ControlChar:
						if (segment.Content == "[" || segment.Content == "(" || segment.Content == "/")
							return null;
						return type;
					case SegmentKind.PropertyOrType:
						type = segment.Resolve(context, type);
						break;
					case SegmentKind.Indexer:
						if (type != null) {
							IProperty prop = type.GetProperties().FirstOrDefault(p => p.IsIndexer);
							if (prop != null) {
								type = prop.ReturnType;
							}
						}
						break;
					case SegmentKind.SourceTraversal:
						// ignore
						return null;
				}
			}
			
			return type;
		}
		
		static IReturnType Resolve(this PropertyPathSegment segment, XamlCompletionContext context, IReturnType previousType)
		{
			if (segment.Kind == SegmentKind.SourceTraversal)
				return previousType;
			if (segment.Kind == SegmentKind.ControlChar)
				return previousType;
			
			string content = segment.Content;
			
			if (segment.Kind == SegmentKind.AttachedProperty && content.StartsWith("(")) {
				content = content.TrimStart('(');
				if (content.Contains("."))
					content = content.Remove(content.IndexOf('.'));
			}
			
			XamlContextDescription tmp = context.Description;
			context.Description = XamlContextDescription.InTag;
			
			ResolveResult rr = XamlResolver.Resolve(content, context);
			IReturnType type = null;
			
			if (rr is TypeResolveResult)
				type = (rr as TypeResolveResult).ResolvedType;

			if (previousType != null) {
				IMember member = previousType.GetMemberByName(content);
				if (member != null)
					type = member.ReturnType;
			} else {
				if (rr is MemberResolveResult) {
					MemberResolveResult mrr = rr as MemberResolveResult;
					if (mrr.ResolvedMember != null)
						type = mrr.ResolvedMember.ReturnType;
				}
				if (rr is TypeResolveResult)
					type = (rr as TypeResolveResult).ResolvedType;
			}
			
			context.Description = tmp;
			return type;
		}
		
		static IMember GetMemberByName(this IReturnType type, string name)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			
			foreach (IMember member in type.GetFields()) {
				if (member.Name == name)
					return member;
			}
			
			foreach (IMember member in type.GetProperties()) {
				if (member.Name == name)
					return member;
			}
			
			return null;
		}
		
		static IEnumerable<ICompletionItem> CreateEventCompletion(XamlCompletionContext context, IClass c)
		{
			IMethod invoker = c.Methods.FirstOrDefault(method => method.Name == "Invoke");
			if (invoker != null && context.ActiveElement != null) {
				var item = context.ActiveElement;
				var evt = ResolveAttribute(context.Attribute.ToQualifiedName(), context) as IEvent;
				if (evt == null)
					return Enumerable.Empty<ICompletionItem>();
				
				int offset = XmlEditor.XmlParser.GetActiveElementStartIndex(context.Editor.Document.Text, context.Editor.Caret.Offset);
				
				if (offset == -1)
					return Enumerable.Empty<ICompletionItem>();
				
				var loc = context.Editor.Document.OffsetToPosition(offset);
				
				string name = context.ActiveElement.GetAttributeValue("Name");
				if (string.IsNullOrEmpty(name))
					name = context.ActiveElement.GetAttributeValue(XamlNamespace, "Name");
				
				IList<ICompletionItem> list = new List<ICompletionItem>();
				list.Add(new NewEventCompletionItem(evt, (string.IsNullOrEmpty(name) ? item.Name : name)));
				
				return CompletionDataHelper.AddMatchingEventHandlers(context, invoker).Concat(list);
			}
			
			return Enumerable.Empty<ICompletionItem>();
		}
		
		static IMember ResolveAttribute(QualifiedNameWithLocation attribute, XamlCompletionContext context)
		{
			if (attribute == null)
				return null;
			
			return ResolveAttribute(attribute.FullXmlName, context);
		}

		static IMember ResolveAttribute(string attribute, XamlCompletionContext context)
		{
			MemberResolveResult mrr = XamlResolver.Resolve(attribute, context) as MemberResolveResult;
			
			if (mrr == null)
				return null;
			
			return mrr.ResolvedMember;
		}

		static bool DoStaticExtensionCompletion(XamlCompletionItemList list, XamlCompletionContext context)
		{
			AttributeValue selItem = Utils.GetMarkupExtensionAtPosition(context.AttributeValue.ExtensionValue, context.ValueStartOffset)
				.PositionalArguments.LastOrDefault();
			if (context.PressedKey == '.') {
				if (selItem != null && selItem.IsString) {
					var rr = XamlResolver.Resolve(selItem.StringValue, context) as TypeResolveResult;
					if (rr != null)
						list.Items.AddRange(MemberCompletion(context, rr.ResolvedType, string.Empty));
					return false;
				}
			} else {
				if (selItem != null && selItem.IsString) {
					int index = selItem.StringValue.IndexOf('.');
					string s = (index > -1) ? selItem.StringValue.Substring(0, index) : selItem.StringValue;
					var rr = XamlResolver.Resolve(s, context) as TypeResolveResult;
					if (rr != null) {
						list.Items.AddRange(MemberCompletion(context, rr.ResolvedType, (index == -1) ? "." : string.Empty));
						
						list.PreselectionLength = (index > -1) ? selItem.StringValue.Length - index - 1 : 0;
						
						return false;
					} else
						DoStaticTypeCompletion(selItem, list, context);
				} else {
					DoStaticTypeCompletion(selItem, list, context);
				}
			}
			
			return true;
		}

		static void DoStaticTypeCompletion(AttributeValue selItem, XamlCompletionItemList list, XamlCompletionContext context)
		{
			var items = GetClassesFromContext(context);
			foreach (var ns in items) {
				list.Items.AddRange(ns.Value.Where(c => c.Fields.Any(f => f.IsStatic) || c.Properties.Any(p => p.IsStatic))
				                    .Select(c => new XamlCodeCompletionItem(c, ns.Key)));
			}
			if (selItem != null && selItem.IsString) {
				list.PreselectionLength = selItem.StringValue.Length;
			}
		}
		
		public static IReturnType ResolveType(string typeName, XamlContext context)
		{
			if (context.ParseInformation == null)
				return null;
			
			XamlCompilationUnit cu = context.ParseInformation.CompilationUnit as XamlCompilationUnit;
			if (cu == null)
				return null;
			string prefix = "";
			int len = typeName.IndexOf(':');
			string name = typeName;
			if (len > 0) {
				prefix = typeName.Substring(0, len);
				name = typeName.Substring(len + 1, name.Length - len - 1);
			}
			string namespaceName = "";
			if (context.XmlnsDefinitions.TryGetValue(prefix, out namespaceName)) {
				IReturnType rt = cu.CreateType(namespaceName, name);
				if (rt != null && rt.GetUnderlyingClass() != null)
					return rt;
			}
			return null;
		}

		public static IEnumerable<ICompletionItem> AddMatchingEventHandlers(XamlCompletionContext context, IMethod delegateInvoker)
		{
			if (context.ParseInformation == null)
				yield break;
			
			var unit = context.ParseInformation.CompilationUnit;
			var loc = context.Editor.Caret.Position;
			IClass c = unit.GetInnermostClass(loc.Line, loc.Column);
			if (c == null)
				yield break;
			CompoundClass compound = c.GetCompoundClass() as CompoundClass;
			if (compound != null) {
				foreach (IClass part in compound.Parts) {
					foreach (IMethod m in part.Methods) {
						if (m.Parameters.Count != delegateInvoker.Parameters.Count)
							continue;
						
						if ((m.ReturnType != null && delegateInvoker.ReturnType != null) && m.ReturnType.DotNetName != delegateInvoker.ReturnType.DotNetName)
							continue;
						
						bool equal = m.Parameters.SequenceEqual(delegateInvoker.Parameters, new ParameterComparer());
						if (equal) {
							yield return new XamlCodeCompletionItem(m);
						}
					}
				}
			}
		}

		public static bool Compare(this IParameter p1, IParameter p2)
		{
			return (p1.ReturnType.DotNetName == p2.ReturnType.DotNetName) &&
				(p1.IsOut == p2.IsOut) && (p1.IsParams == p2.IsParams) && (p1.IsRef == p2.IsRef);
		}

		static IDictionary<string, IEnumerable<IClass>> GetClassesFromContext(XamlCompletionContext context)
		{
			var result = new Dictionary<string, IEnumerable<IClass>>();
			
			if (context.ParseInformation == null)
				return result;
			
			IProjectContent pc = context.ProjectContent;
			
			foreach (var ns in context.XmlnsDefinitions) {
				result.Add(ns.Key, XamlCompilationUnit.GetNamespaceMembers(pc, ns.Value));
			}
			
			return result;
		}
		
		public static bool IsPubliclySetable(this IProperty thisValue)
		{
			return thisValue.CanSet &&
				(thisValue.SetterModifiers == ModifierEnum.None ||
				 (thisValue.SetterModifiers & ModifierEnum.Public) == ModifierEnum.Public);
		}
		
		public static IEnumerable<ICompletionItem> GetDependencyProperties(this IReturnType type, bool excludeSuffix, bool addType, bool requiresSetable, bool showFull)
		{
			foreach (var field in type.GetFields()) {
				if (field.ReturnType.FullyQualifiedName != "System.Windows.DependencyProperty")
					continue;
				if (field.Name.Length <= "Property".Length || !field.Name.EndsWith("Property", StringComparison.Ordinal))
					continue;
				string fieldName = field.Name.Remove(field.Name.Length - "Property".Length);
				IProperty property = type.GetProperties().FirstOrDefault(p => p.Name == fieldName);
				if (property == null)
					continue;
				if (requiresSetable && !property.IsPubliclySetable())
					continue;
				
				if (!excludeSuffix)
					fieldName = field.Name;
				
				if (showFull) {
					addType = false;
					
					fieldName = field.DeclaringType.Name + "." + fieldName;
				}
				
				yield return new XamlLazyValueCompletionItem(field, fieldName, addType);
			}
		}
		
		public static IEnumerable<ICompletionItem> GetRoutedEvents(this IReturnType type, bool excludeSuffix, bool addType, bool showFull)
		{
			foreach (var field in type.GetFields()) {
				if (field.ReturnType.FullyQualifiedName != "System.Windows.RoutedEvent")
					continue;
				if (field.Name.Length <= "Event".Length || !field.Name.EndsWith("Event", StringComparison.Ordinal))
					continue;
				string fieldName = field.Name.Remove(field.Name.Length - "Event".Length);
				if (!type.GetEvents().Any(p => p.Name == fieldName))
					continue;
				
				if (!excludeSuffix)
					fieldName = field.Name;
				
				if (showFull) {
					addType = false;
					
					fieldName = field.DeclaringType.Name + "." + fieldName;
				}
				
				yield return new XamlLazyValueCompletionItem(field, fieldName, addType);
			}
		}

		internal static List<ICompletionItem> GetListOfAttached(XamlCompletionContext context, string prefixClassName, string prefixNamespace, bool events, bool properties)
		{
			List<ICompletionItem> result = new List<ICompletionItem>();
			
			if (context.ParseInformation == null)
				return result;
			
			IProjectContent pc = context.ProjectContent;
			
			if (!string.IsNullOrEmpty(prefixClassName)) {
				var ns = context.XmlnsDefinitions[prefixNamespace];
				IClass c = XamlCompilationUnit.GetNamespaceMembers(pc, ns).FirstOrDefault(item => item.Name == prefixClassName);
				if (c != null && c.ClassType == ClassType.Class) {
					if (!c.ClassInheritanceTree.Any(b => b.FullyQualifiedName == "System.Attribute")) {
						prefixNamespace = string.IsNullOrEmpty(prefixNamespace) ? prefixNamespace : prefixNamespace + ":";
						if (properties)
							AddAttachedProperties(c, result, prefixNamespace, prefixNamespace + prefixClassName);
						if (events)
							AddAttachedEvents(c, result, prefixNamespace, prefixNamespace + prefixClassName);
					}
				}
			} else {
				foreach (var ns in context.XmlnsDefinitions) {
					string key = string.IsNullOrEmpty(ns.Key) ? "" : ns.Key + ":";
					
					foreach (IClass c in XamlCompilationUnit.GetNamespaceMembers(pc, ns.Value)) {
						if (c.ClassType != ClassType.Class)
							continue;
						if (c.HasAttached(properties, events))
							result.Add(new XamlCodeCompletionItem(c, ns.Key));
					}
				}
			}
			
			return result;
		}
		
		public static void AddAttachedProperties(IClass c, List<ICompletionItem> result, string key, string prefix)
		{
			if (c == null)
				return;
			
			var attachedProperties = c.Fields.Where(f => f.IsAttached(true, false));
			
			int prefixLength = (prefix.Length > 0) ? prefix.Length + 1 : 0;
			
			result.AddRange(
				attachedProperties.Select(
					item => {
						string property = item.Name.Remove(item.Name.Length - "Property".Length);
						string name = key + c.Name + "." + item.Name.Remove(item.Name.Length - "Property".Length);
						return new XamlCodeCompletionItem(name.Remove(0, prefixLength), new DefaultProperty(c, property) { ReturnType = GetAttachedPropertyType(item, c) });
					}
				)
			);
		}
		
		static void AddAttachedEvents(IClass c, List<ICompletionItem> result, string key, string prefix)
		{
			var attachedEvents = c.Fields.Where(f => f.IsAttached(false, true));
			
			int prefixLength = (prefix.Length > 0) ? prefix.Length + 1 : 0;
			
			result.AddRange(
				attachedEvents.Select(
					item => {
						string @event = item.Name.Remove(item.Name.Length - "Event".Length);
						string name = key + c.Name + "." + item.Name.Remove(item.Name.Length - "Event".Length);
						return new XamlCodeCompletionItem(name.Remove(0, prefixLength), 	new DefaultEvent(c, @event) { ReturnType = GetAttachedEventDelegateType(item, c) });
					}
				)
			);
		}

		static IReturnType GetAttachedEventDelegateType(IField field, IClass c)
		{
			if (c == null || field == null)
				return null;
			
			string eventName = field.Name.Remove(field.Name.Length - "Event".Length);
			
			IMethod method = c.Methods.FirstOrDefault(m => m.IsPublic && m.IsStatic && m.Parameters.Count == 2 && (m.Name == "Add" + eventName + "Handler" || m.Name == "Remove" + eventName + "Handler"));
			
			if (method == null)
				return null;
			
			return method.Parameters[1].ReturnType;
		}

		static IReturnType GetAttachedPropertyType(IField field, IClass c)
		{
			if (c == null || field == null)
				return null;
			
			string propertyName = field.Name.Remove(field.Name.Length - "Property".Length);
			
			IMethod method = c.Methods.FirstOrDefault(m => m.IsPublic && m.IsStatic && m.Name == "Get" + propertyName);
			
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
