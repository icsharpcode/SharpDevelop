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
using System.Linq;

namespace ICSharpCode.XamlBinding
{
	public struct PropertyPathSegment : IEquatable<PropertyPathSegment>
	{
		public SegmentKind Kind;
		public string Content;
		
		public PropertyPathSegment(SegmentKind kind, string content)
		{
			this.Kind = kind;
			this.Content = content;
		}
		
		public override string ToString()
		{
			return "[PropertyPathSegment Kind: " + Kind + ", Content: " + Content + " ]";
		}
		
		public override bool Equals(object obj)
		{
			if (obj is PropertyPathSegment)
				return Equals((PropertyPathSegment)obj);
			return false;
		}
		
		[System.Security.SecuritySafeCriticalAttribute()]
		public override int GetHashCode()
		{
			return GetHashCode(this);
		}
		
		public bool Equals(PropertyPathSegment other)
		{
			if (other == null)
				return false;
			
			return other.Content == this.Content &&
				other.Kind == this.Kind;
		}
		
		public int GetHashCode(PropertyPathSegment obj)
		{
			return obj.Content.GetHashCode() ^ obj.Kind.GetHashCode();
		}
		
		public static bool operator ==(PropertyPathSegment lhs, PropertyPathSegment rhs)
		{
			return Equals(lhs, rhs);
		}
		
		public static bool operator !=(PropertyPathSegment lhs, PropertyPathSegment rhs)
		{
			return !(lhs == rhs);
		}
	}
}
