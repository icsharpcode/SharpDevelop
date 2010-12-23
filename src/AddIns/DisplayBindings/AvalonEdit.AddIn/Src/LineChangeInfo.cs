// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
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
		public static readonly LineChangeInfo Empty = new LineChangeInfo(ChangeType.None, "");
		
		ChangeType change;
		
		public ChangeType Change {
			get { return change; }
			set { change = value; }
		}
		
		string deletedLinesAfterThisLine;
		
		public string DeletedLinesAfterThisLine {
			get { return deletedLinesAfterThisLine; }
			set { deletedLinesAfterThisLine = value; }
		}
		
		public LineChangeInfo(ChangeType change, string deletedLinesAfterThisLine)
		{
			this.change = change;
			this.deletedLinesAfterThisLine = deletedLinesAfterThisLine;
		}
		
		#region Equals and GetHashCode implementation
		public override bool Equals(object obj)
		{
			return (obj is LineChangeInfo) && Equals((LineChangeInfo)obj);
		}
		
		public bool Equals(LineChangeInfo other)
		{
			return this.change == other.change && this.deletedLinesAfterThisLine == other.deletedLinesAfterThisLine;
		}
		
		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked {
				hashCode += 1000000007 * change.GetHashCode();
				if (deletedLinesAfterThisLine != null)
					hashCode += 1000000009 * deletedLinesAfterThisLine.GetHashCode();
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
