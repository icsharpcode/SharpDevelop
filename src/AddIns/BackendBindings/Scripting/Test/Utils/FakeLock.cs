// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.Scripting;

namespace ICSharpCode.Scripting.Tests.Utils
{
	public class FakeLock : ILock
	{
		public List<string> Lines;
		public bool IsDisposed;
		public int UnreadLineCountWhenLockCreated = -1;
		public int UnreadLineCountWhenLockDisposed = -1;
		
		public FakeLock(List<string> lines)
		{
			this.Lines = lines;
			UnreadLineCountWhenLockCreated = lines.Count;
		}
		
		public void Dispose()
		{
			UnreadLineCountWhenLockDisposed = Lines.Count;
			IsDisposed = true;
		}
	}
}
