// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.AspNet.Mvc
{
	public interface IMvcClass
	{
		string FullName { get; }
		string Name { get; }
		string Namespace { get; }
		string BaseClassFullName { get; }
		string AssemblyLocation { get; }
		
		bool IsModelClass();
	}
}
