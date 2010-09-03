// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Data;
using ICSharpCode.SharpDevelop.Debugging;

namespace ICSharpCode.UnitTesting
{
	public interface IUnitTestDebuggerService
	{
		bool IsDebuggerLoaded { get; }
		IDebugger CurrentDebugger { get; }
	}
}
