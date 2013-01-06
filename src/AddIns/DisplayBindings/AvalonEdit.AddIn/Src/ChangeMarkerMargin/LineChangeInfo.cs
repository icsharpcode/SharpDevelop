// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
