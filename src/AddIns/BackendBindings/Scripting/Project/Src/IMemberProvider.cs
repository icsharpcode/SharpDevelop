// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;

namespace ICSharpCode.Scripting
{
	/// <summary>
	/// Returns member names or global names for the console command line.
	/// </summary>
	public interface IMemberProvider
	{
		IList<string> GetMemberNames(string name);
		
		IList<string> GetGlobals(string name);
	}
}
