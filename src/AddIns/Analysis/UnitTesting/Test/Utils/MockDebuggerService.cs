// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.UnitTesting;

namespace UnitTesting.Tests.Utils
{
	public class MockDebuggerService : IUnitTestDebuggerService
	{
		bool debuggerLoaded;
		MockDebugger debugger = new MockDebugger();
		
		public bool IsDebuggerLoaded {
			get { return debuggerLoaded; }
			set { debuggerLoaded = value; }
		}
		
		public IDebugger CurrentDebugger {
			get { return debugger; }
		}
		
		public MockDebugger MockDebugger {
			get { return debugger; }
		}
	}
}
