// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Parser
{
	/// <summary>
	/// Interface for global assembly cache service.
	/// </summary>
	[SDService("SD.GlobalAssemblyCache")]
	public interface IGlobalAssemblyCacheService
	{
		/// <summary>
		/// Gets whether the file name is within the GAC.
		/// </summary>
		bool IsGacAssembly(string fileName);
		
		/// <summary>
		/// Gets the names of all assemblies in the GAC.
		/// </summary>
		IEnumerable<DomAssemblyName> Assemblies { get; }
		
		/// <summary>
		/// Gets the file name for an assembly stored in the GAC.
		/// Returns null if the assembly cannot be found.
		/// </summary>
		FileName FindAssemblyInNetGac(DomAssemblyName reference);
		
		/// <summary>
		/// Gets the full display name of the GAC assembly of the specified short name.
		/// Returns null if the assembly cannot be found.
		/// </summary>
		DomAssemblyName FindBestMatchingAssemblyName(DomAssemblyName reference);
	}
}
