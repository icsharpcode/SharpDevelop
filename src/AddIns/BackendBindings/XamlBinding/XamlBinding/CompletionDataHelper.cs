// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using ICSharpCode.XmlEditor;

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
			new SpecialCompletionItem("xmlns:")
		};
		
		public static readonly ReadOnlyCollection<string> XamlNamespaceAttributes = new List<string> {
			"Class", "ClassModifier", "FieldModifier", "Name", "Subclass", "TypeArguments", "Uid", "Key"
		}.AsReadOnly();
		
		public static readonly ReadOnlyCollection<string> RootOnlyElements = new List<string> {
			"Class", "ClassModifier", "Subclass"
		}.AsReadOnly();
		
		static readonly List<ICompletionItem> emptyList = new List<ICompletionItem>();
		
		public const string XamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml";
		public const string WpfXamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
		public const string MarkupCompatibilityNamespace = "http://schemas.openxmlformats.org/markup-compatibility/2006";
		
		public const bool EnableXaml2009 = true;
		
		public static XamlContext ResolveContext(string text, string fileName, int line, int col)
		{
			int offset = Math.Max(0, Math.Min(Utils.GetOffsetFromFilePos(text, line, col), text.Length - 1));
			
			ParseInformation info = string.IsNullOrEmpty(fileName) ? null : ParserService.GetParseInformation(fileName);
			string attribute = XmlParser.GetAttributeNameAtIndex(text, offset);
			bool inAttributeValue = XmlParser.IsInsideAttributeValue(text, offset);
			string attributeValue = XmlParser.GetAttributeValueAtIndex(text, offset);
			int offsetFromValueStart = Utils.GetOffsetFromValueStart(text, offset);
			
			AttributeValue value = MarkupExtensionParser.ParseValue(attributeValue);
			XamlContextDescription description = XamlContextDescription.None;
			
			var lookUpInfo = Utils.LookupInfoAtTarget(text, line, col, offset);
			
			string wordBeforeIndex = text.GetWordBeforeOffset(offset);
			
			if (lookUpInfo.Active != null && lookUpInfo.Parent != lookUpInfo.Active)
				description = XamlContextDescription.AtTag;
			
			if (lookUpInfo.ActiveElementStartIndex > -1 &&
			    (char.IsWhiteSpace(text[offset]) || !string.IsNullOrEmpty(attribute) ||
			     Extensions.Is(text[offset], '"', '\'') || !wordBeforeIndex.StartsWith("<", StringComparison.OrdinalIgnoreCase)))
				description = XamlContextDescription.InTag;

			if (inAttributeValue) {
				description = XamlContextDescription.InAttributeValue;

				if (value != null && !value.IsString)
					description = XamlContextDescription.InMarkupExtension;

				if (attributeValue.StartsWith("{}", StringComparison.Ordinal) && attributeValue.Length > 2)
					description = XamlContextDescription.InAttributeValue;
			}
			
			if (Utils.IsInsideXmlComment(text, offset))
				description = XamlContextDescription.InComment;
			
			int prefixEnd = attribute.IndexOf(':');
			
			string prefix = "";
			string xmlNamespace = "";
			string localName = attribute;
			
			if (prefixEnd > -1) {
				prefix = attribute.Substring(0, prefixEnd);
				localName = attribute.Substring(prefixEnd + 1, attribute.Length - prefix.Length - 1);
			}
			
			lookUpInfo.XmlnsDefinitions.TryGetValue(prefix, out xmlNamespace);
			
			var qAttribute = new QualifiedNameWithLocation(localName, xmlNamespace, prefix, -1, -1);
			
			var context = new XamlContext() {
				Description       = description,
				ActiveElement     = lookUpInfo.Active,
				ParentElement     = lookUpInfo.Parent,
				Ancestors         = lookUpInfo.Ancestors.ToList(),
				AttributeName     = string.IsNullOrEmpty(attribute) ? null : qAttribute,
				InRoot            = lookUpInfo.IsRoot,
				AttributeValue    = value,
				RawAttributeValue = attributeValue,
				ValueStartOffset  = offsetFromValueStart,
				XmlnsDefinitions  = lookUpInfo.XmlnsDefinitions,
				ParseInformation  = info,
				IgnoredXmlns      = lookUpInfo.IgnoredXmlns.AsReadOnly()
			};

			return context;
		}
		
		public static XamlCompletionContext ResolveCompletionContext(ITextEditor editor, char typedValue)
		{
			var context = new XamlCompletionContext(ResolveContext(editor.Document.Text, editor.FileName, editor.Caret.Line, editor.Caret.Column)) {
				PressedKey = typedValue,
				Editor = editor
			};
			
			return context;
		}
		
		static List<ICompletionItem> CreateAttributeList(XamlCompletionContext context, bool includeEvents)
		{
			QualifiedNameWithLocation lastElement = context.ActiveElement;
			if (context.ParseInformation == null)
				return emptyList;
			XamlCompilationUnit cu = context.ParseInformation.BestCompilationUnit as XamlCompilationUnit;
			if (cu == null)
				return emptyList;
			IReturnType rt = cu.CreateType(lastElement.Namespace, lastElement.Name.Trim('.'));
			
			var list = new List<ICompletionItem>();
			
			string xamlPrefix = Utils.GetXamlNamespacePrefix(context);
			string xKey = string.IsNullOrEmpty(xamlPrefix) ? "" : xamlPrefix + ":";
			
			if (xamlBuiltInTypes.Concat(XamlNamespaceAttributes).Select(s => xKey + s).Contains(lastElement.FullXmlName))
				return emptyList;
			
			if (lastElement.Name.EndsWith(".", StringComparison.OrdinalIgnoreCase) || context.PressedKey == '.') {
				if (context.ParentElement.Name.StartsWith(lastElement.Name.TrimEnd('.'), StringComparison.OrdinalIgnoreCase))
					AddAttributes(rt, list, includeEvents);
				else if (rt != null && rt.GetUnderlyingClass() != null) {
					string key = string.IsNullOrEmpty(lastElement.Prefix) ? "" : lastElement.Prefix + ":";

					AddAttachedProperties(rt.GetUnderlyingClass(), list, key, lastElement.Name.Trim('.'));
				}
			} else {
				if (rt == null) {
					list.Add(new XamlCompletionItem(xamlPrefix, XamlNamespace, "Uid"));
					return list;
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
				if (p.IsPublic && (p.IsPubliclySetable() || p.ReturnType.IsCollectionReturnType())) {
					list.Add(new XamlCodeCompletionItem(p));
				}
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
			return inRoot || !RootOnlyElements.Contains(item);
		}
		
		public static IEnumerable<ICompletionItem> CreateListForXmlnsCompletion(IProjectContent projectContent)
		{
			List<XmlnsCompletionItem> list = new List<XmlnsCompletionItem>();
			
			foreach (IProjectContent content in projectContent.ReferencedContents) {
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
			
			return list
				.Distinct(new XmlnsEqualityComparer())
				.OrderBy(item => item, new XmlnsComparer())
				.Cast<ICompletionItem>();
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

			XamlCompilationUnit cu = context.ParseInformation.BestCompilationUnit as XamlCompilationUnit;
			
			IReturnType rt = null;

			if (last != null && cu != null) {
				if (!last.Name.Contains(".") || last.Name.EndsWith(".", StringComparison.OrdinalIgnoreCase)) {
					rt = cu.CreateType(last.Namespace, last.Name.Trim('.'));
					string contentPropertyName = GetContentPropertyName(rt);
					if (!string.IsNullOrEmpty(contentPropertyName)) {
						string fullName = string.IsNullOrEmpty(last.Prefix) ? last.Name + "." + contentPropertyName : last.Prefix + ":" + last.Name + "." + contentPropertyName;
						MemberResolveResult mrr = XamlResolver.Resolve(fullName, context) as MemberResolveResult;
						
						if (mrr != null) {
							rt = mrr.ResolvedType;
						}
					}
				} else {
					string fullName = string.IsNullOrEmpty(last.Prefix) ? last.Name : last.Prefix + ":" + last.Name;
					MemberResolveResult mrr = XamlResolver.Resolve(fullName, context) as MemberResolveResult;
					
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
							if (!(!c.IsStatic && !c.DerivesFrom("System.Attribute") && c.Methods.Any(m => m.IsConstructor && m.IsPublic)))
								continue;
						} else if (c.ClassType == ClassType.Interface) {
						} else {
							continue;
						}
					} else {
						if (!(c.ClassType == ClassType.Class && c.IsAbstract == includeAbstract && !c.IsStatic &&
						      !c.DerivesFrom("System.Attribute") && c.Methods.Any(m => m.IsConstructor && m.IsPublic)))
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
					
					if (item.Text == last.FullXmlName)
						parentAdded = true;
					
					result.Add(new XamlCodeCompletionItem(c, ns.Key));
				}
			}
			
			if (!parentAdded && !last.FullXmlName.Contains("."))
				result.Add(new XamlCodeCompletionItem(cu.CreateType(last.Namespace, last.Name.Trim('.')).GetUnderlyingClass(), last.Prefix));
			
			string xamlPrefix = Utils.GetXamlNamespacePrefix(context);
			
			var xamlItems = XamlNamespaceAttributes.AsEnumerable();
			
			if (EnableXaml2009)
				xamlItems = xamlBuiltInTypes.Concat(XamlNamespaceAttributes);
			
			foreach (string item in xamlItems)
				result.Add(new XamlCompletionItem(xamlPrefix, XamlNamespace, item));
			
			return result;
		}

		public static IEnumerable<ICompletionItem> CreateListOfMarkupExtensions(XamlCompletionContext context)
		{
			var list = CreateElementList(context, true, false);
			
			var neededItems = list.OfType<XamlCodeCompletionItem>()
				.Where(i => (i.Entity as IClass).DerivesFrom("System.Windows.Markup.MarkupExtension"));
			neededItems
				.ForEach(
					selItem => {
						var it = selItem as XamlCodeCompletionItem;
						string text = it.Text;
						if (it.Text.EndsWith("Extension", StringComparison.Ordinal))
							text = text.Remove(it.Text.Length - "Extension".Length);
						it.Text = text;
					}
				);
			
			return neededItems.Cast<ICompletionItem>().Add(new XamlCompletionItem(Utils.GetXamlNamespacePrefix(context), XamlNamespace, "Reference"));
		}

		public static XamlCompletionItemList CreateListForContext(XamlCompletionContext context)
		{
			XamlCompletionItemList list = new XamlCompletionItemList();
			
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
					string word = context.Editor.GetWordBeforeCaretExtended();
					
					if (context.PressedKey == '.' || word.Contains(".")) {
						string ns = "";
						int pos = word.IndexOf(':');
						if (pos > -1)
							ns = word.Substring(0, pos);
						
						string element = word.Substring(pos + 1, word.Length - pos - 1);
						string className = element;
						int propertyStart = element.IndexOf('.');
						if (propertyStart != -1)
							className = element.Substring(0, propertyStart).TrimEnd('.');
						TypeResolveResult trr = new XamlResolver().Resolve(new ExpressionResult(className, context), info, editor.Document.Text) as TypeResolveResult;
						IClass typeClass = (trr != null && trr.ResolvedType != null) ? trr.ResolvedType.GetUnderlyingClass() : null;
						
						if (typeClass != null && typeClass.HasAttached(true, true))
							list.Items.AddRange(GetListOfAttached(context, className, ns, true, true));
					} else {
						QualifiedNameWithLocation last = context.ActiveElement;
						TypeResolveResult trr = new XamlResolver().Resolve(new ExpressionResult(last.Name, context), info, editor.Document.Text) as TypeResolveResult;
						IClass typeClass = (trr != null && trr.ResolvedType != null) ? trr.ResolvedType.GetUnderlyingClass() : null;
						list.Items.AddRange(CreateAttributeList(context, true));
						list.Items.AddRange(standardAttributes);
					}
					break;
				case XamlContextDescription.InAttributeValue:
					XamlCodeCompletionBinding.Instance.CtrlSpace(editor);
					break;
			}
			
			list.SortItems();
			
			return list;
		}
		
		static void AddClosingTagCompletion(XamlCompletionContext context, XamlCompletionItemList list)
		{
			if (context.ParentElement != null && !context.InRoot) {
				ResolveResult rr = XamlResolver.Resolve(context.ParentElement.FullXmlName, context);
				TypeResolveResult trr = rr as TypeResolveResult;
				MemberResolveResult mrr = rr as MemberResolveResult;

				if (trr != null) {
					if (trr.ResolvedClass == null)
						return;
					list.Items.Add(new XamlCodeCompletionItem("/" + context.ParentElement.FullXmlName, trr.ResolvedClass));
				} else if (mrr != null) {
					if (mrr.ResolvedMember == null)
						return;
					list.Items.Add(new XamlCodeCompletionItem("/" + context.ParentElement.FullXmlName, mrr.ResolvedMember));
				}
			}
		}

		public static IEnumerable<IInsightItem> CreateMarkupExtensionInsight(XamlCompletionContext context)
		{
			var markup = Utils.GetMarkupExtensionAtPosition(context.AttributeValue.ExtensionValue, context.Editor.Caret.Offset);
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
			var list = new XamlCompletionItemList();
			string visibleValue = context.RawAttributeValue.Substring(0, context.ValueStartOffset + 1);
			if (context.PressedKey == '=')
				visibleValue += "=";
			context.RawAttributeValue = visibleValue;
			context.AttributeValue = MarkupExtensionParser.ParseValue(visibleValue);
			var markup = Utils.GetInnermostMarkupExtensionInfo(context.AttributeValue.ExtensionValue);
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
			
			list.Items.AddRange(type.GetProperties().Where(p => p.CanSet && p.IsPublic).Select(p => new XamlCodeCompletionItem(p.Name + "=", p)).Cast<ICompletionItem>());
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
						list.Items.AddRange(CreateElementList(context, true, true));
						AttributeValue selItem = Utils.GetInnermostMarkupExtensionInfo(context.AttributeValue.ExtensionValue)
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
			var loc = context.ParentElement.Location;
			
			IList<QualifiedNameWithLocation> ancestors = context.Ancestors;
			
			isExplicit = false;
			
			for (int i = 0; i < ancestors.Count; i++) {
				if (ancestors[i].Name == "Style") {
					isExplicit = true;
					return Utils.GetAttributeValue(
						context.Editor.Document.Text,
						ancestors[i].Location.Line,
						ancestors[i].Location.Column,
						"TargetType");
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
				string prefix = Utils.GetXamlNamespacePrefix(context);
				if (!string.IsNullOrEmpty(prefix))
					nullExtensionName = prefix + ":" + nullExtensionName;
				yield return new SpecialCompletionItem("{" + nullExtensionName + "}");
				c = rt.TypeArguments.First().GetUnderlyingClass();
				if (c == null)
					yield break;
			}
			
			bool isExplicit, showFull = false;
			IReturnType typeName;
			
			string valueBeforeCaret = (context.ValueStartOffset > 0) ? context.RawAttributeValue.Substring(0, context.ValueStartOffset + 1) : "";
			
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
							break;
						case "System.Windows.DependencyProperty":
							typeName = GetType(context, out isExplicit);
							
							bool isReadOnly = context.ActiveElement.Name.EndsWith("Trigger");
							
							Core.LoggingService.Debug("value: " + valueBeforeCaret);

							if (!isExplicit && valueBeforeCaret.Contains("."))
								showFull = true;
							
							if (typeName != null) {
								foreach (var item in typeName.GetDependencyProperties(true, !isExplicit, !isReadOnly, showFull))
									yield return item;
							}
							break;
						case "System.Windows.RoutedEvent":
							typeName = GetType(context, out isExplicit);
							
							Core.LoggingService.Debug("value: " + valueBeforeCaret);
							
							if (!isExplicit && valueBeforeCaret.Contains("."))
								showFull = true;
							
							if (typeName != null) {
								foreach (var item in typeName.GetRoutedEvents(true, !isExplicit, showFull))
									yield return item;
							}
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
		
		static IEnumerable<ICompletionItem> CreateEventCompletion(XamlCompletionContext context, IClass c)
		{
			IMethod invoker = c.Methods.Where(method => method.Name == "Invoke").FirstOrDefault();
			if (invoker != null && context.ActiveElement != null) {
				var item = context.ActiveElement;
				var evt = ResolveAttribute(context.AttributeName, context) as IEvent;
				if (evt == null)
					return Enumerable.Empty<ICompletionItem>();
				
				int offset = XmlParser.GetActiveElementStartIndex(context.Editor.Document.Text, context.Editor.Caret.Offset);
				
				if (offset == -1)
					return Enumerable.Empty<ICompletionItem>();
				
				var loc = context.Editor.Document.OffsetToPosition(offset);
				
				string prefix = Utils.GetXamlNamespacePrefix(context);
				string name = Utils.GetAttributeValue(context.Editor.Document.Text, loc.Line, loc.Column + 1, "name");
				if (string.IsNullOrEmpty(name))
					name = Utils.GetAttributeValue(context.Editor.Document.Text, loc.Line, loc.Column + 1, (string.IsNullOrEmpty(prefix) ? "" : prefix + ":") + "name");
				
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
			AttributeValue selItem = Utils.GetInnermostMarkupExtensionInfo(context.AttributeValue.ExtensionValue)
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
				                    .Select(c => new XamlCodeCompletionItem(c, ns.Key))
				                    .Cast<ICompletionItem>());
			}
			if (selItem != null && selItem.IsString) {
				list.PreselectionLength = selItem.StringValue.Length;
			}
		}
		
		public static IReturnType ResolveType(string typeName, XamlContext context)
		{
			if (context.ParseInformation == null)
				return null;
			
			XamlCompilationUnit cu = context.ParseInformation.BestCompilationUnit as XamlCompilationUnit;
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
			
			var unit = context.ParseInformation.MostRecentCompilationUnit;
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
			
			IProjectContent pc = context.ParseInformation.BestCompilationUnit.ProjectContent;
			
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
			
			IProjectContent pc = context.ParseInformation.BestCompilationUnit.ProjectContent;
			
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
			
			int prefixLength = (prefix.Length > 0) ? prefix.Length + 1 : 0;
			
			result.AddRange(
				attachedProperties.Select(
					item => {
						string property = item.Name.Remove(item.Name.Length - "Property".Length);
						string name = key + c.Name + "." + item.Name.Remove(item.Name.Length - "Property".Length);
						return new XamlCodeCompletionItem(name.Remove(0, prefixLength), new DefaultProperty(c, property) { ReturnType = GetAttachedPropertyType(item, c) });
					}
				)
				.Cast<ICompletionItem>()
			);
		}
		
		static void AddAttachedEvents(IClass c, List<ICompletionItem> result, string key, string prefix)
		{
			var attachedEvents = c.Fields
				.Where(f =>
				       f.IsPublic &&
				       f.IsStatic &&
				       f.IsReadonly &&
				       f.ReturnType != null &&
				       f.ReturnType.FullyQualifiedName == "System.Windows.RoutedEvent" &&
				       f.Name.Length > "Event".Length &&
				       f.Name.EndsWith("Event", StringComparison.Ordinal)
				      );
			
			int prefixLength = (prefix.Length > 0) ? prefix.Length + 1 : 0;
			
			result.AddRange(
				attachedEvents.Select(
					item => {
						string @event = item.Name.Remove(item.Name.Length - "Event".Length);
						string name = key + c.Name + "." + item.Name.Remove(item.Name.Length - "Event".Length);
						return new XamlCodeCompletionItem(name.Remove(0, prefixLength), 	new DefaultEvent(c, @event) { ReturnType = GetAttachedEventDelegateType(item, c) });
					}
				)
				.Cast<ICompletionItem>()
			);
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