// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using ICSharpCode.XmlEditor;

namespace ICSharpCode.XamlBinding
{
	public static class Extensions
	{
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
		
		public static IEnumerable<ICompletionItem> RemoveEvents(this IEnumerable<ICompletionItem> list)
		{
			foreach (var item in list) {
				if (item is XamlCodeCompletionItem) {
					var comItem = item as XamlCodeCompletionItem;
					if (!(comItem.Entity is IEvent))
						yield return item;
				} else yield return item;
			}
		}
		
		public static IEnumerable<ICompletionItem> RemoveProperties(this IEnumerable<ICompletionItem> list)
		{
			foreach (var item in list) {
				if (item is XamlCodeCompletionItem) {
					var comItem = item as XamlCodeCompletionItem;
					if (!(comItem.Entity is IProperty))
						yield return item;
				} else yield return item;
			}
		}
	}
}
