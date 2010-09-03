// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
