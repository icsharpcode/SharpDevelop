// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;

using System.Windows.Media;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.NRefactory.Utils;
using ICSharpCode.NRefactory.Xml;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace ICSharpCode.XamlBinding
{
	/// <summary>
	/// Description of CompletionDataGenerator.
	/// </summary>
	public class CompletionDataGenerator
	{
		ICompilation compilation;
		
		static readonly List<ICompletionItem> standardElements = new List<ICompletionItem> {
			new XamlCompletionItem("!--"),
			new XamlCompletionItem("![CDATA["),
			new XamlCompletionItem("?")
		};
		
		static readonly List<ICompletionItem> standardAttributes = new List<ICompletionItem> {
			new XamlCompletionItem("xmlns:"),
			new XamlCompletionItem("xml:space"),
			new XamlCompletionItem("xml:lang")
		};
		
		public XamlCompletionItemList CreateListForContext(XamlCompletionContext context)
		{
			XamlCompletionItemList list = new XamlCompletionItemList(context);
			
			ITextEditor editor = context.Editor;
			compilation = SD.ParserService.GetCompilationForFile(editor.FileName);
			XamlAstResolver resolver = new XamlAstResolver(compilation, context.ParseInformation);
			
			switch (context.Description) {
				case XamlContextDescription.None:
					if (context.Forced) {
						list.Items.AddRange(standardElements);
						list.Items.AddRange(CreateElementList(context, false));
						AddClosingTagCompletion(context, list, resolver);
					}
					break;
				case XamlContextDescription.AtTag:
					if ((editor.Caret.Offset > 0 && editor.Document.GetCharAt(editor.Caret.Offset - 1) == '.') || context.PressedKey == '.') {
						list.Items.AddRange(CreateAttributeList(context, false));
					} else {
						list.Items.AddRange(standardElements);
						list.Items.AddRange(CreateElementList(context, false));
						AddClosingTagCompletion(context, list, resolver);
					}
					break;
				case XamlContextDescription.InTag:
					string word = editor.GetWordBeforeCaretExtended();
					
					if (context.PressedKey == '.' || word.Contains(".")) {
						int pos = word.IndexOf(':');
						
						string element = word.Substring(pos + 1, word.Length - pos - 1);
						string className = word;
						int propertyStart = element.IndexOf('.');
						if (propertyStart != -1) {
							element = element.Substring(0, propertyStart).TrimEnd('.');
							className = className.Substring(0, propertyStart + pos + 1).TrimEnd('.');
						}
						
						int caretOffset = editor.Caret.Offset;
						int offset = editor.Document.LastIndexOf(className, caretOffset - word.Length, word.Length, StringComparison.OrdinalIgnoreCase);
						TextLocation loc = editor.Document.GetLocation(offset);
						
						XamlFullParseInformation info = context.ParseInformation;
						XamlResolver nameResolver = new XamlResolver(compilation);
						TypeResolveResult trr = nameResolver.ResolveExpression(className, context) as TypeResolveResult;
						ITypeDefinition typeClass = trr != null ? trr.Type.GetDefinition() : null;
						
						if (typeClass != null && typeClass.HasAttached(true, true))
							list.Items.AddRange(GetListOfAttached(context, typeClass, true, true));
					} else {
						list.Items.AddRange(CreateAttributeList(context, true));
						list.Items.AddRange(standardAttributes);
					}
					break;
				case XamlContextDescription.InAttributeValue:
					new XamlCodeCompletionBinding().CtrlSpace(editor);
					break;
			}
			
			list.SortItems();
			
			return list;
		}
		
		public IList<ICompletionItem> CreateElementList(XamlCompletionContext context, bool includeAbstract)
		{
			if (context.ParseInformation == null)
				return EmptyList<ICompletionItem>.Instance;
			List<ICompletionItem> result = new List<ICompletionItem>();
			AXmlElement last = context.ParentElement;
			ITextEditor editor = context.Editor;
			compilation = SD.ParserService.GetCompilationForFile(editor.FileName);
			IUnresolvedFile file = context.ParseInformation.UnresolvedFile;
			var items = GetClassesFromContext(context);
			IType rt = null;

			if (last != null) {
				// If we have an element that is not a property or an incomplete
				// definition => interpret element as a type.
				XamlResolver resolver = new XamlResolver(compilation);
				int dotIndex = last.LocalName.IndexOf(".", StringComparison.Ordinal) + 1;
				if (dotIndex < 1 || dotIndex == last.LocalName.Length) {
					rt = resolver.ResolveType(last.Namespace, last.LocalName.Trim('.'));
					string contentPropertyName = GetContentPropertyName(rt.GetDefinition());
					// If the type has a content property specified, use its type for completion.
					if (!string.IsNullOrEmpty(contentPropertyName)) {
						IProperty p = rt.GetProperties(m => m.Name == contentPropertyName).FirstOrDefault();
						if (p != null) {
							rt = p.ReturnType;
						}
					}
				} else {
					string typeName = last.LocalName.Substring(0, dotIndex - 1);
					string memberName = last.LocalName.Substring(dotIndex);
					rt = resolver.ResolveType(last.Namespace, typeName);
					IMember member = rt.GetMembers(m => m.Name == memberName).FirstOrDefault();
					if (member != null) {
						rt = member.ReturnType;
					}
				}
			}
			
			bool parentAdded = false;
			var utd = file.GetInnermostTypeDefinition(editor.Caret.Location);
			ITypeDefinition currentTypeDef = null;
			if (utd != null) {
				currentTypeDef = utd.Resolve(new SimpleTypeResolveContext(compilation.MainAssembly)).GetDefinition();
			}
			MemberLookup memberLookup = new MemberLookup(currentTypeDef, compilation.MainAssembly);
			
			IList<ITypeDefinition> possibleTypesInCollection = EmptyList<ITypeDefinition>.Instance;
			if (rt != null && Extensions.IsListType(rt)) {
				possibleTypesInCollection = rt.GetMethods(m => m.Parameters.Count == 1 && "Add".Equals(m.Name, StringComparison.Ordinal))
					.Select(m => m.Parameters[0].Type.GetDefinition())
					.Where(t => t != null)
					.ToList();
			}
			
			foreach (var ns in items) {
				foreach (ITypeDefinition td in ns.Value) {
					if (td.Kind != TypeKind.Class && (!includeAbstract || td.Kind != TypeKind.Interface))
						continue;
					if (td.IsStatic || (!includeAbstract && td.IsAbstract) || td.IsDerivedFrom(KnownTypeCode.Attribute))
						continue;
					if (td.Kind == TypeKind.Class && !td.GetConstructors().Any(m => memberLookup.IsAccessible(m, false)))
						continue;
					if (possibleTypesInCollection.Count > 0 && !possibleTypesInCollection.Any(td.IsDerivedFrom))
						continue;
					string fullName = td.Name;
					if (!string.IsNullOrEmpty(ns.Key))
						fullName = ns.Key + ":" + fullName;
					XamlCompletionItem item = new XamlCompletionItem(fullName, td);
					parentAdded = parentAdded || (last != null && item.Text == last.Name);
					result.Add(item);
				}
			}
			
			// TODO reimplement this if it is really necessary.
//			if (!parentAdded && last != null && !last.Name.Contains(".")) {
//				IClass itemClass = cu.CreateType(last.Namespace, last.LocalName.Trim('.')).GetUnderlyingClass();
//				if (itemClass != null)
//					result.Add(new XamlCodeCompletionItem(itemClass, last.Prefix));
//			}
			
			var xamlItems = XamlConst.XamlNamespaceAttributes.AsEnumerable();
			
			if (XamlConst.EnableXaml2009)
				xamlItems = XamlConst.XamlBuiltInTypes.Concat(xamlItems);
			
			foreach (string item in xamlItems) {
				result.Add(new XamlCompletionItem(context.XamlNamespacePrefix + ":" + item));
			}
			
			return result;
		}
		
		void AddClosingTagCompletion(XamlContext context, DefaultCompletionItemList list, XamlAstResolver resolver)
		{
			if (context.ParentElement != null && !context.InRoot) {
				ResolveResult rr = resolver.ResolveElement(context.ParentElement);
				TypeResolveResult trr = rr as TypeResolveResult;
				MemberResolveResult mrr = rr as MemberResolveResult;

				if (trr != null) {
					if (trr.IsError) return;
					list.Items.Add(new XamlCompletionItem("/" + context.ParentElement.Name, trr.Type.GetDefinition()));
				} else if (mrr != null) {
					if (mrr.IsError) return;
					list.Items.Add(new XamlCompletionItem("/" + context.ParentElement.Name, mrr.Member));
				}
			}
		}

		string GetContentPropertyName(ITypeDefinition type)
		{
			if (type == null)
				return string.Empty;
			IType attributeType = compilation.FindType(typeof(System.Windows.Markup.ContentPropertyAttribute));
			IAttribute contentProperty = type.Attributes.FirstOrDefault(attribute => attributeType.Equals(attribute.AttributeType));
			if (contentProperty != null) {
				string value = null;
				if (contentProperty.PositionalArguments.Count > 0) {
					value = contentProperty.PositionalArguments[0].ConstantValue as string;
				}
				if (value == null) {
					foreach (var p in contentProperty.NamedArguments) {
						if (p.Key.Name == "Name") {
							value = p.Value.ConstantValue as string;
							break;
						}
					}
				}
				return value ?? string.Empty;
			}
			
			return string.Empty;
		}
		
		IList<ICompletionItem> CreateAttributeList(XamlCompletionContext context, bool includeEvents)
		{
			if (context.ParseInformation == null)
				return EmptyList<ICompletionItem>.Instance;
			AXmlElement lastElement = context.ActiveElement;
			IUnresolvedFile file = context.ParseInformation.UnresolvedFile;
			XamlResolver resolver = new XamlResolver(compilation);
			IType type = resolver.ResolveType(lastElement.Namespace, lastElement.LocalName.Trim('.'));
			
			var list = new List<ICompletionItem>();
			
			string xamlPrefix = context.XamlNamespacePrefix;
			string xKey = string.IsNullOrEmpty(xamlPrefix) ? "" : xamlPrefix + ":";
			
			if (lastElement.Prefix == context.XamlNamespacePrefix && XamlConst.IsBuiltin(lastElement.LocalName))
				return EmptyList<ICompletionItem>.Instance;
			
			if (lastElement.LocalName.EndsWith(".", StringComparison.OrdinalIgnoreCase) || context.PressedKey == '.') {
				if (type.Kind == TypeKind.Unknown)
					return EmptyList<ICompletionItem>.Instance;
				
				if (context.ParentElement != null
				    && context.ParentElement.LocalName.StartsWith(lastElement.LocalName.TrimEnd('.'), StringComparison.OrdinalIgnoreCase)) {
					AddAttributes(type, list, includeEvents);
				}
				AddAttachedProperties(type.GetDefinition(), list);
			} else {
				if (type.Kind == TypeKind.Unknown) {
					list.Add(new XamlCompletionItem(xKey + "Uid"));
				} else {
					AddAttributes(type, list, includeEvents);
					list.AddRange(GetListOfAttached(context, null, includeEvents, true));
					list.AddRange(
						XamlConst.XamlNamespaceAttributes
						.Where(localName => XamlConst.IsAttributeAllowed(context.InRoot, localName))
						.Select(item => new XamlCompletionItem(xKey + item))
					);
				}
			}
			
			return list;
		}
		
		void AddAttributes(IType type, IList<ICompletionItem> list, bool includeEvents)
		{
			if (type.Kind == TypeKind.Unknown)
				return;
			
			foreach (IProperty p in type.GetProperties(up => up.IsPublic)) {
				if ((p.CanSet && p.Setter.IsPublic) || p.ReturnType.IsCollectionType())
					list.Add(new XamlCompletionItem(p));
			}
			
			if (includeEvents) {
				foreach (IEvent e in type.GetEvents(ue => ue.IsPublic)) {
					list.Add(new XamlCompletionItem(e));
				}
			}
		}
		
		IDictionary<string, IEnumerable<ITypeDefinition>> GetClassesFromContext(XamlContext context)
		{
			var result = new Dictionary<string, IEnumerable<ITypeDefinition>>();
			
			if (compilation == null)
				return result;
			
			foreach (var ns in context.XmlnsDefinitions) {
				result.Add(ns.Key, ns.Value.GetContents(compilation));
			}
			
			return result;
		}
		
		public IEnumerable<ICompletionItem> CreateListForXmlnsCompletion(ICompilation compilation)
		{
			this.compilation = compilation;
			
			List<XmlnsCompletionItem> list = new List<XmlnsCompletionItem>();
			IType xmlnsAttrType = compilation.FindType(typeof(System.Windows.Markup.XmlnsDefinitionAttribute));
			foreach (IAssembly asm in compilation.ReferencedAssemblies) {
				foreach (IAttribute att in asm.AssemblyAttributes) {
					if (att.PositionalArguments.Count == 2 && att.AttributeType.Equals(xmlnsAttrType)) {
						ResolveResult arg1 = att.PositionalArguments[0];
						if (arg1.IsCompileTimeConstant && arg1.ConstantValue is string) {
							list.Add(new XmlnsCompletionItem((string)arg1.ConstantValue, true));
						}
					}
				}
				
				foreach (INamespace @namespace in TreeTraversal.PreOrder(asm.RootNamespace, ns => ns.ChildNamespaces)) {
					list.Add(new XmlnsCompletionItem(@namespace.FullName, asm.AssemblyName));
				}
			}
			
			foreach (INamespace @namespace in TreeTraversal.PreOrder(compilation.MainAssembly.RootNamespace, ns => ns.ChildNamespaces)) {
				list.Add(new XmlnsCompletionItem(@namespace.FullName, false));
			}
			
			list.Add(new XmlnsCompletionItem(XamlConst.MarkupCompatibilityNamespace, true));
			
			return list
				.Distinct(new XmlnsEqualityComparer())
				.OrderBy(item => item, new XmlnsComparer());
		}
		
		#region Member completion
		public IEnumerable<IInsightItem> MemberInsight(MemberResolveResult result)
		{
			switch (result.Type.FullName) {
				case "System.Windows.Thickness":
					yield return new MemberInsightItem(result.Member, "uniformLength");
					yield return new MemberInsightItem(result.Member, "left, top");
					yield return new MemberInsightItem(result.Member, "left, top, right, bottom");
					break;
				case "System.Windows.Size":
					yield return new MemberInsightItem(result.Member, "width, height");
					break;
				case "System.Windows.Point":
					yield return new MemberInsightItem(result.Member, "x, y");
					break;
				case "System.Windows.Rect":
					yield return new MemberInsightItem(result.Member, "x, y, width, height");
					break;
			}
		}
		
		public IEnumerable<ICompletionItem> MemberCompletion(XamlCompletionContext context, IType type, string textPrefix = "")
		{
			ITextEditor editor = context.Editor;
			compilation = SD.ParserService.GetCompilationForFile(editor.FileName);
			
			if (type.Name == typeof(System.Nullable<>).Name) {
				string nullExtensionName = "Null";
				if (!string.IsNullOrEmpty(context.XamlNamespacePrefix))
					nullExtensionName = context.XamlNamespacePrefix + ":" + nullExtensionName;
				yield return new XamlCompletionItem("{" + nullExtensionName + "}");
				type = type.TypeArguments.FirstOrDefault();
				if (type == null) yield break;
			}
			
			ITypeDefinition definition = type.GetDefinition();
			
			if (definition == null) yield break;
			
			switch (definition.Kind) {
				case TypeKind.Class:
					IType typeName;
					bool isExplicit, showFull = false;
					switch (definition.FullName) {
						case "System.String":
							// return nothing
							break;
						case "System.Type":
							foreach (var item in CreateElementList(context, true))
								yield return item;
							break;
						case "System.Windows.PropertyPath":
							foreach (var item in CreatePropertyPathCompletion(context))
								yield return item;
							break;
						case "System.Windows.DependencyProperty":
							typeName = GetType(context, out isExplicit);
							
							bool isReadOnly = context.ActiveElement.Name.EndsWith("Trigger", StringComparison.Ordinal);
							
							if (!isExplicit && ((context.ValueStartOffset > 0) ? context.RawAttributeValue.Substring(0, context.ValueStartOffset) : "").Contains("."))
								showFull = true;
							
							if (typeName != null) {
								foreach (var item in typeName.GetDependencyProperties(true, !isExplicit, !isReadOnly, showFull))
									yield return item;
							}
							break;
						case "System.Windows.RoutedEvent":
							typeName = GetType(context, out isExplicit);
							
							if (!isExplicit && ((context.ValueStartOffset > 0) ? context.RawAttributeValue.Substring(0, context.ValueStartOffset) : "").Contains("."))
								showFull = true;
							
							if (typeName != null) {
								foreach (var item in typeName.GetRoutedEvents(true, !isExplicit, showFull))
									yield return item;
							}
							break;
						case "System.Windows.Media.FontFamily":
							foreach (var font in Fonts.SystemFontFamilies)
								yield return new XamlCompletionItem(font.FamilyNames.First().Value) { Image = ClassBrowserIconService.Const };
							break;
						default:
							if (context.Description == XamlContextDescription.InMarkupExtension) {
								foreach (IField f in definition.Fields)
									yield return new XamlCompletionItem(textPrefix + f.Name, f);
								foreach (IProperty p in definition.GetProperties(pr => pr.IsPublic && pr.IsStatic && pr.CanGet))
									yield return new XamlCompletionItem(textPrefix + p.Name, p);
							}
							break;
					}
					break;
				case TypeKind.Enum:
					foreach (IField f in definition.Fields)
						yield return new XamlCompletionItem(textPrefix + f.Name, f);
					foreach (IProperty p in definition.Properties.Where(pr => pr.IsPublic && pr.IsStatic && pr.CanGet))
						yield return new XamlCompletionItem(textPrefix + p.Name, p);
					break;
				case TypeKind.Struct:
					switch (definition.FullName) {
						case "System.Boolean":
							yield return new DefaultCompletionItem("True") { Image = ClassBrowserIconService.Const };
							yield return new DefaultCompletionItem("False") { Image = ClassBrowserIconService.Const };
							break;
						case "System.Windows.GridLength":
							yield return new XamlCompletionItem("Auto") { Image = ClassBrowserIconService.Const };
							yield return new XamlCompletionItem("*") { Image = ClassBrowserIconService.Const };
							break;
					}
					break;
				case TypeKind.Delegate:
					foreach (var item in CreateEventCompletion(context, definition))
						yield return item;
					break;
			}
			
			var classes = definition.ParentAssembly
				.GetAllTypeDefinitions()
				.Where(cla => cla.FullName == definition.FullName + "s" ||
				       cla.FullName == definition.FullName + "es");
			foreach (var coll in classes) {
				foreach (var item in coll.Properties)
					yield return new XamlCompletionItem(item);
				foreach (var item in coll.Fields.Where(f => f.IsPublic && f.IsStatic && f.ReturnType.FullName == definition.FullName))
					yield return new XamlCompletionItem(item);
			}
		}
		#endregion
		
		#region Markup Extensions
		public IEnumerable<IInsightItem> CreateMarkupExtensionInsight(XamlCompletionContext context)
		{
			var markup = Utils.GetMarkupExtensionAtPosition(context.AttributeValue.ExtensionValue, context.ValueStartOffset);
			var resolver = new XamlResolver(compilation);
			var type = (resolver.ResolveExpression(markup.ExtensionType, context) ?? resolver.ResolveExpression(markup.ExtensionType + "Extension", context)).Type;
			
			if (type != null) {
				var ctors = type
					.GetMethods(m => m.IsPublic && m.IsConstructor && m.Parameters.Count >= markup.PositionalArguments.Count)
					.OrderBy(m => m.Parameters.Count);
				
				foreach (var ctor in ctors)
					yield return new MarkupExtensionInsightItem(ctor);
			}
		}
		
		public ICompletionItemList CreateMarkupExtensionCompletion(XamlCompletionContext context)
		{
			var list = new XamlCompletionItemList(context);
			compilation = SD.ParserService.GetCompilationForFile(context.Editor.FileName);
			string visibleValue = context.RawAttributeValue.Substring(0, Utils.MinMax(context.ValueStartOffset, 0, context.RawAttributeValue.Length));
			if (context.PressedKey == '=')
				visibleValue += "=";
			var markup = Utils.GetMarkupExtensionAtPosition(context.AttributeValue.ExtensionValue, context.ValueStartOffset);
			var resolver = new XamlResolver(compilation);
			var type = resolver.ResolveExpression(markup.ExtensionType, context).Type;
			if (type.Kind == TypeKind.Unknown)
				type = resolver.ResolveExpression(markup.ExtensionType + "Extension", context).Type;
			
			if (type.Kind == TypeKind.Unknown) {
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
		
		void DoNamedArgsCompletion(XamlCompletionItemList list, XamlCompletionContext context, IType type, MarkupExtensionInfo markup)
		{
			if (markup.NamedArguments.Count > 0 && !context.Editor.GetWordBeforeCaret().StartsWith(",", StringComparison.OrdinalIgnoreCase)) {
				int lastStart = markup.NamedArguments.Max(i => i.Value.StartOffset);
				var item = markup.NamedArguments.First(p => p.Value.StartOffset == lastStart);
				
				if (context.RawAttributeValue.EndsWith("=", StringComparison.OrdinalIgnoreCase) ||
				    (item.Value.IsString && item.Value.StringValue.EndsWith(context.Editor.GetWordBeforeCaretExtended(), StringComparison.Ordinal))) {
					var resolver = new XamlResolver(compilation);
					MemberResolveResult mrr = resolver.ResolveAttributeValue(item.Key, context) as MemberResolveResult;
					if (mrr != null && mrr.Member != null && mrr.Member.ReturnType != null) {
						IType memberType = mrr.Member.ReturnType;
						list.Items.AddRange(MemberCompletion(context, memberType, string.Empty));
					}
					return;
				}
			}
			
			list.Items.AddRange(type.GetProperties().Where(p => p.CanSet && p.IsPublic).Select(p => new XamlCompletionItem(p.Name + "=", p)));
		}

		/// <remarks>returns true if elements from named args completion should be added afterwards.</remarks>
		bool DoPositionalArgsCompletion(XamlCompletionItemList list, XamlCompletionContext context, MarkupExtensionInfo markup, IType type)
		{
			switch (type.FullName) {
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
					var ctors = type.GetMethods(m => m.IsPublic && m.IsConstructor && m.Parameters.Count >= markup.PositionalArguments.Count + 1);
					if (context.Forced)
						return true;
					if (ctors.Any() || markup.PositionalArguments.Count == 0)
						return false;
					break;
			}
			
			return true;
		}
		
		bool DoStaticExtensionCompletion(XamlCompletionItemList list, XamlCompletionContext context)
		{
			AttributeValue selItem = Utils.GetMarkupExtensionAtPosition(context.AttributeValue.ExtensionValue, context.ValueStartOffset)
				.PositionalArguments.LastOrDefault();
			var resolver = new XamlResolver(compilation);
			if (context.PressedKey == '.') {
				if (selItem != null && selItem.IsString) {
					var rr = resolver.ResolveAttributeValue(selItem.StringValue, context) as TypeResolveResult;
					if (rr != null)
						list.Items.AddRange(MemberCompletion(context, rr.Type, string.Empty));
					return false;
				}
			} else {
				if (selItem != null && selItem.IsString) {
					int index = selItem.StringValue.IndexOf('.');
					string s = (index > -1) ? selItem.StringValue.Substring(0, index) : selItem.StringValue;
					var rr = resolver.ResolveAttributeValue(s, context) as TypeResolveResult;
					if (rr != null && rr.Type.Kind != TypeKind.Unknown) {
						list.Items.AddRange(MemberCompletion(context, rr.Type, (index == -1) ? "." : string.Empty));
						
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

		void DoStaticTypeCompletion(AttributeValue selItem, XamlCompletionItemList list, XamlCompletionContext context)
		{
			var items = GetClassesFromContext(context);
			foreach (var ns in items) {
				string key = ns.Key;
				if (!string.IsNullOrEmpty(key)) key += ":";
				list.Items.AddRange(
					ns.Value
					.Where(c => c.Fields.Any(f => f.IsStatic) || c.Properties.Any(p => p.IsStatic))
					.Select(c => new XamlCompletionItem(key + c.Name, c))
				);
			}
			if (selItem != null && selItem.IsString) {
				list.PreselectionLength = selItem.StringValue.Length;
			}
		}
		
		
		public IEnumerable<ICompletionItem> CreateListOfMarkupExtensions(XamlCompletionContext context)
		{
			var markupExtensionType = compilation.FindType(typeof(System.Windows.Markup.MarkupExtension))
				.GetDefinition();
			if (markupExtensionType == null)
				yield break;
			string text;
			foreach (var ns in GetClassesFromContext(context)) {
				foreach (var definition in ns.Value.Where(td => td.IsDerivedFrom(markupExtensionType))) {
					text = definition.Name;
					if (text.EndsWith("Extension", StringComparison.Ordinal))
						text = text.Remove(text.Length - "Extension".Length);
					string prefix = ns.Key;
					if (prefix.Length > 0) {
						text = prefix + ":" + text;
					}
					yield return new XamlCompletionItem(text, definition);
				}
			}
			
			text = "Reference";
			if (context.XamlNamespacePrefix != "") {
				text = context.XamlNamespacePrefix + ":" + text;
			}
			yield return new XamlCompletionItem(text);
		}

		#endregion
		
		public IEnumerable<ICompletionItem> FindMatchingEventHandlers(XamlCompletionContext context, IEvent member, string targetName)
		{
			ITypeDefinition td = member.ReturnType.GetDefinition();
			if (td == null) yield break;

			IMethod delegateInvoker = td.GetMethods(m => m.Name == "Invoke").FirstOrDefault();
			if (delegateInvoker == null) yield break;
			
			if (context.ParseInformation == null)
				yield break;
			
			var file = context.ParseInformation.UnresolvedFile;
			var loc = context.Editor.Caret.Location;
			var unresolved = file.GetInnermostTypeDefinition(loc.Line, loc.Column);
			if (unresolved == null)
				yield break;
			IType type = unresolved.Resolve(new SimpleTypeResolveContext(compilation.MainAssembly)).GetDefinition();
			foreach (IMethod method in type.GetMethods(m => m.Parameters.Count == delegateInvoker.Parameters.Count)) {
				if (!method.ReturnType.Equals(delegateInvoker.ReturnType))
					continue;
				
				if (method.Parameters.SequenceEqual(delegateInvoker.Parameters, new ParameterComparer())) {
					yield return new XamlCompletionItem(method);
				}
			}
			
			yield return new NewEventCompletionItem(member, targetName);
		}
		
		IEnumerable<ICompletionItem> CreateEventCompletion(XamlCompletionContext context, ITypeDefinition td)
		{
			IMethod invoker = td.GetMethods(method => method.Name == "Invoke").FirstOrDefault();
			if (invoker != null && context.ActiveElement != null) {
				var item = context.ActiveElement;
				var resolver = new XamlAstResolver(compilation, context.ParseInformation);
				var mrr = resolver.ResolveAttribute(context.Attribute) as MemberResolveResult;
				IEvent evt;
				if (mrr == null || (evt = mrr.Member as IEvent) == null)
					return EmptyList<ICompletionItem>.Instance;
				int offset = XmlEditor.XmlParser.GetActiveElementStartIndex(context.Editor.Document.Text, context.Editor.Caret.Offset);
				
				if (offset == -1)
					return Enumerable.Empty<ICompletionItem>();
				
				var loc = context.Editor.Document.GetLocation(offset);
				
				string name = context.ActiveElement.GetAttributeValue("Name");
				if (string.IsNullOrEmpty(name))
					name = context.ActiveElement.GetAttributeValue(XamlConst.XamlNamespace, "Name");
				
				return FindMatchingEventHandlers(context, evt, (string.IsNullOrEmpty(name) ? item.Name : name));
			}
			
			return EmptyList<ICompletionItem>.Instance;
		}
		
		IList<ICompletionItem> CreatePropertyPathCompletion(XamlCompletionContext context)
		{
			bool isExplicit;
			IType typeName = GetType(context, out isExplicit);
			IList<ICompletionItem> list = new List<ICompletionItem>();
			
			string value = context.ValueStartOffset > -1 ? context.RawAttributeValue.Substring(0, Math.Min(context.ValueStartOffset + 1, context.RawAttributeValue.Length)) : "";
			
			if (value.EndsWithAny(']', ')'))
				return list;
			
			var segments = PropertyPathParser.Parse(value).ToList();
			
			int completionStart;
			bool isAtDot = false;
			
			IType propertyPathType = ResolvePropertyPath(segments, context, typeName, out completionStart);
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
		
		IEnumerable<ICompletionItem> GetAllTypes(XamlCompletionContext context)
		{
			var items = GetClassesFromContext(context);
			
			foreach (var ns in items) {
				foreach (var c in ns.Value) {
					if (c.Kind == TypeKind.Class && !c.IsDerivedFrom(KnownTypeCode.Attribute))
						yield return new XamlCompletionItem(ns.Key, c);
				}
			}
		}
		
		IType GetType(XamlContext context, out bool isExplicit)
		{
			string targetTypeValue = Utils.LookForTargetTypeValue(context, out isExplicit, "Trigger", "Setter");
			AttributeValue value = MarkupExtensionParser.ParseValue(targetTypeValue ?? string.Empty);
			XamlResolver resolver = new XamlResolver(compilation);
			return resolver.ResolveAttributeValue(context, value).Type;
		}
		
		static IType ResolvePropertyPath(IList<PropertyPathSegment> segments, XamlCompletionContext context, IType parentType, out int lastIndex)
		{
			IType type = parentType;
			
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
							IProperty prop = type.GetProperties(p => p.IsIndexer).FirstOrDefault();
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
		
		#region Attached Properties and Events
		internal List<ICompletionItem> GetListOfAttached(XamlContext context, ITypeDefinition attachedType, bool events, bool properties)
		{
			List<ICompletionItem> result = new List<ICompletionItem>();
			
			if (attachedType != null) {
				if (attachedType.Kind == TypeKind.Class && !attachedType.IsDerivedFrom(KnownTypeCode.Attribute)) {
					if (properties)
						AddAttachedProperties(attachedType, result);
					if (events)
						AddAttachedEvents(attachedType, result);
				}
			} else {
				foreach (var ns in context.XmlnsDefinitions) {
					string key = string.IsNullOrEmpty(ns.Key) ? "" : ns.Key + ":";
					
					foreach (ITypeDefinition td in ns.Value.GetContents(compilation)) {
						if (td.Kind != TypeKind.Class)
							continue;
						if (td.HasAttached(properties, events))
							result.Add(new XamlCompletionItem(td));
					}
				}
			}
			
			return result;
		}
		
		public static void AddAttachedProperties(ITypeDefinition td, List<ICompletionItem> result)
		{
			var attachedProperties = td.Fields.Where(f => f.IsAttached(true, false));
			var unresolvedType = td.Parts.First();
			var resolveContext = new SimpleTypeResolveContext(td);
			
			result.AddRange(
				attachedProperties.Select(
					property => {
						string propertyName = property.Name.Remove(property.Name.Length - "Property".Length);
						IUnresolvedProperty item = new DefaultUnresolvedProperty(unresolvedType, propertyName);
						IMember entity = item.CreateResolved(resolveContext);
						return new XamlCompletionItem(propertyName, entity);
					}
				)
			);
		}
		
		static void AddAttachedEvents(ITypeDefinition td, List<ICompletionItem> result)
		{
			var attachedEvents = td.Fields.Where(f => f.IsAttached(false, true));
			var unresolvedType = td.Parts.First();
			var resolveContext = new SimpleTypeResolveContext(td);

			result.AddRange(
				attachedEvents.Select(
					field => {
						string eventName = GetEventNameFromField(field);
						IUnresolvedEvent item = new DefaultUnresolvedEvent(unresolvedType, eventName);
						IMember entity = item.CreateResolved(resolveContext);
						return new XamlCompletionItem(eventName, entity);
					}
				)
			);
		}

		static IType GetAttachedEventDelegateType(IField field, IType t)
		{
			if (t == null || field == null)
				return null;
			
			string eventName = GetEventNameFromField(field);
			
			IMethod method = t.GetMethods(m => m.IsPublic && m.IsStatic && m.Parameters.Count == 2 && (m.Name == "Add" + eventName + "Handler" || m.Name == "Remove" + eventName + "Handler")).FirstOrDefault();
			
			if (method == null)
				return null;
			
			return method.Parameters[1].Type;
		}

		static IType GetAttachedPropertyType(IField field, IType t)
		{
			if (t == null || field == null)
				return null;
			
			string propertyName = field.Name.Remove(field.Name.Length - "Property".Length);
			
			IMethod method = t.GetMethods(m => m.IsPublic && m.IsStatic && m.Name == "Get" + propertyName).FirstOrDefault();
			
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
		#endregion
	}
}
