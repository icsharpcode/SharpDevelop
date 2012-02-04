// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Reflection;

namespace ICSharpCode.TextTemplating
{
	public interface IAddIn
	{
		string PrimaryIdentity { get; }
		
		IEnumerable<IAddInRuntime> GetRuntimes();
	}
}
