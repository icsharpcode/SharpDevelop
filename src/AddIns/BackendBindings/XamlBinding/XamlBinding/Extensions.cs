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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;

using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace ICSharpCode.XamlBinding
{
	public static class Extensions
	{
		public static XElement AddAttribute(this XElement element, XName name, object value)
		{
			if (element == null)
				throw new ArgumentNullException("element");
			element.SetAttributeValue(name, value);
			return element;
		}
		
		public static XElement MoveBefore(this XElement element, XNode target)
		{
			if (element == null)
				throw new ArgumentNullException("element");
			
			if (element != target) {
				if (element.Parent != null)
					element.Remove();
				
				if (target.Parent != null)
					target.AddBeforeSelf(element);
			}
			
			return element;
		}
		
		public static string[] GetCurrentNamespaces(this XElement element)
		{
			Dictionary<XName, string> active = new Dictionary<XName, string>();
			
			var current = element;
			
			while (current != null) {
				var newAttributes = current.Attributes()
					.Where(i => i.IsNamespaceDeclaration && !active.ContainsKey(i.Name))
					.ToArray();
				foreach (var item in newAttributes)
					active.Add(item.Name, item.Value);
				current = current.Parent;
			}
			
			return active.Select(pair => pair.Value).ToArray();
		}
		
		public static XElement MoveAfter(this XElement element, XNode target)
		{
			if (element == null)
				throw new ArgumentNullException("element");
			
			if (element != target) {
				if (element.Parent != null)
					element.Remove();
				
				if (target.Parent != null)
					target.AddAfterSelf(element);
			}
			
			return element;
		}
		
		public static void AddRange(this UIElementCollection collection, IEnumerable<UIElement> items)
		{
			foreach (var item in items)
				collection.Add(item);
		}
		
		public static string[] Split(this string thisValue, StringSplitOptions options, params char[] delimiters)
		{
			if (thisValue == null)
				throw new ArgumentNullException("thisValue");
			
			return thisValue.Split(delimiters, options);
		}
		
		public static bool EndsWith(this string thisValue, StringComparison comparison, params char[] characters)
		{
			if (thisValue == null)
				throw new ArgumentNullException("thisValue");
			
			foreach (var c in characters) {
				if (thisValue.EndsWith(c.ToString(), comparison))
					return true;
			}
			
			return false;
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
		
		public static string Replace(this string thisValue, int index, int length, string text)
		{
			if (thisValue == null)
				throw new ArgumentNullException("thisValue");
			if (index < 0 || index > thisValue.Length)
				throw new ArgumentOutOfRangeException("index", index, "Value must be between 0 and " + thisValue.Length);
			if (length < 0 || length > thisValue.Length)
				throw new ArgumentOutOfRangeException("length", length, "Value must be between 0 and " + thisValue.Length);
			if ((index + length) > thisValue.Length)
				throw new ArgumentOutOfRangeException("index + length", index + length, "Value must be between 0 and " + thisValue.Length);
			
			return thisValue.Substring(0, index) + text + thisValue.Substring(index + length);
		}
		
		public static bool Is(char value, params char[] choice)
		{
			foreach (var ch in choice) {
				if (ch == value)
					return true;
			}
			
			return false;
		}
		
		public static IEnumerable<T> Add<T>(this IEnumerable<T> items, params T[] addItems)
		{
			return items.Concat(addItems);
		}
		
		public static string GetWordBeforeCaretExtended(this ITextEditor editor)
		{
			IDocumentLine line = editor.Document.GetLine(editor.Caret.Line);
			string lineText = editor.Document.GetText(line);
			int index = Math.Min(editor.Caret.Column - 1, lineText.Length);
			string text = lineText.Substring(0, index);
			int startIndex = text.LastIndexOfAny(' ', '\t', '"', '=', '<', '\'', '>', '{', '}');
			if (startIndex > -1)
				return text.Substring(startIndex + 1);
			
			return string.Empty;
		}
		
		public static int LastIndexOfAny(this string thisValue, params char[] anyOf)
		{
			return thisValue.LastIndexOfAny(anyOf);
		}
		
		public static int IndexOfAny(this string thisValue, int startIndex, params char[] anyOf)
		{
			return thisValue.IndexOfAny(anyOf, startIndex);
		}
		
		public static string GetWordBeforeOffset(this string text, int startIndex)
		{
			if (string.IsNullOrEmpty(text))
				return string.Empty;
			
			int offset = startIndex = Math.Min(startIndex, text.Length - 1);
			
			while (offset > -1 && char.IsWhiteSpace(text[offset]))
				offset--;
			
			while (offset > -1 && !char.IsWhiteSpace(text[offset]))
				offset--;
			
			offset = Math.Max(0, offset);
			
			return text.Substring(offset, startIndex - offset + 1).Trim();
		}
		
		public static string GetWordAfterOffset(this string text, int startIndex)
		{
			if (string.IsNullOrEmpty(text))
				return string.Empty;
			
			int offset = startIndex = Math.Max(startIndex, 0);
			
			while (offset < text.Length && (char.IsWhiteSpace(text[offset]) || Extensions.Is(text[offset], '>', '<')))
				offset++;
			
			while (offset < text.Length && !(char.IsWhiteSpace(text[offset]) || Extensions.Is(text[offset], '>', '<')))
				offset++;
			
			offset = Math.Min(offset, text.Length - 1);
			
			return text.Substring(startIndex, offset - startIndex + 1).Trim();
		}
		
		public static IType Resolve(this PropertyPathSegment segment, XamlCompletionContext context, IType previousType)
		{
			if (segment.Kind == SegmentKind.SourceTraversal)
				return previousType;
			if (segment.Kind == SegmentKind.ControlChar)
				return previousType;
			
			string content = segment.Content;
			
			if (segment.Kind == SegmentKind.AttachedProperty && content.StartsWith("(", StringComparison.Ordinal)) {
				content = content.TrimStart('(');
				if (content.Contains("."))
					content = content.Remove(content.IndexOf('.'));
			}
			
			ICompilation compilation = SD.ParserService.GetCompilationForFile(context.Editor.FileName);
			XamlResolver resolver = new XamlResolver(compilation);
			
			ResolveResult rr = resolver.ResolveExpression(content, context);
			IType type = rr.Type;

			if (previousType != null) {
				IMember member = previousType.GetMemberByName(content);
				if (member != null)
					type = member.ReturnType;
			} else if (rr is MemberResolveResult) {
				MemberResolveResult mrr = rr as MemberResolveResult;
				if (mrr.Member != null)
					type = mrr.Member.ReturnType;
			}
			return type;
		}
		
		static IMember GetMemberByName(this IType type, string name)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			
			IMember member = type.GetFields(m => m.Name == name).FirstOrDefault();
			if (member == null) {
				member = type.GetProperties(m => m.Name == name).FirstOrDefault();
			}
			
			return member;
		}
		
		public static IEnumerable<ICompletionItem> FlattenToList(this IDictionary<string, IEnumerable<ITypeDefinition>> data)
		{
			foreach (var item in data) {
				foreach (var c in item.Value) {
					string name = c.Name;
					if (item.Key != "")
						name = item.Key + ":" + name;
					yield return new XamlCompletionItem(name, c);
				}
			}
		}
		
		public static IEnumerable<ICompletionItem> GetDependencyProperties(this IType type, bool excludeSuffix, bool addType, bool requiresSetable, bool showFull)
		{
			foreach (var field in type.GetFields()) {
				if (field.ReturnType.FullName != "System.Windows.DependencyProperty")
					continue;
				if (field.Name.Length <= "Property".Length || !field.Name.EndsWith("Property", StringComparison.Ordinal))
					continue;
				string fieldName = field.Name.Remove(field.Name.Length - "Property".Length);
				IProperty property = type.GetProperties().FirstOrDefault(p => p.Name == fieldName);
				if (property == null)
					continue;
				if (requiresSetable && !(property.CanSet && property.Setter.IsPublic))
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
		
		public static IEnumerable<ICompletionItem> GetRoutedEvents(this IType type, bool excludeSuffix, bool addType, bool showFull)
		{
			foreach (var field in type.GetFields()) {
				if (field.ReturnType.FullName != "System.Windows.RoutedEvent")
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
		
		public static TKey GetKeyByValue<TKey, TValue>(this Dictionary<TKey, TValue> thisValue, TValue value)
		{
			foreach (var pair in thisValue) {
				if (pair.Value.Equals(value))
					return pair.Key;
			}
			
			return default(TKey);
		}
		
		public static int GetLineNumber(this IXmlLineInfo thisValue)
		{
			return thisValue.LineNumber;
		}
		
		public static int GetLinePosition(this IXmlLineInfo thisValue)
		{
			return thisValue.LinePosition;
		}
		
		public static TextLocation GetLocation(this IXmlLineInfo thisValue)
		{
			return new TextLocation(thisValue.GetLinePosition(), thisValue.GetLineNumber());
		}
		
		public static bool IsInRange(this IXmlLineInfo item, TextLocation begin, TextLocation end)
		{
			return IsInRange(item, begin.Line, begin.Column, end.Line, end.Column);
		}
		
		public static bool IsInRange(this IXmlLineInfo item, int beginLine, int beginColumn, int endLine, int endColumn)
		{
			if (item.GetLineNumber() >= beginLine && item.GetLineNumber() <= endLine) {
				if (item.GetLineNumber() == beginLine) {
					return item.GetLinePosition() >= beginColumn;
				}
				if (item.GetLineNumber() == endLine) {
					return item.GetLinePosition() <= endColumn;
				}
				return true;
			}
			
			return false;
		}
		
		public static bool IsCollectionType(this IType thisValue)
		{
			if (thisValue == null)
				throw new ArgumentNullException("thisValue");
			return thisValue.GetAllBaseTypeDefinitions().Any(t => t.FullName == "System.Collections.ICollection");
		}
		
		public static bool IsListType(this IType thisValue)
		{
			if (thisValue == null)
				throw new ArgumentNullException("thisValue");
			return thisValue.GetAllBaseTypeDefinitions().Any(t => t.FullName == "System.Collections.IList");
		}
		
		public static bool HasAttached(this ITypeDefinition thisValue, bool lookForProperties, bool lookForEvents)
		{
			if (!lookForProperties && !lookForEvents)
				return false;
			
			foreach (IField field in thisValue.Fields) {
				if (field.IsAttached(lookForProperties, lookForEvents))
					return true;
			}
			
			return false;
		}
		
		public static bool IsAttached(this IField field, bool lookForProperties, bool lookForEvents)
		{
			if (!field.IsPublic || !field.IsStatic || !field.IsReadOnly || field.ReturnType == null)
				return false;
			
			bool foundMethod = false;
			
			if (lookForProperties && field.ReturnType.FullName == "System.Windows.DependencyProperty") {
				if (field.Name.Length <= "Property".Length)
					return false;
				if (!field.Name.EndsWith("Property", StringComparison.Ordinal))
					return false;
				
				string fieldName = field.Name.Remove(field.Name.Length - "Property".Length);
				
				foreach (IMethod method in field.DeclaringTypeDefinition.Methods) {
					if (!method.IsPublic || !method.IsStatic || method.Name.Length <= 3)
						continue;
					if (!method.Name.StartsWith("Get", StringComparison.Ordinal) && !method.Name.StartsWith("Set", StringComparison.Ordinal))
						continue;
					foundMethod = method.Name.Remove(0, 3) == fieldName;
					if (foundMethod)
						return true;
				}
			}
			
			if (lookForEvents && !foundMethod && field.ReturnType.FullName == "System.Windows.RoutedEvent") {
				if (field.Name.Length <= "Event".Length)
					return false;
				if (!field.Name.EndsWith("Event", StringComparison.Ordinal))
					return false;
				
				return true;
			}
			
			return false;
		}
		
		public static T PopOrDefault<T>(this Stack<T> stack)
		{
			if (stack.Count > 0)
				return stack.Pop();
			
			return default(T);
		}
		
		public static Brush ToBrush(this Color color)
		{
			var b = new SolidColorBrush(color);
			b.Freeze();
			return b;
		}
	}
}
