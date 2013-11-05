// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;

using System.Threading.Tasks;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Editor
{
	/// <summary>
	/// Provides the BASE-version for a given file - the original unmodified
	/// copy from the last commit.
	/// This interface is implemented by the version control AddIns.
	/// </summary>
	public interface IDocumentVersionProvider
	{
		/// <summary>
		/// Provides the BASE-Version for a file. This can be either the file saved
		/// to disk or a base version provided by any VCS.
		/// </summary>
		Task<Stream> OpenBaseVersionAsync(FileName fileName);
		
		/// <summary>
		/// Starts watching for changes to the BASE-version of the specified file.
		/// The callback delegate gets called whenever the BASE-version has changed.
		/// </summary>
		/// <returns>Returns a disposable that can be used to stop watching for changes.
		/// You must dispose the disposable to prevent a memory leak, the GC will
		/// not help out in this case!</returns>
		IDisposable WatchBaseVersionChanges(FileName fileName, EventHandler callback);
	}
	
	public class VersioningServices
	{
		public static readonly VersioningServices Instance = new VersioningServices();
		
		List<IDocumentVersionProvider> baseVersionProviders;
		
		public List<IDocumentVersionProvider> DocumentVersionProviders {
			get {
				if (baseVersionProviders == null)
					baseVersionProviders = AddInTree.BuildItems<IDocumentVersionProvider>("/Workspace/DocumentVersionProviders", this, false);
				
				return baseVersionProviders;
			}
		}
	}
}
