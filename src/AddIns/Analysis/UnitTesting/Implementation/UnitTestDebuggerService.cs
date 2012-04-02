// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Debugging;

namespace ICSharpCode.UnitTesting
{
	public class UnitTestDebuggerService : IUnitTestDebuggerService
	{
		public bool IsDebuggerLoaded {
			get { return DebuggerService.IsDebuggerLoaded; }
		}
		
		public IDebugger CurrentDebugger {
			get { return DebuggerService.CurrentDebugger; }
		}
	}
}
