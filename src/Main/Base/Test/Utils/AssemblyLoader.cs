// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.SharpDevelop
{
	public static class AssemblyLoader
	{
		public static readonly IUnresolvedAssembly Corlib = new CecilLoader { LazyLoad = true }.LoadAssemblyFile(typeof(object).Assembly.Location);
	}
}
