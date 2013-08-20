// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;
using Mono.Cecil;
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
		/// Creates a compilation for the specified assembly.
		/// </summary>
		ICompilation CreateCompilationForAssembly(IAssemblyModel assembly, bool includeInternalMembers = false);
		
		/// <summary>
		/// Creates a compilation for the specified assembly.
		/// </summary>
		ICompilation CreateCompilationForAssembly(FileName assembly, bool includeInternalMembers = false);
		
		/// <summary>
		/// Creates an IAssemblyModel for the given assembly file.
		/// </summary>
		IAssemblyModel GetAssemblyModel(FileName fileName, bool includeInternalMembers = false);
		
		/// <summary>
		/// Creates an <see cref="ICSharpCode.SharpDevelop.Dom.IAssemblyModel"/> from a file name and catches
		/// errors by showing messages to user.
		/// </summary>
		/// <param name="modelFactory">Model factory.</param>
		/// <param name="fileName">Assembly file name.</param>
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
}
