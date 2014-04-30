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
using System.Collections.Generic;
using System.Linq;

using System.Text;

namespace ICSharpCode.XamlBinding
{
	public class MarkupExtensionInfo : IEquatable<MarkupExtensionInfo>
	{
		public string ExtensionType { get; set; }
		public IList<AttributeValue> PositionalArguments { get; private set; }
		public IDictionary<string, AttributeValue> NamedArguments { get; private set; }
		
		public int StartOffset { get; set; }
		public int EndOffset { get; set; }
		public bool IsClosed { get; set; }
		
		public MarkupExtensionInfo()
			: this(string.Empty, new List<AttributeValue>(), new Dictionary<string, AttributeValue>(StringComparer.OrdinalIgnoreCase))
		{
		}
		
		public MarkupExtensionInfo(string type, IList<AttributeValue> posArgs, IDictionary<string, AttributeValue> namedArgs)
		{
			this.ExtensionType = type;
			this.PositionalArguments = posArgs;
			this.NamedArguments = namedArgs;
		}
		
		public override bool Equals(object obj)
		{
			return Equals(obj as MarkupExtensionInfo);
		}
		
		public override int GetHashCode()
		{
			unchecked {
				int hash = ExtensionType.GetHashCode() ^
					StartOffset.GetHashCode() ^
					EndOffset.GetHashCode();
				
				foreach (var value in PositionalArguments)
					hash ^= value.GetHashCode();
				
				foreach (var pair in NamedArguments)
					hash = hash ^ pair.Key.GetHashCode() ^ pair.Value.GetHashCode();
				
				return hash;
			}
		}
		
		public bool Equals(MarkupExtensionInfo other)
		{
			if (ReferenceEquals(other, null))
				return false;
			
			if (other.ExtensionType != ExtensionType)
				return false;
			
			if (other.StartOffset != StartOffset)
				return false;
			
			if (other.EndOffset != EndOffset)
				return false;
			
			if (other.NamedArguments.Count != NamedArguments.Count)
				return false;
			
			if (other.PositionalArguments.Count != PositionalArguments.Count)
				return false;
			
			for (int i = 0; i < PositionalArguments.Count; i++) {
				if (!PositionalArguments[i].Equals(other.PositionalArguments[i]))
					return false;
			}
			
			List<KeyValuePair<string, AttributeValue>> myItems = NamedArguments.ToList();
			List<KeyValuePair<string, AttributeValue>> otherItems = other.NamedArguments.ToList();
			
			for (int i = 0; i < myItems.Count; i++) {
				if (myItems[i].Key != otherItems[i].Key)
					return false;
				
				if (!myItems[i].Value.Equals(otherItems[i].Value))
					return false;
			}
			
			return true;
		}
		
		public static bool operator ==(MarkupExtensionInfo lhs, MarkupExtensionInfo rhs)
		{
			if (object.ReferenceEquals(lhs, rhs))
				return true;
			
			if (((object)lhs) == null)
				return false;
			
			return lhs.Equals(rhs);
		}
		
		public static bool operator !=(MarkupExtensionInfo lhs, MarkupExtensionInfo rhs)
		{
			return !(lhs == rhs);
		}
		
		public override string ToString()
		{
			StringBuilder builder = new StringBuilder("{");
			
			foreach (var pos in PositionalArguments)
				builder.Append(pos + ",");
			
			builder.Append("}");
			
			string posArgs = builder.ToString();
			
			builder = new StringBuilder("{");
			
			foreach (var pair in NamedArguments)
				builder.Append(pair.Key + "=" + pair.Value + ",");
			
			builder.Append("}");
			
			string namedArgs = builder.ToString();
			
			return string.Format("[MarkupExtensionInfo Type={0}, Start={1}, End={2}, Pos={3}, Named={4}]",
			                    ExtensionType, StartOffset, EndOffset, posArgs, namedArgs);
		}
	}
	
	public class AttributeValue : IEquatable<AttributeValue>
	{
		string stringValue;
		MarkupExtensionInfo extensionValue;
		
		public int StartOffset { get; set; }
		
		public int EndOffset {
			get {
				if (IsString)
					return StartOffset + StringValue.Length;
				
				return extensionValue.EndOffset;
			}
		}
		
		public bool IsClosed {
			get { return IsString || extensionValue.IsClosed; }
		}
		
		public bool IsString {
			get { return stringValue != null; }
		}
		
		public string StringValue {
			get { return stringValue; }
		}
		
		public MarkupExtensionInfo ExtensionValue {
			get { return extensionValue; }
		}
		
		public AttributeValue(string value)
		{
			if (value == null)
				throw new ArgumentNullException("value");
			
			this.stringValue = value;
		}
		
		public AttributeValue(MarkupExtensionInfo value)
		{
			if (value == null)
				throw new ArgumentNullException("value");
			
			this.extensionValue = value;
		}
		
		public override bool Equals(object obj)
		{
			if (obj is string)
				return Equals(obj as string);
			
			if (obj is AttributeValue)
				return Equals(obj as AttributeValue);
			
			return false;
		}
		
		public override int GetHashCode()
		{
			int hash = StartOffset.GetHashCode() ^ EndOffset.GetHashCode();
			
			if (IsString)
				return hash ^ StringValue.GetHashCode();
			
			return hash ^ ExtensionValue.GetHashCode();
		}
		
		public bool Equals(AttributeValue other)
		{
			if (ReferenceEquals(other, null))
				return false;
			
			if (IsString != other.IsString)
				return false;
			
			if (StartOffset != other.StartOffset)
				return false;
			
			if (EndOffset != other.EndOffset)
				return false;
			
			if (IsString)
				return other.StringValue == StringValue;
			else
				return other.ExtensionValue == extensionValue;
		}
		
		public static bool operator ==(AttributeValue lhs, AttributeValue rhs)
		{
			if (object.ReferenceEquals(lhs, rhs))
				return true;
			
			if (((object)rhs) == null)
				return false;
			
			return rhs.Equals(lhs);
		}
		
		public static bool operator !=(AttributeValue lhs, AttributeValue rhs)
		{
			return !(lhs == rhs);
		}
		
		public override string ToString()
		{
			if (IsString)
				return string.Format("[AttributeValue Start={1}, End={2}, String={0}]", StringValue, StartOffset, EndOffset);
			
			return string.Format("[AttributeValue Start={1}, End={2}, Extension={0}]", ExtensionValue, StartOffset, EndOffset);
		}
	}
}
