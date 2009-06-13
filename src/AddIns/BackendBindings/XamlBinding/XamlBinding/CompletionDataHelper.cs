// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Project;
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
using ICSharpCode.XmlEditor;
using LoggingService = ICSharpCode.Core.LoggingService;

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
		
		static readonly List<string> xamlNamespaceAttributes = new List<string> {
			"Class", "ClassModifier", "FieldModifier", "Name", "Subclass", "TypeArguments", "Uid"
		};
		#endregion
		
		public const string XamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml";
		
		public static XamlContext ResolveContext(string text, string fileName, int line, int col)
		{
			int offset = Utils.GetOffsetFromFilePos(text, line, col);
			
			ParseInformation info = ParserService.GetParseInformation(fileName);
			XmlElementPath path = XmlParser.GetActiveElementStartPathAtIndex(text, offset);
			string attribute = XmlParser.GetAttributeNameAtIndex(text, offset);
			string attributeValue = XmlParser.GetAttributeValueAtIndex(text, offset);
			bool inAttributeValue = XmlParser.IsInsideAttributeValue(text, offset);
			int offsetFromValueStart = Utils.GetOffsetFromValueStart(text, offset);
			AttributeValue value = null;
			
			value = MarkupExtensionParser.ParseValue(attributeValue);
			
			XamlContextDescription description = XamlContextDescription.InTag;
			
			if (path == null || path.Elements.Count == 0) {
				description = XamlContextDescription.None;
				path = XmlParser.GetParentElementPath(text.Substring(0, offset));
			} else {
				int ltOffset = XmlParser.GetActiveElementStartIndex(text, offset);
				if (ltOffset == -1)
					description = XamlContextDescription.AtTag;
				else {
					string space = text.Substring(ltOffset + 1, offset - ltOffset - 1);
					var last = path.Elements.LastOrDefault();
					if (last != null && last.ToString().StartsWith(space, StringComparison.Ordinal))
						description = XamlContextDescription.AtTag;
				}
			}
			
			if (inAttributeValue)
				description = XamlContextDescription.InAttributeValue;
			
			if (value != null && !value.IsString)
				description = XamlContextDescription.InMarkupExtension;
			
			if (Utils.IsInsideXmlComment(text, offset))
				description = XamlContextDescription.InComment;
			
			Dictionary<string, string> xmlnsDefs = new Dictionary<string, string>();
			
			using (XmlTextReader reader = Utils.CreateReaderAtTarget(text, line, col)) {
                xmlnsDefs.AddRange(reader.GetNamespacesInScope(XmlNamespaceScope.All));
			}
			
			var context = new XamlContext() {
				Description = description,
				AttributeName = attribute,
				AttributeValue = value,
				RawAttributeValue = attributeValue,
				ValueStartOffset = offsetFromValueStart,
				Path = (path == null || path.Elements.Count == 0) ? null : path,
				XmlnsDefinitions = xmlnsDefs,
				ParseInformation = info
			};
			
			return context;
		}
		
		public static XamlCompletionContext ResolveCompletionContext(ITextEditor editor, char typedValue)
		{
			string text = editor.Document.Text;
			int offset = editor.Caret.Offset;
			
			ParseInformation info = ParserService.GetParseInformation(editor.FileName);
			XmlElementPath path = XmlParser.GetActiveElementStartPathAtIndex(text, offset);
			string attribute = XmlParser.GetAttributeNameAtIndex(text, offset);
			string attributeValue = XmlParser.GetAttributeValueAtIndex(text, offset);
			bool inAttributeValue = XmlParser.IsInsideAttributeValue(text, offset);
			int offsetFromValueStart = Utils.GetOffsetFromValueStart(text, offset);
			AttributeValue value = null;
			
			value = MarkupExtensionParser.ParseValue(attributeValue);
			
			XamlContextDescription description = XamlContextDescription.InTag;
			
			if (path == null || path.Elements.Count == 0) {
				description = XamlContextDescription.None;
				path = XmlParser.GetParentElementPath(text.Substring(0, offset));
			} else {
				int ltOffset = XmlParser.GetActiveElementStartIndex(text, offset);
				if (ltOffset == -1)
					description = XamlContextDescription.AtTag;
				else {
					string space = text.Substring(ltOffset + 1, offset - ltOffset - 1);
					var last = path.Elements.LastOrDefault();
					if (last != null && last.ToString().StartsWith(space, StringComparison.Ordinal))
						description = XamlContextDescription.AtTag;
				}
			}
			
			if (inAttributeValue)
				description = XamlContextDescription.InAttributeValue;
			
			if (value != null && !value.IsString)
				description = XamlContextDescription.InMarkupExtension;
			
			if (Utils.IsInsideXmlComment(text, offset))
				description = XamlContextDescription.InComment;
			
			Dictionary<string, string> xmlnsDefs = new Dictionary<string, string>();
			
			using (XmlTextReader reader = Utils.CreateReaderAtTarget(text, editor.Caret.Line, editor.Caret.Column)) {
                xmlnsDefs.AddRange(reader.GetNamespacesInScope(XmlNamespaceScope.All));
			}
			
			var context = new XamlCompletionContext() {
				PressedKey = typedValue,
				Description = description,
				AttributeName = attribute,
				AttributeValue = value,
				RawAttributeValue = attributeValue,
				ValueStartOffset = offsetFromValueStart,
				Path = (path == null || path.Elements.Count == 0) ? null : path,
				XmlnsDefinitions = xmlnsDefs,
				ParseInformation = info,
				Editor = editor
			};
			
			return context;
		}
		
		static List<ICompletionItem> CreateListForAttributeName(XamlCompletionContext context, string[] existingItems)
		{
			QualifiedName lastElement = context.Path.Elements.LastOrDefault();
			XamlCompilationUnit cu = context.ParseInformation.BestCompilationUnit as XamlCompilationUnit;
			if (cu == null)
				return null;
			IReturnType rt = cu.CreateType(lastElement.Namespace, lastElement.Name.Trim('.'));
			if (rt == null)
				return null;
			var list = new List<ICompletionItem>();
			
			string xamlPrefix = Utils.GetXamlNamespacePrefix(context);
			
			foreach (string item in xamlNamespaceAttributes) {
				if (!existingItems.Contains(xamlPrefix + ":" + item))
					list.Add(new XamlCompletionItem(xamlPrefix, XamlNamespace, item));
			}
			
			foreach (IProperty p in rt.GetProperties()) {
				if (p.IsPublic && p.CanSet && !existingItems.Contains(p.Name)) {
					list.Add(new XamlCodeCompletionItem(p));
				}
			}
			foreach (IEvent e in rt.GetEvents()) {
				if (e.IsPublic && !existingItems.Contains(e.Name)) {
					list.Add(new XamlCodeCompletionItem(e));
				}
			}
			return list;
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
					return x.Namespace.CompareTo(y.Namespace);
				if (x.IsUrl)
					return -1;
				if (y.IsUrl)
					return 1;
				if (x.Assembly == y.Assembly)
					return x.Namespace.CompareTo(y.Namespace);
				else
					return x.Assembly.CompareTo(y.Assembly);
			}
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
		
		public static IList<ICompletionItem> CreateListForElement(ParseInformation parseInfo, string fileContent, int caretLine, int caretColumn, bool addOpeningBrace)
		{
			var items = GetClassesFromContext(parseInfo, fileContent, caretLine, caretColumn);
			var result = new List<ICompletionItem>();
			
			foreach (var ns in items) {
				result.AddRange((from c in ns.Value
				                 where (c.ClassType == ClassType.Class &&
				                        !c.IsAbstract && !c.IsStatic &&
				                        !c.ClassInheritanceTree.Any(b => b.FullyQualifiedName == "System.Attribute") &&
				                        c.Methods.Any(m => m.IsConstructor && m.IsPublic))
				                 select (new XamlCodeCompletionItem(c, ns.Key, addOpeningBrace))).Cast<ICompletionItem>()
				               );
			}
			
			return result;
		}
		
		public static IList<ICompletionItem> CreateListOfMarkupExtensions(ParseInformation parseInfo, string fileContent, int caretLine, int caretColumn)
		{
			var list = CreateListForElement(parseInfo, fileContent, caretLine, caretColumn, false);
			
			var neededItems = list
				.Where(i => ((i as XamlCodeCompletionItem).Entity as IClass).ClassInheritanceTree
				       .Any(item => item.FullyQualifiedName == "System.Windows.Markup.MarkupExtension"))
				.Select(
					selItem => {
						var it = selItem as XamlCodeCompletionItem;
						string text = it.Text;
						if (it.Text.EndsWith("Extension", StringComparison.Ordinal))
							text = text.Remove(it.Text.Length - "Extension".Length);
						return new XamlCodeCompletionItem(it.Entity, text);
					}
				)
				.Cast<ICompletionItem>();
			
			return neededItems.ToList();
		}

		public static ICompletionItemList CreateListForContext(ITextEditor editor, XamlCompletionContext context)
		{
			XamlCompletionItemList list = new XamlCompletionItemList();
			ParseInformation info = ParserService.GetParseInformation(editor.FileName);
			
			switch (context.Description) {
				case XamlContextDescription.None:
					if (context.Forced) {
						list.Items.AddRange(standardElements.Select(item => new DefaultCompletionItem("<" + item.Text)).Cast<ICompletionItem>());
						list.Items.AddRange(CreateListForElement(info, editor.Document.Text, editor.Caret.Line, editor.Caret.Column, true));
					}
					break;
				case XamlContextDescription.AtTag:
					list.Items.AddRange(standardElements);
					if (editor.Document.GetCharAt(editor.Caret.Offset - 1) == '.' || context.PressedKey == '.') {
						var loc = editor.Document.OffsetToPosition(Utils.GetParentElementStart(editor));
						var existing = Utils.GetListOfExistingAttributeNames(editor.Document.Text, loc.Line, loc.Column);
						list.Items.AddRange(CreateListForAttributeName(context, existing).RemoveEvents());
					} else
						list.Items.AddRange(CreateListForElement(info, editor.Document.Text, editor.Caret.Line, editor.Caret.Column, false));
					break;
				case XamlContextDescription.InTag:
					var existingAttribs = Utils.GetListOfExistingAttributeNames(editor.Document.Text, editor.Caret.Line, editor.Caret.Column);
					list.Items.AddRange(CreateListForAttributeName(context, existingAttribs));
					
					QualifiedName last = context.Path.Elements[context.Path.Elements.Count - 1];
					
					TypeResolveResult trr = new XamlResolver().Resolve(new ExpressionResult(last.Name, context), info, editor.Document.Text) as TypeResolveResult;
					
					if (trr != null && trr.ResolvedType != null && trr.ResolvedType.GetUnderlyingClass() != null) {
						if (trr.ResolvedType.GetUnderlyingClass().ClassInheritanceTree.Any(i => i.FullyQualifiedName == "System.Windows.DependencyObject")) {
							list.Items.AddRange(GetListOfAttachedProperties(info, editor.Document.Text, editor.Caret.Line, editor.Caret.Column, existingAttribs));
							list.Items.AddRange(GetListOfAttachedEvents(info, editor.Document.Text, editor.Caret.Line, editor.Caret.Column, existingAttribs));
						}
					}
					
					list.Items.AddRange(standardAttributes);
					break;
				case XamlContextDescription.InAttributeValue:
					XamlCodeCompletionBinding.Instance.CtrlSpace(editor);
					break;
			}
			
			list.SortItems();
			
			return list;
		}
		
		static bool FilterCollectionAttributes(ICompletionItem item)
		{
			if (item is XamlCodeCompletionItem) {
				var comItem = item as XamlCodeCompletionItem;
				if (comItem.Entity is IProperty) {
					var prop = comItem.Entity as IProperty;
					var c = prop.ReturnType.GetUnderlyingClass();
					return c != null && c.ClassInheritanceTree.Any(b => b.FullyQualifiedName == "System.Collections.IEnumerable");
				}
			}
			
			return false;
		}
		
		public static IEnumerable<IInsightItem> CreateMarkupExtensionInsight(XamlCompletionContext context, ParseInformation info, ITextEditor editor)
		{
			var markup = Utils.GetInnermostMarkup(context.AttributeValue.ExtensionValue);
			var trr = ResolveMarkupExtensionType(markup, context);
			
			if (trr != null) {
				var ctors = trr.ResolvedType
					.GetMethods()
					.Where(m => m.IsPublic && m.IsConstructor && m.Parameters.Count >= markup.PositionalArguments.Count + 1)
					.OrderBy(m => m.Parameters.Count);
				
				yield return new MarkupExtensionInsightItem(new DefaultMethod(trr.ResolvedClass, trr.ResolvedClass.Name));
				
				foreach (var ctor in ctors)
					yield return new MarkupExtensionInsightItem(ctor);
			}
		}
		
		public static ICompletionItemList CreateMarkupExtensionCompletion(XamlCompletionContext context, ParseInformation info, ITextEditor editor)
		{
			var list = new XamlCompletionItemList();
			var path = XmlParser.GetActiveElementStartPathAtIndex(editor.Document.Text, editor.Caret.Offset);
			
			var markup = Utils.GetInnermostMarkup(context.AttributeValue.ExtensionValue);
			
			var trr = ResolveMarkupExtensionType(markup, context);
			
			if (trr == null) {
				list.Items.AddRange(CreateListOfMarkupExtensions(info, editor.Document.Text, editor.Caret.Line, editor.Caret.Column));
				list.PreselectionLength = markup.ExtensionType.Length;
			} else {
				if (trr.ResolvedType != null) {
					if (markup.NamedArguments.Count == 0) {
						DoPositionalArgsCompletion(list, context, trr, info, editor);
						DoNamedArgsCompletion(list, trr, markup);
					} else
						DoNamedArgsCompletion(list, trr, markup);
				}
			}
			
			list.SortItems();
			
			return list;
		}

		static void DoNamedArgsCompletion(XamlCompletionItemList list, TypeResolveResult trr, MarkupExtensionInfo markup)
		{
			var ctors = trr.ResolvedType.GetMethods().Where(m => m.IsConstructor && m.Parameters.Count >= markup.PositionalArguments.Count);
			if (ctors.Any(ctor => ctor.Parameters.Count >= markup.PositionalArguments.Count)) {
				list.Items.AddRange(trr.ResolvedType.GetProperties().Where(p => p.CanSet && p.IsPublic).Select(p => new XamlCodeCompletionItem(p, p.Name + "=")).Cast<ICompletionItem>());
			}
		}
		
		static void DoPositionalArgsCompletion(XamlCompletionItemList list, XamlCompletionContext context, TypeResolveResult trr, ParseInformation info, ITextEditor editor)
		{
			switch (trr.ResolvedType.FullyQualifiedName) {
				case "System.Windows.Markup.ArrayExtension":
				case "System.Windows.Markup.NullExtension":
					// x:Null/x:Array does not need completion, ignore it
					break;
				case "System.Windows.Markup.StaticExtension":
					if (context.AttributeValue.ExtensionValue.PositionalArguments.Count == 1 && context.PressedKey == ' ') break;
					if (context.AttributeValue.ExtensionValue.PositionalArguments.Count <= 1) DoStaticExtensionCompletion(list, context);
					break;
				case "System.Windows.Markup.TypeExtension":
					if (context.AttributeValue.ExtensionValue.PositionalArguments.Count == 1 && context.PressedKey == ' ') break;
					if (context.AttributeValue.ExtensionValue.PositionalArguments.Count <= 1) {
						list.Items.AddRange(CreateListForElement(info, editor.Document.Text, editor.Caret.Line, editor.Caret.Column, false));
						AttributeValue selItem = context.AttributeValue.ExtensionValue.PositionalArguments.LastOrDefault();
						if (selItem != null && selItem.IsString) {
							string s = selItem.StringValue;
							list.PreselectionLength = s.Length;
							list.SuggestedItem = list.Items.FirstOrDefault(item => item.Text.StartsWith(s, StringComparison.OrdinalIgnoreCase));
						}
					}

					break;
				default:
//							var ctors = trr.ResolvedType
//								.GetMethods()
//								.Where(m => m.IsPublic && m.IsConstructor && m.Parameters.Count >= markup.PositionalArguments.Count + 1)
//								.OrderBy(m => m.Parameters.Count);
//
//							//var ctor = FindCompletableCtor(ctors, markup.PositionalArguments.Count)
					break;
			}
		}
		
		public static IEnumerable<IInsightItem> MemberInsight(MemberResolveResult result)
		{
			switch (result.ResolvedType.FullyQualifiedName) {
				case "System.Windows.Thickness":
					yield return new MemberInsightItem(result.ResolvedMember, "left");
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
		
		public static IEnumerable<ICompletionItem> MemberCompletion(XamlCompletionContext context, IReturnType type)
		{
			if (type == null || type.GetUnderlyingClass() == null)
				yield break;
			
			var c = type.GetUnderlyingClass();
			
			switch (c.ClassType) {
				case ClassType.Enum:
					foreach (IField f in c.Fields)
						yield return new XamlCodeCompletionItem(f);
					break;
				case ClassType.Struct:
					if (c.FullyQualifiedName == "System.Boolean") {
						yield return new DefaultCompletionItem("True");
						yield return new DefaultCompletionItem("False");
					}
					break;
				case ClassType.Delegate:
					IMethod invoker = c.Methods.Where(method => method.Name == "Invoke").FirstOrDefault();
					if (invoker != null) {
						if (context.Path != null) {
							var item = context.Path.Elements.LastOrDefault();
							var evt = ResolveAttribute(context.AttributeName, context) as IEvent;
							if (evt == null)
								break;
							
							int offset = XmlParser.GetActiveElementStartIndex(context.Editor.Document.Text, context.Editor.Caret.Offset);
							
							if (offset == -1)
								break;
							
							var loc = context.Editor.Document.OffsetToPosition(offset);
							
							string prefix = Utils.GetXamlNamespacePrefix(context);
							string name = Utils.GetAttributeValue(context.Editor.Document.Text, loc.Line, loc.Column + 1, "name");
							if (string.IsNullOrEmpty(name))
								name = Utils.GetAttributeValue(context.Editor.Document.Text, loc.Line, loc.Column + 1, (string.IsNullOrEmpty(prefix) ? "" : prefix + ":") + "name");
							
							yield return new NewEventCompletionItem(evt, (string.IsNullOrEmpty(name)) ? item.Name : name);
							
							foreach (var eventItem in CompletionDataHelper.AddMatchingEventHandlers(context.Editor, invoker))
								yield return eventItem;
						}
					}
					break;
			}
			
			switch (c.FullyQualifiedName) {
				case "System.Windows.Media.Brush":
					foreach (var item in typeof(System.Windows.Media.Brushes).GetProperties()) {
						yield return new DefaultCompletionItem(item.Name);
					}
					break;
				case "System.Windows.Media.Color":
					foreach (var item in typeof(System.Windows.Media.Colors).GetProperties()) {
						yield return new DefaultCompletionItem(item.Name);
					}
					break;
			}
		}
		
		static IEntity ResolveAttribute(string attribute, XamlCompletionContext context)
		{
			XamlResolver resolver = new XamlResolver();
			var exp = new ExpressionResult(attribute, context);
			var mrr = resolver.Resolve(exp, context.ParseInformation, context.Editor.Document.Text) as MemberResolveResult;
			
			return mrr.ResolvedMember;
		}

		static void DoStaticExtensionCompletion(XamlCompletionItemList list, XamlCompletionContext context)
		{
			AttributeValue selItem = context.AttributeValue.ExtensionValue.PositionalArguments.LastOrDefault();
			if (context.PressedKey == '.') {
				if (selItem != null && selItem.IsString) {
					var rr = ResolveStringValue(selItem.StringValue, context) as TypeResolveResult;
					if (rr != null)
						list.Items.AddRange(MemberCompletion(context, rr.ResolvedType));
				}
			} else {
				if (selItem != null && selItem.IsString) {
					int index = selItem.StringValue.IndexOf('.');
					string s = (index > -1) ? selItem.StringValue.Substring(0, index) : selItem.StringValue;
					var rr = ResolveStringValue(s, context) as TypeResolveResult;
					if (rr != null) {
						list.Items.AddRange(MemberCompletion(context, rr.ResolvedType));
						
						list.PreselectionLength = selItem.StringValue.Length - index - 1;
						list.SuggestedItem = list.Items.FirstOrDefault(item => item.Text.StartsWith(selItem.StringValue.Substring(index + 1), StringComparison.OrdinalIgnoreCase));
					} else
						DoStaticTypeCompletion(selItem, list, context.ParseInformation, context.Editor);
				} else {
					DoStaticTypeCompletion(selItem, list, context.ParseInformation, context.Editor);
				}
			}
		}

		static void DoStaticTypeCompletion(AttributeValue selItem, XamlCompletionItemList list, ParseInformation info, ITextEditor editor)
		{
			var items = GetClassesFromContext(info, editor.Document.Text, editor.Caret.Line, editor.Caret.Column);
			foreach (var ns in items) {
				list.Items.AddRange(ns.Value.Where(c => c.Fields.Any(f => f.IsStatic) || c.Properties.Any(p => p.IsStatic))
				                    .Select(c => new XamlCodeCompletionItem(c, ns.Key, false))
				                    .Cast<ICompletionItem>());
			}
			if (selItem != null && selItem.IsString) {
				string s = selItem.StringValue;
				list.PreselectionLength = s.Length;
				list.SuggestedItem = list.Items.FirstOrDefault(item => item.Text.StartsWith(s, StringComparison.OrdinalIgnoreCase));
			}
		}
		
		static ResolveResult ResolveStringValue(string value, XamlCompletionContext context)
		{
			var resolver = new XamlResolver();
			var rr = resolver.Resolve(new ExpressionResult(value, context), context.ParseInformation, context.Editor.Document.Text);
			return rr;
		}
		
		static IMethod FindCompletableCtor(IList<IMethod> ctors, int index)
		{
			return null;
		}

		public static TypeResolveResult ResolveMarkupExtensionType(MarkupExtensionInfo markup, XamlCompletionContext context)
		{
			XamlResolver resolver = new XamlResolver();
			TypeResolveResult trr = resolver.Resolve(new ExpressionResult(markup.ExtensionType, context), context.ParseInformation, context.Editor.Document.Text) as TypeResolveResult;
			if (trr == null) trr = resolver.Resolve(new ExpressionResult(markup.ExtensionType + "Extension", context), context.ParseInformation, context.Editor.Document.Text) as TypeResolveResult;
			
			return trr;
		}
		
		public static TypeResolveResult ResolveType(string name, XamlCompletionContext context)
		{
			return new XamlResolver()
				.Resolve(new ExpressionResult(name, context), context.ParseInformation, context.Editor.Document.Text) as TypeResolveResult;
		}
		
		public static IEnumerable<ICompletionItem> AddMatchingEventHandlers(ITextEditor editor, IMethod delegateInvoker)
		{
			ParseInformation p = ParserService.GetParseInformation(editor.FileName);
			var unit = p.MostRecentCompilationUnit;
			var loc = editor.Document.OffsetToPosition(editor.Caret.Offset);
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
		
		static string GetPrefixForNamespace(string @namespace, string fileContent, int caretLine, int caretColumn)
		{
			using (XmlTextReader r = Utils.CreateReaderAtTarget(fileContent, caretLine, caretColumn)) {
				foreach (var item in r.GetNamespacesInScope(XmlNamespaceScope.ExcludeXml)) {
						if (item.Value == @namespace) {
							return item.Key;
						}
				}
				
				return string.Empty;
			}
		}
		
		static IDictionary<string, IEnumerable<IClass>> GetClassesFromContext(ParseInformation parseInfo, string fileContent, int caretLine, int caretColumn)
		{
			using (XmlTextReader r = Utils.CreateReaderAtTarget(fileContent, caretLine, caretColumn)) {
				IProjectContent pc = parseInfo.BestCompilationUnit.ProjectContent;
				
				var result = new Dictionary<string, IEnumerable<IClass>>();
				
				foreach (var ns in r.GetNamespacesInScope(XmlNamespaceScope.ExcludeXml)) {
					result.Add(ns.Key, XamlCompilationUnit.GetNamespaceMembers(pc, ns.Value));
				}
				
				return result;
			}
		}
		
		static List<ICompletionItem> GetListOfAttachedProperties(ParseInformation parseInfo, string fileContent, int caretLine, int caretColumn, string[] existingItems)
		{
			using (XmlTextReader r = Utils.CreateReaderAtTarget(fileContent, caretLine, caretColumn)) {
				List<ICompletionItem> result = new List<ICompletionItem>();
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
							                        	return new XamlCodeCompletionItem(new DefaultProperty(c, property) { ReturnType = GetAttachedPropertyType(item, c) }, name);
							                        }
							                       )
							                .Where(item => !existingItems.Any(str => str == item.Text))
							                .Cast<ICompletionItem>()
							               );
						}
					}
				}
				
				return result;
			}
		}
		
		static List<ICompletionItem> GetListOfAttachedEvents(ParseInformation parseInfo, string fileContent, int caretLine, int caretColumn, string[] existingItems)
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
					                	item => new XamlCodeCompletionItem(
						              		new DefaultEvent(c, GetEventNameFromField(item)) {
										       ReturnType = GetAttachedEventDelegateType(item, c)
										    },
					                		(string.IsNullOrEmpty(ns.Key) ? "" : ns.Key + ":") + c.Name + "." + item.Name.Remove(item.Name.Length - "Event".Length)
					                	)
					                )
					                .Where(item => !existingItems.Any(str => str == item.Text))
					                .Cast<ICompletionItem>()
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
