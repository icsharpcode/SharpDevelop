// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using System.Runtime.InteropServices.ComTypes;
using System.Threading;
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
using ICSharpCode.SharpDevelop.Parser;

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
						list.Items.AddRange(CreateElementList(context, false, false));
						AddClosingTagCompletion(context, list, resolver);
					}
					break;
				case XamlContextDescription.AtTag:
					if ((editor.Caret.Offset > 0 && editor.Document.GetCharAt(editor.Caret.Offset - 1) == '.') || context.PressedKey == '.') {
						list.Items.AddRange(CreateAttributeList(context, false));
					} else {
						list.Items.AddRange(standardElements);
						list.Items.AddRange(CreateElementList(context, false, false));
						AddClosingTagCompletion(context, list, resolver);
					}
					break;
				case XamlContextDescription.InTag:
					string word = editor.GetWordBeforeCaretExtended();
					
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
						
						int caretOffset = editor.Caret.Offset;
						int offset = editor.Document.LastIndexOf(className, caretOffset - word.Length, word.Length, StringComparison.OrdinalIgnoreCase);
						TextLocation loc = editor.Document.GetLocation(offset);
						
						XamlFullParseInformation info = context.ParseInformation;
						TypeResolveResult trr = resolver.ResolveAtLocation(loc) as TypeResolveResult;
						ITypeDefinition typeClass = trr != null ? trr.Type.GetDefinition() : null;
						
						if (typeClass != null && typeClass.HasAttached(true, true))
							list.Items.AddRange(GetListOfAttached(context, ns, typeClass, true, true));
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
		
		public IList<ICompletionItem> CreateElementList(XamlCompletionContext context, bool classesOnly, bool includeAbstract)
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
			var rtd = utd.Resolve(new SimpleTypeResolveContext(compilation.MainAssembly)).GetDefinition();
			MemberLookup memberLookup = new MemberLookup(rtd, compilation.MainAssembly);
			
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
				AddAttachedProperties(lastElement.Prefix, type.GetDefinition(), list);
			} else {
				if (type.Kind == TypeKind.Unknown) {
					list.Add(new XamlCompletionItem(xKey + "Uid"));
				} else {
					AddAttributes(type, list, includeEvents);
					list.AddRange(GetListOfAttached(context, null, null, includeEvents, true));
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
		
		static IType GetType(XamlCompletionContext context, out bool isExplicit)
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
					IReturnType typeName;
					bool isExplicit, showFull = false;
					switch (definition.FullName) {
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
								yield return new SpecialValueCompletionItem(font.FamilyNames.First().Value);
							break;
						default:
							if (context.Description == XamlContextDescription.InMarkupExtension) {
								foreach (IField f in c.Fields)
									yield return new XamlCompletionItem(textPrefix + f.Name, f);
								foreach (IProperty p in c.Properties.Where(pr => pr.IsPublic && pr.IsStatic && pr.CanGet))
									yield return new XamlCompletionItem(textPrefix + p.Name, p);
							}
							break;
					}
					break;
				case TypeKind.Enum:
					foreach (IField f in c.Fields)
						yield return new XamlCompletionItem(textPrefix + f.Name, f);
					foreach (IProperty p in c.Properties.Where(pr => pr.IsPublic && pr.IsStatic && pr.CanGet))
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
					foreach (var item in CreateEventCompletion(context, c))
						yield return item;
					break;
			}
			
			var classes = c.ProjectContent.Classes.Where(
				cla => (cla.FullyQualifiedName == c.FullyQualifiedName + "s" ||
				        cla.FullyQualifiedName == c.FullyQualifiedName + "es"));
			foreach (var coll in classes) {
				foreach (var item in coll.Properties)
					yield return new XamlCompletionItem(item.Name);
				foreach (var item in coll.Fields.Where(f => f.IsPublic && f.IsStatic && f.ReturnType.FullyQualifiedName == c.FullyQualifiedName))
					yield return new XamlCompletionItem(item.Name);
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
		internal List<ICompletionItem> GetListOfAttached(XamlContext context, string prefix, ITypeDefinition attachedType, bool events, bool properties)
		{
			List<ICompletionItem> result = new List<ICompletionItem>();
			
			if (attachedType != null) {
				if (attachedType.Kind == TypeKind.Class && !attachedType.IsDerivedFrom(KnownTypeCode.Attribute)) {
					if (properties)
						AddAttachedProperties(prefix, attachedType, result);
					if (events)
						AddAttachedEvents(prefix, attachedType, result);
				}
			} else {
				foreach (var ns in context.XmlnsDefinitions) {
					string key = string.IsNullOrEmpty(ns.Key) ? "" : ns.Key + ":";
					
					foreach (ITypeDefinition td in ns.Value.GetContents(compilation)) {
						if (td.Kind == TypeKind.Class)
							continue;
						if (td.HasAttached(properties, events))
							result.Add(new XamlCompletionItem(td));
					}
				}
			}
			
			return result;
		}
		
		public static void AddAttachedProperties(string prefix, ITypeDefinition td, List<ICompletionItem> result)
		{
			var attachedProperties = td.Fields.Where(f => f.IsAttached(true, false));
			var unresolvedType = td.Parts.First();
			var resolveContext = new SimpleTypeResolveContext(td);
			if (!string.IsNullOrEmpty(prefix))
				prefix += ":";
			
			result.AddRange(
				attachedProperties.Select(
					property => {
						string propertyName = property.Name.Remove(property.Name.Length - "Property".Length);
						IUnresolvedProperty item = new DefaultUnresolvedProperty(unresolvedType, propertyName);
						IMember entity = item.CreateResolved(resolveContext);
						return new XamlCompletionItem(prefix + propertyName, entity);
					}
				)
			);
		}
		
		static void AddAttachedEvents(string prefix, ITypeDefinition td, List<ICompletionItem> result)
		{
			var attachedEvents = td.Fields.Where(f => f.IsAttached(false, true));
			var unresolvedType = td.Parts.First();
			var resolveContext = new SimpleTypeResolveContext(td);
			if (!string.IsNullOrEmpty(prefix))
				prefix += ":";
			
			result.AddRange(
				attachedEvents.Select(
					field => {
						string eventName = GetEventNameFromField(field);
						IUnresolvedEvent item = new DefaultUnresolvedEvent(unresolvedType, eventName);
						IMember entity = item.CreateResolved(resolveContext);
						return new XamlCompletionItem(prefix + eventName, entity);
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
