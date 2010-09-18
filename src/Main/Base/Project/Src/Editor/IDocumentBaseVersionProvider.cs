// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Editor
{
	public interface IDocumentBaseVersionProvider
	{
		/// <summary>
		/// Provides the BASE-Version for a file. This can be either the file saved
		/// to disk or a base version provided by any VCS.
		/// </summary>
		Stream OpenBaseVersion(string fileName);
	}
	
	public interface IDiffProvider
	{
		Stream GetDiff(string fileName, ITextBuffer modifiedBuffer);
	}
	
	public class VersioningServices
	{
		public static readonly VersioningServices Instance = new VersioningServices();
		
		List<IDocumentBaseVersionProvider> baseVersionProviders;
		
		public List<IDocumentBaseVersionProvider> BaseVersionProviders {
			get {
				if (baseVersionProviders == null)
					baseVersionProviders = AddInTree.BuildItems<IDocumentBaseVersionProvider>("/Workspace/BaseVersionProviders", this, false);
				
				return baseVersionProviders;
			}
		}
		
		List<IDiffProvider> diffProviders;
		
		public List<IDiffProvider> DiffProviders {
			get {
				if (diffProviders == null)
					diffProviders = AddInTree.BuildItems<IDiffProvider>("/Workspace/DiffProviders", this, false);
				
				return diffProviders;
			}
		}
	}
	
	public interface IChangeWatcher : IDisposable
	{
		event EventHandler ChangeOccurred;
		/// <summary>
		/// Returns the change information for a given line.
		/// Pass null to get the changes before the first line.
		/// </summary>
		LineChangeInfo GetChange(IDocumentLine line);
		void Initialize(IDocument document);
	}
	
	public enum ChangeType
	{
		None,
		Added,
		Modified,
		Unsaved
	}
	
	public struct LineChangeInfo : IEquatable<LineChangeInfo>
	{
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
