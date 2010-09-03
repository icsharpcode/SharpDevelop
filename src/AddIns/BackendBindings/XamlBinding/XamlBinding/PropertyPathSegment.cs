// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using System.Collections.Generic;

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
