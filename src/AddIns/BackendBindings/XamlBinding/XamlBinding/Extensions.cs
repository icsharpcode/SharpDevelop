// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Xml;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using ICSharpCode.XmlEditor;

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
		
		public static ElementWrapper ToWrapper(this AXmlElement element)
		{
			return new ElementWrapper(element);
		}
		
		public static AttributeWrapper ToWrapper(this AXmlAttribute attribute)
		{
			return new AttributeWrapper(attribute);
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
		
		public static QualifiedNameWithLocation ToQualifiedName(this AXmlAttribute thisValue)
		{
			return new QualifiedNameWithLocation(thisValue.LocalName, thisValue.Namespace, thisValue.Prefix, thisValue.StartOffset);
		}
		
		public static QualifiedNameWithLocation ToQualifiedName(this AXmlElement thisValue)
		{
			return new QualifiedNameWithLocation(thisValue.LocalName, thisValue.Namespace, thisValue.Prefix, thisValue.StartOffset);
		}
		
		public static string GetWordBeforeCaretExtended(this ITextEditor editor)
		{
			IDocumentLine line = editor.Document.GetLine(editor.Caret.Line);
			int index = Math.Min(editor.Caret.Column - 1, line.Text.Length);
			string text = line.Text.Substring(0, index);
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
		
		public static Location GetLocation(this IXmlLineInfo thisValue)
		{
			return new Location(thisValue.GetLinePosition(), thisValue.GetLineNumber());
		}
		
		public static bool IsInRange(this IXmlLineInfo item, Location begin, Location end)
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
		
		public static bool IsCollectionType(this IClass thisValue)
		{
			if (thisValue == null)
				throw new ArgumentNullException("thisValue");
			return thisValue.ClassInheritanceTree.Any(cla => cla.FullyQualifiedName == "System.Collections.ICollection");
		}
		
		public static bool IsCollectionReturnType(this IReturnType type)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			if (type.GetUnderlyingClass() != null)
				return type.GetUnderlyingClass().IsCollectionType();
			
			return false;
		}
		
		public static bool IsListType(this IClass thisValue)
		{
			if (thisValue == null)
				throw new ArgumentNullException("thisValue");
			return thisValue.ClassInheritanceTree.Any(cla => cla.FullyQualifiedName == "System.Collections.IList");
		}
		
		public static bool IsListReturnType(this IReturnType type)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			if (type.GetUnderlyingClass() != null)
				return type.GetUnderlyingClass().IsListType();
			
			return false;
		}
		
		public static bool HasAttached(this IClass thisValue, bool lookForProperties, bool lookForEvents)
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
			if (!field.IsPublic || !field.IsStatic || !field.IsReadonly || field.ReturnType == null)
				return false;
			
			bool foundMethod = false;
			
			if (lookForProperties && field.ReturnType.FullyQualifiedName == "System.Windows.DependencyProperty") {
				if (field.Name.Length <= "Property".Length)
					return false;
				if (!field.Name.EndsWith("Property", StringComparison.Ordinal))
					return false;
				
				string fieldName = field.Name.Remove(field.Name.Length - "Property".Length);
				
				foreach (IMethod method in field.DeclaringType.Methods) {
					if (!method.IsPublic || !method.IsStatic || method.Name.Length <= 3)
						continue;
					if (!method.Name.StartsWith("Get") && !method.Name.StartsWith("Set"))
						continue;
					foundMethod = method.Name.Remove(0, 3) == fieldName;
					if (foundMethod)
						return true;
				}
			}
			
			if (lookForEvents && !foundMethod && field.ReturnType.FullyQualifiedName == "System.Windows.RoutedEvent") {
				if (field.Name.Length <= "Event".Length)
					return false;
				if (!field.Name.EndsWith("Event", StringComparison.Ordinal))
					return false;
				
				return true;
			}
			
			return false;
		}
		
		/// <remarks>Works only if fullyQualifiedClassName is the name of a class!</remarks>
		public static bool DerivesFrom(this IClass myClass, string fullyQualifiedClassName)
		{
			return myClass.ClassInheritanceTreeClassesOnly.Any(c => c.FullyQualifiedName == fullyQualifiedClassName);
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
