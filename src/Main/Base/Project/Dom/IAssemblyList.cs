// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// Base interface for assembly list implementations.
	/// </summary>
	public interface IAssemblyList
	{
		string Name { get; set; }
		IMutableModelCollection<IAssemblyModel> Assemblies { get; set; }
	}
	
	public class AssemblyList : IAssemblyList
	{
		public string Name { get; set; }
		public IMutableModelCollection<IAssemblyModel> Assemblies { get; set; }
		
		public AssemblyList()
		{
			Name = "<default>";
			Assemblies = new NullSafeSimpleModelCollection<IAssemblyModel>();
		}
	}
}
