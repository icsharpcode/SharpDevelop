// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Threading;

namespace ICSharpCode.Scripting
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
