// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
