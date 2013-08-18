// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
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
		/// Creates a compilation for the specified assembly.
		/// </summary>
		ICompilation CreateCompilationForAssembly(IAssemblyModel assembly);
	}
	
	public interface IAssemblySearcher
	{
		FileName FindAssembly(DomAssemblyName fullName);
	}
	
	public class DefaultAssemblySearcher : IAssemblySearcher
	{
		FileName mainAssemblyFileName;
		DirectoryName baseDirectory;
		
		public DefaultAssemblySearcher(FileName mainAssemblyFileName)
		{
			if (mainAssemblyFileName == null)
				throw new ArgumentNullException("mainAssemblyFileName");
			this.mainAssemblyFileName = mainAssemblyFileName;
			this.baseDirectory = mainAssemblyFileName.GetParentDirectory();
		}
		
		public FileName FindAssembly(DomAssemblyName fullName)
		{
			throw new NotImplementedException();
		}
	}
}
