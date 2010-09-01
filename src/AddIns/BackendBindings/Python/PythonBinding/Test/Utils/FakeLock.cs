// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.PythonBinding;

namespace PythonBinding.Tests.Utils
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
