// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.Core;
using ICSharpCode.Editor;
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
		void Initialize(TextDocument document, FileName fileName);
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
		
		public readonly int OldStartLineNumber;
		public readonly int NewStartLineNumber;
		public readonly int OldEndLineNumber;
		public readonly int NewEndLineNumber;
		
		public LineChangeInfo(ChangeType change, int oldStartLineNumber, int newStartLineNumber, int oldEndLineNumber, int newEndLineNumber)
		{
			this.change = change;
			this.OldStartLineNumber = oldStartLineNumber;
			this.NewStartLineNumber = newStartLineNumber;
			this.OldEndLineNumber = oldEndLineNumber;
			this.NewEndLineNumber = newEndLineNumber;
		}
		
		#region Equals and GetHashCode implementation
		public override bool Equals(object obj)
		{
			return (obj is LineChangeInfo) && Equals((LineChangeInfo)obj);
		}
		
		public bool Equals(LineChangeInfo other)
		{
			return this.change == other.change &&
				this.OldEndLineNumber == other.OldEndLineNumber &&
				this.NewStartLineNumber == other.NewStartLineNumber &&
				this.OldStartLineNumber == other.OldStartLineNumber &&
				this.NewEndLineNumber == other.NewEndLineNumber;
		}
		
		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked {
				hashCode += 1000000007 * change.GetHashCode();
				hashCode += 1000000009 * OldStartLineNumber.GetHashCode();
				hashCode += 1000000021 * NewStartLineNumber.GetHashCode();
				hashCode += 1000000033 * OldEndLineNumber.GetHashCode();
				hashCode += 1000000087 * NewEndLineNumber.GetHashCode();
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
