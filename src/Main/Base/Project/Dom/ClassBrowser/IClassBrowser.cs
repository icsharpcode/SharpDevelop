// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.TreeView;

namespace ICSharpCode.SharpDevelop.Dom.ClassBrowser
{
	public interface IClassBrowser
	{
		ICollection<SharpTreeNode> SpecialNodes { get; }
		AssemblyList AssemblyList { get; set; }
	}
	
	public class AssemblyList
	{
		public string Name { get; set; }
		public IMutableModelCollection<IAssemblyModel> Assemblies { get; set; }
		
		public AssemblyList()
		{
			Name = "<default>";
			Assemblies = new SimpleModelCollection<IAssemblyModel>();
		}
	}
}
