// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Threading;

namespace ICSharpCode.PackageManagement.Scripting
{
	public class PackageManagementThread : IThread
	{
		Thread thread;
		
		public PackageManagementThread(ThreadStart threadStart)
		{
			thread = new Thread(threadStart);
		}
		
		public void Start()
		{
			thread.Start();
		}
		
		public bool Join(int milliescondsTimeout)
		{
			return thread.Join(milliescondsTimeout);
		}
	}
}
