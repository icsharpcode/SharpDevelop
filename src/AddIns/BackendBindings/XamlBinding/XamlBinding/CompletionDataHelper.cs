// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.XmlEditor;

namespace ICSharpCode.XamlBinding
{
	public static class CompletionDataHelper
	{
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
				if (rr != null) {
					AddPropertiesForType(result, r, rr);
					AddEventsForType(result, r, rr);
				}
				
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
							
							if (!string.IsNullOrEmpty(ns.Key))
								result.Add(new XamlCompletionItem(c, ns.Key));
							else
								result.Add(new XamlCompletionItem(c));
						}
					}
				}
				
				return result;
			}
		}
		
		static void AddEventsForType(List<ICompletionItem> result, XmlTextReader r, TypeResolveResult rr)
		{
			if (rr.ResolvedType != null) {
				foreach (IEvent e in rr.ResolvedType.GetEvents()) {
					if (!e.IsPublic)
						continue;
					
					string propPrefix = e.DeclaringType.Name;
					if (!string.IsNullOrEmpty(r.Prefix))
						propPrefix = r.Prefix + ":" + propPrefix;
					result.Add(new XamlCompletionItem(e, propPrefix));
				}
			}
		}
		
		static void AddPropertiesForType(List<ICompletionItem> result, XmlTextReader r, TypeResolveResult rr)
		{
			if (rr.ResolvedType != null) {
				foreach (IProperty p in rr.ResolvedType.GetProperties()) {
					if (!p.IsPublic)
						continue;
					if (!p.CanSet && !IsCollectionType(p.ReturnType))
						continue;
					string propPrefix = p.DeclaringType.Name;
					if (!string.IsNullOrEmpty(r.Prefix))
						propPrefix = r.Prefix + ":" + propPrefix;
					result.Add(new XamlCompletionItem(p, propPrefix));
				}
			}
		}
		
		static bool IsCollectionType(IReturnType rt)
		{
			if (rt == null)
				return false;
			return rt.GetMethods().Any(m => m.Name == "Add" && m.IsPublic);
		}
		
		class XamlCompletionProperty : DefaultProperty, IEntity
		{
			string newName;

			public XamlCompletionProperty(IProperty baseProperty, string prefix)
				: base(baseProperty.DeclaringType, baseProperty.Name)
			{
				this.Modifiers = baseProperty.Modifiers;
				this.CopyDocumentationFrom(baseProperty);
				newName = prefix + "." + baseProperty.Name;
			}

			string IEntity.Name
			{
				get { return newName; }
			}
		}
		
		class XamlCompletionEvent : DefaultEvent, IEntity
		{
			string newName;

			public XamlCompletionEvent(IEvent baseEvent, string prefix)
				: base(baseEvent.DeclaringType, baseEvent.Name)
			{
				this.Modifiers = baseEvent.Modifiers;
				this.CopyDocumentationFrom(baseEvent);
				newName = prefix + "." + baseEvent.Name;
			}

			string IEntity.Name
			{
				get { return newName; }
			}
		}
		
		static readonly List<ICompletionItem> standardElements = new List<ICompletionItem> {
			new DefaultCompletionItem("!--"),
			new DefaultCompletionItem("![CDATA["),
			new DefaultCompletionItem("?")
		};
		
		static readonly List<ICompletionItem> standardAttributes = new List<ICompletionItem> {
			new DefaultCompletionItem("xmlns:")
		};
		
		public const string XamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml";
		
		static List<ICompletionItem> CreateListOfMarkupExtensions(ParseInformation parseInfo, string fileContent, int caretLine, int caretColumn)
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
					list.Items.AddRange(standardAttributes);
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
						string name = Utils.GetAttributeValue(editor.Document.Text, editor.Caret.Offset, "name");
						list.Items.Add(new NewEventCompletionItem(e, (string.IsNullOrEmpty(name)) ? item.Name : name));
						AddMatchingEventHandlers(editor, invoker, list.Items);
					}
					break;
				case XamlContext.InMarkupExtension:
					list.Items.AddRange(CreateListOfMarkupExtensions(info, editor.Document.Text, editor.Caret.Line, editor.Caret.Column));
					break;
			}
			
			list.SortItems();
			
			if (list.Items.Count >= 1)
				list.SuggestedItem = list.Items[0];
			
			return list;
		}
		
		static void AddMatchingEventHandlers(ITextEditor editor, IMethod delegateInvoker, List<ICompletionItem> list)
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
	}
}
