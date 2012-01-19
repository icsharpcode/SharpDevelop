// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
		CecilLoader loader = new CecilLoader(true);
		ICompilation compilation = null;
		IUnresolvedAssembly mainAssem = null;
		
		public AssemblyAnalyzer(params string[] fileNames)
		{
			List<IUnresolvedAssembly> references = new List<IUnresolvedAssembly>();
			foreach (string fileName in fileNames) {
				if (mainAssem == null)
					mainAssem = loader.LoadAssemblyFile(fileName);
				else
					references.Add(loader.LoadAssemblyFile(fileName));
				}
			compilation = new SimpleCompilation(mainAssem, references);
		}
		
		public ReadOnlyCollection<AssemblyNode> Analyze()
		{
			AssemblyNode mainAssembly = new AssemblyNode();
			return new ReadOnlyCollection<AssemblyNode>(new AssemblyNode[] { mainAssembly });
		}
	}
}
