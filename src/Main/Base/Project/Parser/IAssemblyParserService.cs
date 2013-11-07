// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Threading;
using System.Threading.Tasks;

using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;

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
		IUnresolvedAssembly GetAssembly(FileName fileName, CancellationToken cancellationToken = default(CancellationToken));
		
		/// <summary>
		/// Loads the specified assembly file from disk.
		/// </summary>
		Task<IUnresolvedAssembly> GetAssemblyAsync(FileName fileName, CancellationToken cancellationToken = default(CancellationToken));
		
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
