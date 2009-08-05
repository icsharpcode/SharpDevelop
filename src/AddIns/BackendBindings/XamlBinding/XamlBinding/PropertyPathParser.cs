// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ICSharpCode.XamlBinding
{
	public class PropertyPathParser
	{
		public static IEnumerable<PropertyPathSegment> Parse(string text)
		{
			string token = "";
			
			bool inBrace = false;
			bool inIndexer = false;
			bool isSourceTraversal = false;
			
			string lastToken = "";
			
			foreach (string v in PropertyPathTokenizer.Tokenize(text)) {
				lastToken = v;
				
				if (v == "(") {
					inBrace = true;
					continue;
				}
				
				if (v == "[") {
					inIndexer = true;
					if (!inBrace && !string.IsNullOrEmpty(token)) {
						yield return new PropertyPathSegment(SegmentKind.PropertyOrType, token);
						token = "";
					}
					continue;
				}
				
				if (v == ")") {
					inBrace = false;
					if (!string.IsNullOrEmpty(token))
						yield return new PropertyPathSegment(SegmentKind.AttachedProperty, token);
					token = "";
					continue;
				}
				
				if (v == "]") {
					inIndexer = false;
					if (!inBrace && !string.IsNullOrEmpty(token)) {
						yield return new PropertyPathSegment(SegmentKind.Indexer, token);
						token = "";
					}
					continue;
				}
				
				if (inBrace)
					token += v;
				else if (inIndexer)
					token += v;
				else if (v == ".") {
					if (!string.IsNullOrEmpty(token))
						yield return new PropertyPathSegment(isSourceTraversal ? SegmentKind.SourceTraversal : SegmentKind.PropertyOrType, token);
					token = "";
					isSourceTraversal = false;
				} else if (v == "/") {
					token += v;
					isSourceTraversal = true;
				} else {
					token += v;
				}
				
				Debug.Print("inBrace: " + inBrace + " token: '" + token + "'");
			}
			
			Debug.Print("inBrace: " + inBrace + " token: '" + token + "' lastToken: '" + lastToken + "'");

			
			if (inBrace && !string.IsNullOrEmpty(token)) {
				yield return new PropertyPathSegment(SegmentKind.AttachedProperty, "(" + token.Trim('.', '/'));
			}
			if (!string.IsNullOrEmpty(lastToken)) {
				char c = lastToken.First();
				
				if (c == ')' || c == ']')
					yield break;
				
				if (PropertyPathTokenizer.ControlChars.Contains(c))
					yield return new PropertyPathSegment(SegmentKind.ControlChar, c.ToString());
				else if (!inBrace && !string.IsNullOrEmpty(token))
					yield return new PropertyPathSegment(isSourceTraversal ? SegmentKind.SourceTraversal : SegmentKind.PropertyOrType, token);
			}
		}
	}
}
