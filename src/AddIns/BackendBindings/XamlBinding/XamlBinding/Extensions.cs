// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop;
using System;
using System.Linq;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.XmlEditor;

namespace ICSharpCode.XamlBinding
{
	public static class Extensions
	{
		public static string[] Split(this string s, StringSplitOptions options, params char[] delimiters)
		{
			return s.Split(delimiters, options);
		}
		
		public static bool EndsWith(this string s, StringComparison comparison, params char[] characters)
		{
			foreach (var c in characters) {
				if (s.EndsWith(c.ToString(), comparison))
					return true;
			}
			
			return false;
		}
		
		public static QualifiedName LastOrDefault(this QualifiedNameCollection coll)
		{
			if (coll.Count > 0)
				return coll[coll.Count - 1];
			
			return null;
		}
	}
}
