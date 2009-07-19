// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.AvalonEdit.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
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
		
		public static string Replace(this string str, int index, int length, string text)
		{
			if (str == null)
				throw new ArgumentNullException("str");
			if (index < 0 || index > str.Length)
				throw new ArgumentOutOfRangeException("index", index, "Value must be between 0 and " + str.Length);
			if (length < 0 || length > str.Length)
				throw new ArgumentOutOfRangeException("length", length, "Value must be between 0 and " + str.Length);
			if ((index + length) > str.Length)
				throw new ArgumentOutOfRangeException("index + length", index + length, "Value must be between 0 and " + str.Length);
			
			return str.Substring(0, index) + text + str.Substring(index + length);
		}
		
		public static bool Is(char value, params char[] chars)
		{
			foreach (var c in chars) {
				if (c == value)
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
		
		public static TKey GetKeyByValue<TKey, TValue>(this Dictionary<TKey, TValue> dict, TValue value)
		{
			foreach (var pair in dict) {
				if (pair.Value.Equals(value))
					return pair.Key;
			}
			
			return default(TKey);
		}
		
		public static int GetLineNumber(this XObject item)
		{
			return (item as IXmlLineInfo).LineNumber;
		}
		
		public static int GetLinePosition(this XObject item)
		{
			return (item as IXmlLineInfo).LinePosition;
		}
		
		public static bool IsInRange(this XObject item, Location begin, Location end)
		{
			return IsInRange(item, begin.Line, begin.Column, end.Line, end.Column);
		}
		
		public static bool IsInRange(this XObject item, int beginLine, int beginColumn, int endLine, int endColumn)
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
		
		public static bool IsCollectionType(this IClass c)
		{
			if (c == null)
				throw new ArgumentNullException("c");
			return c.ClassInheritanceTree.Any(cla => cla.FullyQualifiedName == "System.Collections.ICollection");
		}
		
		public static bool IsCollectionReturnType(this IReturnType type)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			if (type.GetUnderlyingClass() != null)
				return type.GetUnderlyingClass().IsCollectionType();
			
			return false;
		}
		
		public static bool IsListType(this IClass c)
		{
			if (c == null)
				throw new ArgumentNullException("c");
			return c.ClassInheritanceTree.Any(cla => cla.FullyQualifiedName == "System.Collections.IList");
		}
		
		public static bool IsListReturnType(this IReturnType type)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			if (type.GetUnderlyingClass() != null)
				return type.GetUnderlyingClass().IsListType();
			
			return false;
		}
		
		/// <remarks>Works only if fullyQualyfiedClassName is the name of a class!</remarks>
		public static bool DerivesFrom(this IClass myClass, string fullyQualyfiedClassName)
		{
			return myClass.ClassInheritanceTreeClassesOnly.Any(c => c.FullyQualifiedName == fullyQualyfiedClassName);
		}
		
		public static T PopOrDefault<T>(this Stack<T> stack)
		{
			if (stack.Count > 0)
				return stack.Pop();
			
			return default(T);
		}
	}
}