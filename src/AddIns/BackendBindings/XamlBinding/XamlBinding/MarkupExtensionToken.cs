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

namespace ICSharpCode.XamlBinding
{
	public sealed class MarkupExtensionToken
	{
		public MarkupExtensionTokenKind Kind { get; private set; }
		public string Value { get; private set; }
		
		public int StartOffset { get; set; }
		public int EndOffset { get; set; }
		
		public MarkupExtensionToken(MarkupExtensionTokenKind kind, string value)
		{
			this.Kind = kind;
			this.Value = value;
		}
		
		public override string ToString()
		{
			return "[" + Kind + " " + Value + "]";
		}
		
		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked {
				hashCode += 1000000007 * Kind.GetHashCode();
				if (Value != null) hashCode += 1000000009 * Value.GetHashCode(); 
			}
			return hashCode;
		}
		
		public override bool Equals(object obj)
		{
			MarkupExtensionToken other = obj as MarkupExtensionToken;
			if (other == null) return false; 
			return this.Kind == other.Kind && this.Value == other.Value;
		}
	}
}
