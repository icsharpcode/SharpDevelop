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
		ChangeType GetChange(IDocumentLine line);
		void Initialize(IDocument document);
	}
	
	public enum ChangeType
	{
		None,
		Added,
		Modified,
		Unsaved
	}
}
