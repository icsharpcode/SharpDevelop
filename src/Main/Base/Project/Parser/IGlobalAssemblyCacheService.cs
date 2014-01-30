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
