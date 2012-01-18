// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.CodeQuality.Engine.Dom;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;

namespace ICSharpCode.CodeQuality.Engine
{
	/// <summary>
	/// Description of AssemblyAnalyzer.
	/// </summary>
	public class AssemblyAnalyzer
	{
		public IEnumerable<AssemblyNode> Assemblies { get; private set; }
		
		public AssemblyAnalyzer()
		{
			
		}
	}
}
