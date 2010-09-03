// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using System.Collections.Generic;

namespace ICSharpCode.XamlBinding
{
	public class PropertyPathTokenizer
	{
		string value;
		int offset;
		
		public static readonly char[] ControlChars = new char[] { '.', ',', '(', ')', '[', ']', '/' };
		
		PropertyPathTokenizer(string value)
		{
			this.value = value;
			this.offset = 0;
		}
		
		bool NextToken(out string token)
		{
			token = "";
			
			if (MoveToNext()) {
				switch (value[offset]) {
					case '.':
					case '(':
					case ')':
					case '[':
					case ']':
					case ',':
					case '/':
						token = value[offset].ToString();
						offset++;
						return true;
					default:
						string text = "";
						while (!AtEnd() && char.IsLetterOrDigit(value[offset])) {
							text += value[offset];
							offset++;
						}
						
						token = text;
						return true;
				}
			}
			
			return false;
		}
		
		bool MoveToNext()
		{
			// skip all invalid chars
			while (!AtEnd() && !char.IsLetterOrDigit(value[offset]) && !ControlChars.Contains(value[offset]))
				offset++;
			
			return !AtEnd();
		}
		
		bool AtEnd()
		{
			return offset >= value.Length;
		}
		
		public static IEnumerable<string> Tokenize(string value)
		{
			if (value == null)
				throw new ArgumentNullException("value");
			
			PropertyPathTokenizer tokenizer = new PropertyPathTokenizer(value);
			
			string token;
			
			while (tokenizer.NextToken(out token))
				yield return token;
		}
	}
}
