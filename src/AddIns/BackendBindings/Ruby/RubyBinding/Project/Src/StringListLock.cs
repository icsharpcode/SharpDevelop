// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Threading;

namespace ICSharpCode.RubyBinding
{
	public class StringListLock : ILock
	{
		List<string> lines;
		
		public StringListLock(List<string> lines)
		{
			this.lines = lines;
			Lock();
		}
		
		void Lock()
		{
			Monitor.Enter(lines);
		}
		
		public void Dispose()
		{
			Unlock();
		}
		
		void Unlock()
		{
			Monitor.Exit(lines);
		}
	}
}
