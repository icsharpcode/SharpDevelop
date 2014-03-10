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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.ClassBrowser;

namespace ICSharpCode.SharpDevelop.Parser
{
	/// <summary>
	/// Portions of parser service that deal with loading external assemblies for code completion.
	/// </summary>
	[SDService("SD.AssemblyParserService")]
	public interface IAssemblyParserService
	{
		/// <summary>
		/// Loads the specified assembly file from disk.
		/// </summary>
		/// <exception cref="IOException">Error loading the assembly</exception>
		/// <exception cref="BadImageFormatException">Invalid assembly file</exception>
		IUnresolvedAssembly GetAssembly(FileName fileName, bool includeInternalMembers = false, CancellationToken cancellationToken = default(CancellationToken));
		
		/// <summary>
		/// <code>using (AssemblyParserService.AvoidRedundantChecks())</code>
		/// Within the using block, the AssemblyParserService will only check once per assembly if the
		/// existing cached project content (if any) is up to date.
		/// Any additional accesses will return that cached project content without causing an update check.
		/// This applies only to the thread that called AvoidRedundantChecks() - other threads will
		/// perform update checks as usual.
		/// </summary>
		IDisposable AvoidRedundantChecks();
		
		/// <summary>
		/// Gets the directory for cached project contents.
		/// May return <c>null</c> if on-disk caching is disabled.
		/// </summary>
		string DomPersistencePath { get; }
		
		/// <summary>
		/// Refreshes the specified assembly.
		/// Raises the <see cref="AssemblyRefreshed"/> event if the assembly has changed since it was originally loaded. 
		/// </summary>
		/// <remarks>This method has no effect is the specified file is not a loaded assembly.</remarks>
		void RefreshAssembly(FileName fileName);
		
		event EventHandler<RefreshAssemblyEventArgs> AssemblyRefreshed;
		
		/// <summary>
		/// Creates an IAssemblyModel for the given assembly file.
		/// </summary>
		/// <exception cref="IOException">Error loading the assembly</exception>
		/// <exception cref="BadImageFormatException">Invalid assembly file</exception>
		IAssemblyModel GetAssemblyModel(FileName fileName, bool includeInternalMembers = false);
		
		/// <summary>
		/// Creates an <see cref="ICSharpCode.SharpDevelop.Dom.IAssemblyModel"/> from a file name and catches
		/// errors by showing messages to user.
		/// </summary>
		/// <returns>
		/// Created <see cref="ICSharpCode.SharpDevelop.Dom.IAssemblyModel"/> or <b>null</b>,
		/// if model couldn't be created.
		/// </returns>
		IAssemblyModel GetAssemblyModelSafe(FileName fileName, bool includeInternalMembers = false);
	}
	
	public interface IAssemblySearcher
	{
		FileName FindAssembly(DomAssemblyName fullName);
	}
	
	public class RefreshAssemblyEventArgs : EventArgs
	{
		readonly FileName fileName;
		readonly IUnresolvedAssembly oldAssembly;
		readonly IUnresolvedAssembly newAssembly;
		
		public RefreshAssemblyEventArgs(FileName fileName, IUnresolvedAssembly oldAssembly, IUnresolvedAssembly newAssembly)
		{
			if (fileName == null)
				throw new ArgumentNullException("fileName");
			this.fileName = fileName;
			this.oldAssembly = oldAssembly;
			this.newAssembly = newAssembly;
		}

		public FileName FileName {
			get {
				return fileName;
			}
		}

		public IUnresolvedAssembly OldAssembly {
			get {
				return oldAssembly;
			}
		}
		
		public IUnresolvedAssembly NewAssembly {
			get {
				return newAssembly;
			}
		}
	}
}
