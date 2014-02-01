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
