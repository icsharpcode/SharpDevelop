// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.AvalonEdit.Document;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.XmlEditor;
using LoggingService = ICSharpCode.Core.LoggingService;

namespace ICSharpCode.XamlBinding
{
	public static class CompletionDataHelper
	{
		#region Pre-defined lists
		static readonly List<ICompletionItem> standardElements = new List<ICompletionItem> {
			new SpecialCompletionItem("!--"),
			new SpecialCompletionItem("![CDATA["),
			new SpecialCompletionItem("?")
		};
		
		static readonly List<ICompletionItem> standardAttributes = new List<ICompletionItem> {
			new SpecialCompletionItem("xmlns:")
		};
		
		public static readonly List<string> XamlNamespaceAttributes = new List<string> {
			"Class", "ClassModifier", "FieldModifier", "Name", "Subclass", "TypeArguments", "Uid", "Key"
		};
		
		public static readonly List<string> RootOnlyElements = new List<string> {
			"Class", "ClassModifier", "Subclass"
		};
		
		static readonly List<ICompletionItem> emptyList = new List<ICompletionItem>();
		#endregion
		
		public const string XamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml";
		public const string WpfXamlNamespace = "http://schemas.microsoft.com/netfx/2007/xaml/presentation";
		
		public static XamlContext ResolveContext(string text, string fileName, int line, int col)
		{
			int offset = Math.Max(0, Math.Min(Utils.GetOffsetFromFilePos(text, line, col), text.Length - 1));
			
			ParseInformation info = ParserService.GetParseInformation(fileName);
			string attribute = XmlParser.GetAttributeNameAtIndex(text, offset);
			bool inAttributeValue = XmlParser.IsInsideAttributeValue(text, offset);
			string attributeValue = XmlParser.GetAttributeValueAtIndex(text, offset);
			int offsetFromValueStart = Utils.GetOffsetFromValueStart(text, offset);
			
			AttributeValue value = MarkupExtensionParser.ParseValue(attributeValue);
			XamlContextDescription description = XamlContextDescription.None;
			
			Dictionary<string, string> xmlnsDefs;
			QualifiedName active, parent;
			int elementStartIndex;
			bool isRoot;
			
			Utils.LookUpInfoAtTarget(text, line, col, offset, out xmlnsDefs, out active, out parent, out elementStartIndex, out isRoot);
			
			string wordBeforeIndex = text.GetWordBeforeOffset(offset);
			
			if (active != null && parent != active)
				description = XamlContextDescription.AtTag;
			
			if (elementStartIndex > -1 &&
			    (char.IsWhiteSpace(text[offset]) || !string.IsNullOrEmpty(attribute) ||
			     Extensions.Is(text[offset], '"', '\'') || !wordBeforeIndex.StartsWith("<")))
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
			
			var context = new XamlContext() {
				Description = description,
				ActiveElement = active,
				ParentElement = parent,
				AttributeName = attribute,
				InRoot = isRoot,
				AttributeValue = value,
				RawAttributeValue = attributeValue,
				ValueStartOffset = offsetFromValueStart,
				XmlnsDefinitions = xmlnsDefs,
				ParseInformation = info
			};

			return context;
		}
		
		public static QualifiedName ResolveCurrentElement(string text, int offset, Dictionary<string, string> xmlnsDefinitions)
		{
			if (offset < 0)
				return null;
			
			string elementName = text.GetWordAfterOffset(offset + 1).Trim('>', '<');
			
			string prefix = "";
			string element = "";
			
			if (elementName.IndexOf(':') > -1) {
				string[] data = elementName.Split(':');
				prefix = data[0];
				element = data[1];
			} else {
				element = elementName;
			}
			
			string xmlns = xmlnsDefinitions.ContainsKey(prefix) ? xmlnsDefinitions[prefix] : string.Empty;
			
			return new QualifiedName(element, xmlns, prefix);
		}
		
		public static XamlCompletionContext ResolveCompletionContext(ITextEditor editor, char typedValue)
		{
			var context = new XamlCompletionContext(ResolveContext(editor.Document.Text, editor.FileName, editor.Caret.Line, editor.Caret.Column)) {
				PressedKey = typedValue,
				Editor = editor
			};
			
			return context;
		}
		
		static List<ICompletionItem> CreateAttributeList(XamlCompletionContext context, string[] existingItems, bool includeEvents)
		{
			QualifiedName lastElement = context.ActiveElement;
			if (context.ParseInformation == null)
				return emptyList;
			XamlCompilationUnit cu = context.ParseInformation.BestCompilationUnit as XamlCompilationUnit;
			if (cu == null)
				return emptyList;
			IReturnType rt = cu.CreateType(lastElement.Namespace, lastElement.Name.Trim('.'));
			
			var list = new List<ICompletionItem>();
			
			string xamlPrefix = Utils.GetXamlNamespacePrefix(context);
			
			if (rt == null) {
				list.Add(new XamlCompletionItem(xamlPrefix, XamlNamespace, "Uid"));
				return list;
			} else {
				foreach (string item in XamlNamespaceAttributes.Where(item => AllowedInElement(context.InRoot, item))) {
					if (!existingItems.Contains(xamlPrefix + ":" + item)) {
						list.Add(new XamlCompletionItem(xamlPrefix, XamlNamespace, item));
					}
				}
			}
			
			foreach (IProperty p in rt.GetProperties()) {
				if (p.IsPublic && (p.CanSet || p.ReturnType.IsCollectionReturnType()) && !existingItems.Contains(p.Name)) {
					list.Add(new XamlCodeCompletionItem(p));
				}
			}
			
			if (includeEvents) {
				foreach (IEvent e in rt.GetEvents()) {
					if (e.IsPublic && !existingItems.Contains(e.Name)) {
						list.Add(new XamlCodeCompletionItem(e));
					}
				}
			}
			
			if (!lastElement.Name.EndsWith(".", StringComparison.OrdinalIgnoreCase) && context.PressedKey != '.')
				list.AddRange(GetListOfAttached(context, string.Empty, string.Empty, includeEvents, true));
			
			return list;
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
		
		sealed class XmlnsEqualityComparer : IEqualityComparer<XmlnsCompletionItem> {
			public bool Equals(XmlnsCompletionItem x, XmlnsCompletionItem y)
			{
				return x.Namespace == y.Namespace && x.Assembly == y.Assembly;
			}
			
			public int GetHashCode(XmlnsCompletionItem obj)
			{
				return string.IsNullOrEmpty(obj.Assembly) ? obj.Namespace.GetHashCode() : obj.Namespace.GetHashCode() ^ obj.Assembly.GetHashCode();
			}
		}
		
		sealed class XmlnsComparer : IComparer<XmlnsCompletionItem> {
			public int Compare(XmlnsCompletionItem x, XmlnsCompletionItem y)
			{
				if (x.IsUrl && y.IsUrl)
					return string.CompareOrdinal(x.Namespace, y.Namespace);
				if (x.IsUrl)
					return -1;
				if (y.IsUrl)
					return 1;
				if (x.Assembly == y.Assembly)
					return string.CompareOrdinal(x.Namespace, y.Namespace);
				else
					return string.CompareOrdinal(x.Assembly, y.Assembly);
			}
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
		
		public static IList<ICompletionItem> CreateElementList(XamlCompletionContext context, bool classesOnly)
		{
			var items = GetClassesFromContext(context);
			var result = new List<ICompletionItem>();

			var last = context.ParentElement;
			
			if (context.ParseInformation == null)
				return emptyList;

			XamlCompilationUnit cu = context.ParseInformation.BestCompilationUnit as XamlCompilationUnit;
			
			IReturnType rt = null;
			IReturnType elementReturnType = null;
			
			bool isMember = false;
			bool inContentRoot = false;
			
			if (last != null && cu != null) {
				if (!last.Name.Contains(".") || last.Name.EndsWith(".")) {
					elementReturnType = rt = cu.CreateType(last.Namespace, last.Name.Trim('.'));
					string contentPropertyName = GetContentPropertyName(rt);
					if (!string.IsNullOrEmpty(contentPropertyName)) {
						string fullName = string.IsNullOrEmpty(last.Prefix) ? last.Name + "." + contentPropertyName : last.Prefix + ":" + last.Name + "." + contentPropertyName;
						MemberResolveResult mrr = XamlResolver.Resolve(fullName, context) as MemberResolveResult;
						
						if (mrr != null) {
							rt = mrr.ResolvedType;
							isMember = true;
						}
						
						inContentRoot = true;
					}
				} else {
					string fullName = string.IsNullOrEmpty(last.Prefix) ? last.Name : last.Prefix + ":" + last.Name;
					MemberResolveResult mrr = XamlResolver.Resolve(fullName, context) as MemberResolveResult;
					
					if (mrr != null) {
						rt = mrr.ResolvedType;
						isMember = true;
					}
				}
			}
			
			bool isList = rt != null && rt.IsListReturnType();
			
			foreach (var ns in items) {
				foreach (var c in ns.Value) {
					if (!(c.ClassType == ClassType.Class && !c.IsAbstract && !c.IsStatic &&
					      !c.DerivesFrom("System.Attribute") &&
					      c.Methods.Any(m => m.IsConstructor && m.IsPublic)))
						continue;
					
					if (last != null && isList) {
						var possibleTypes = rt.GetMethods()
							.Where(a => a.Parameters.Count == 1 && a.Name == "Add")
							.Select(method => method.Parameters.First().ReturnType.GetUnderlyingClass());
						
						if (!possibleTypes.Any(t => c.ClassInheritanceTreeClassesOnly.Any(c2 => c2.FullyQualifiedName == t.FullyQualifiedName)))
							continue;
					}
					
					result.Add(new XamlCodeCompletionItem(c, ns.Key));
				}
			}
			
			if (!(rt == null || isMember || classesOnly)) {
				foreach (IProperty p in rt.GetProperties()) {
					if (p.IsPublic && (p.CanSet || p.ReturnType.IsCollectionReturnType()))
						result.Add(new XamlCodeCompletionItem(p, last.Prefix, last.Name));
				}
			} else if (elementReturnType != null && inContentRoot) {
				foreach (IProperty p in elementReturnType.GetProperties()) {
					if (p.IsPublic && (p.CanSet || p.ReturnType.IsCollectionReturnType()))
						result.Add(new XamlCodeCompletionItem(p, last.Prefix, last.Name));
				}
			}
			
			return result;
		}

		public static IList<ICompletionItem> CreateListOfMarkupExtensions(XamlCompletionContext context)
		{
			var list = CreateElementList(context, true);
			
			var neededItems = list
				.Where(i => ((i as XamlCodeCompletionItem).Entity as IClass).DerivesFrom("System.Windows.Markup.MarkupExtension"));
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
			
			return neededItems.ToList();
		}

		public static ICompletionItemList CreateListForContext(XamlCompletionContext context)
		{
			XamlCompletionItemList list = new XamlCompletionItemList();
			
			ParseInformation info = context.ParseInformation;
			ITextEditor editor = context.Editor;
			
			switch (context.Description) {
				case XamlContextDescription.None:
					if (context.Forced) {
						list.Items.AddRange(standardElements);
						list.Items.AddRange(CreateElementList(context, false));
					}
					break;
				case XamlContextDescription.AtTag:
					if ((editor.Caret.Offset > 0 && editor.Document.GetCharAt(editor.Caret.Offset - 1) == '.') || context.PressedKey == '.') {
						var loc = editor.Document.OffsetToPosition(Utils.GetParentElementStart(editor));
						var existing = Utils.GetListOfExistingAttributeNames(editor.Document.Text, loc.Line, loc.Column);
						list.Items.AddRange(CreateAttributeList(context, existing, false));
					} else {
						list.Items.AddRange(standardElements);
						list.Items.AddRange(CreateElementList(context, false));
					}
					break;
				case XamlContextDescription.InTag:
					var existingAttribs = Utils.GetListOfExistingAttributeNames(editor.Document.Text, editor.Caret.Line, editor.Caret.Column);
					string word = context.Editor.GetWordBeforeCaretExtended();
					
					if (context.PressedKey == '.') {
						TypeResolveResult trr = new XamlResolver().Resolve(new ExpressionResult(word.TrimEnd('.'), context), info, editor.Document.Text) as TypeResolveResult;
						IClass typeClass = (trr != null && trr.ResolvedType != null) ? trr.ResolvedType.GetUnderlyingClass() : null;
						
						string ns = "";
						int pos = word.IndexOf(':');
						if (pos > -1)
							ns = word.Substring(0, pos);
						
						if (typeClass != null && typeClass.DerivesFrom("System.Windows.DependencyObject"))
							list.Items.AddRange(GetListOfAttached(context, word.Substring(pos + 1, word.Length - pos - 1).TrimEnd('.'), ns, true, true));
					} else {
						QualifiedName last = context.ActiveElement;
						TypeResolveResult trr = new XamlResolver().Resolve(new ExpressionResult(last.Name, context), info, editor.Document.Text) as TypeResolveResult;
						IClass typeClass = (trr != null && trr.ResolvedType != null) ? trr.ResolvedType.GetUnderlyingClass() : null;
						
						list.Items.AddRange(CreateAttributeList(context, existingAttribs, true));
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
			var markup = Utils.GetInnermostMarkupExtensionInfo(MarkupExtensionParser.Parse(visibleValue));
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
			if (markup.NamedArguments.Count > 0 && !context.Editor.GetWordBeforeCaret().StartsWith(",")) {
				int lastStart = markup.NamedArguments.Max(i => i.Value.StartOffset);
				var item = markup.NamedArguments.First(p => p.Value.StartOffset == lastStart);
				
				if (context.Editor.Document.GetCharAt(context.Editor.Caret.Offset - 1) == '=' ||
				    (item.Value.IsString && item.Value.StringValue.EndsWith(context.Editor.GetWordBeforeCaretExtended()))) {
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
						list.Items.AddRange(CreateElementList(context, true));
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

		public static IEnumerable<ICompletionItem> MemberCompletion(XamlCompletionContext context, IReturnType type, string textPrefix)
		{
			if (type == null || type.GetUnderlyingClass() == null)
				yield break;
			
			var c = type.GetUnderlyingClass();
			
			switch (c.ClassType) {
				case ClassType.Class:
					if (c.FullyQualifiedName == "System.String") {
						// return nothing
					} else if (c.FullyQualifiedName == "System.Type") {
						foreach (var item in CreateElementList(context, true))
							yield return item;
					} else {
						if (context.Description == XamlContextDescription.InMarkupExtension) {
							foreach (IField f in c.Fields)
								yield return new XamlCodeCompletionItem(textPrefix + f.Name, f);
							foreach (IProperty p in c.Properties.Where(pr => pr.IsPublic && pr.IsStatic && pr.CanGet))
								yield return new XamlCodeCompletionItem(textPrefix + p.Name, p);
						}
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
						
						bool equal = true;
						for (int i = 0; i < m.Parameters.Count; i++) {
							equal &= CompareParameter(m.Parameters[i], delegateInvoker.Parameters[i]);
							if (!equal)
								break;
						}
						if (equal) {
							yield return new XamlCodeCompletionItem(m);
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

		static List<ICompletionItem> GetListOfAttached(XamlCompletionContext context, string prefixClassName, string prefixNamespace, bool events, bool properties)
		{
			List<ICompletionItem> result = new List<ICompletionItem>();
			
			if (context.ParseInformation == null)
				return result;
			
			IProjectContent pc = context.ParseInformation.BestCompilationUnit.ProjectContent;
			
			if (!string.IsNullOrEmpty(prefixClassName)) {
				var ns = context.XmlnsDefinitions[prefixNamespace];
				IClass c = XamlCompilationUnit.GetNamespaceMembers(pc, ns).FirstOrDefault(item => item.Name == prefixClassName);
				if (c != null) {
					if (!(c.IsAbstract && c.IsStatic
					      && !c.ClassInheritanceTree.Any(b => b.FullyQualifiedName == "System.Attribute")
					      && c.Methods.Any(m => m.IsConstructor && m.IsPublic))) {
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
						if (c.IsAbstract && c.IsStatic)
							continue;
						if (c.ClassInheritanceTree.Any(b => b.FullyQualifiedName == "System.Attribute"))
							continue;
						if (!c.Methods.Any(m => m.IsConstructor && m.IsPublic))
							continue;
						if (properties)
							AddAttachedProperties(c, result, key, string.Empty);
						if (events)
							AddAttachedEvents(c, result, key, string.Empty);
					}
				}
			}
			
			return result;
		}
		
		static void AddAttachedProperties(IClass c, List<ICompletionItem> result, string key, string prefix)
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