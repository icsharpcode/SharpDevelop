// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.AvalonEdit.AddIn
{
	public interface IChangeWatcher : IDisposable
	{
		event EventHandler ChangeOccurred;
		
		/// <summary>
		/// Returns the change information for a given line.
		/// Pass 0 to get the changes before the first line.
		/// </summary>
		LineChangeInfo GetChange(int lineNumber);
		void Initialize(IDocument document);
		string GetOldVersionFromLine(int lineNumber, out int newStartLine, out bool added);
		bool GetNewVersionFromLine(int lineNumber, out int offset, out int length);
		IDocument CurrentDocument { get; }
		IDocument BaseDocument { get; }
	}
	
	public enum ChangeType
	{
		None,
		Added,
		Deleted,
		Modified,
		Unsaved
	}
	
	public struct LineChangeInfo : IEquatable<LineChangeInfo>
	{
		public static readonly LineChangeInfo Empty = new LineChangeInfo(ChangeType.None, -1, -1, -1, -1);
		
		ChangeType change;
		
		public ChangeType Change {
			get { return change; }
			set { change = value; }
		}
		
		int oldStartLineNumber;
		
		public int OldStartLineNumber {
			get { return oldStartLineNumber; }
			set { oldStartLineNumber = value; }
		}
		
		int newStartLineNumber;
		
		public int NewStartLineNumber {
			get { return newStartLineNumber; }
			set { newStartLineNumber = value; }
		}
		
		int oldEndLineNumber;
		
		public int OldEndLineNumber {
			get { return oldEndLineNumber; }
			set { oldEndLineNumber = value; }
		}
		
		int newEndLineNumber;
		
		public int NewEndLineNumber {
			get { return newEndLineNumber; }
			set { newEndLineNumber = value; }
		}
		
		public LineChangeInfo(ChangeType change, int oldStartLineNumber, int newStartLineNumber, int oldEndLineNumber, int newEndLineNumber)
		{
			this.change = change;
			this.oldStartLineNumber = oldStartLineNumber;
			this.newStartLineNumber = newStartLineNumber;
			this.oldEndLineNumber = oldEndLineNumber;
			this.newEndLineNumber = newEndLineNumber;
		}
		
		#region Equals and GetHashCode implementation
		public override bool Equals(object obj)
		{
			return (obj is LineChangeInfo) && Equals((LineChangeInfo)obj);
		}
		
		public bool Equals(LineChangeInfo other)
		{
			return this.change == other.change &&
				this.oldEndLineNumber == other.oldEndLineNumber &&
				this.newStartLineNumber == other.newStartLineNumber &&
				this.oldStartLineNumber == other.oldStartLineNumber &&
				this.newEndLineNumber == other.newEndLineNumber;
		}
		
		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked {
				hashCode += 1000000007 * change.GetHashCode();
				hashCode += 1000000009 * oldStartLineNumber.GetHashCode();
				hashCode += 1000000021 * newStartLineNumber.GetHashCode();
				hashCode += 1000000033 * oldEndLineNumber.GetHashCode();
				hashCode += 1000000087 * newEndLineNumber.GetHashCode();
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
