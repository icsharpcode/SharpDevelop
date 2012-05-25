// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public interface CodeElements : IEnumerable
	{
		new IEnumerator GetEnumerator();
		
		int Count { get; }
		
		CodeElement Item(object index);
	}
}
