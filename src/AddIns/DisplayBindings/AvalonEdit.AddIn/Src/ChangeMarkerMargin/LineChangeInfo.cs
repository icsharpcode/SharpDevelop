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
using ICSharpCode.SharpDevelop.Widgets.MyersDiff;

namespace ICSharpCode.AvalonEdit.AddIn
{
	public struct LineChangeInfo : IEquatable<LineChangeInfo>
	{
		public static readonly LineChangeInfo EMPTY = new LineChangeInfo(ChangeType.None, 1, 1);
		
		ChangeType change;
		
		public ChangeType Change {
			get { return change; }
			set { change = value; }
		}
		
		int oldStartLineNumber;
		
		public int OldStartLineNumber {
			get { return oldStartLineNumber; }
		}
		
		int oldEndLineNumber;
		
		public int OldEndLineNumber {
			get { return oldEndLineNumber; }
		}
		
		public LineChangeInfo(ChangeType change, int oldStartLineNumber, int oldEndLineNumber)
		{
			this.change = change;
			this.oldStartLineNumber = oldStartLineNumber;
			this.oldEndLineNumber = oldEndLineNumber;
		}
		
		#region Equals and GetHashCode implementation
		public override bool Equals(object obj)
		{
			return (obj is LineChangeInfo) && Equals((LineChangeInfo)obj);
		}
		
		public bool Equals(LineChangeInfo other)
		{
			return this.change == other.change && this.oldStartLineNumber == other.oldStartLineNumber && this.oldEndLineNumber == other.oldEndLineNumber;
		}
		
		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked {
				hashCode += 1000000007 * change.GetHashCode();
				hashCode += 1000000009 * oldStartLineNumber.GetHashCode();
				hashCode += 1000000021 * oldEndLineNumber.GetHashCode();
			}
			return hashCode;
		}
		
		public static bool operator ==(LineChangeInfo lhs, LineChangeInfo rhs)
		{
			return lhs.Equals(rhs);
		}
		
		public static bool operator !=(LineChangeInfo lhs, LineChangeInfo rhs)
		{
			return !(lhs == rhs);
		}
		#endregion

	}
}
