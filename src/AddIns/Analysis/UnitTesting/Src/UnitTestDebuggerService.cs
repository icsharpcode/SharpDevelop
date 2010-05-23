// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
