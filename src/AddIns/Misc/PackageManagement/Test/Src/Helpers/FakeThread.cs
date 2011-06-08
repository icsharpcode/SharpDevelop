// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Threading;
using ICSharpCode.PackageManagement.Scripting;

namespace PackageManagement.Tests.Helpers
{
	public class FakeThread : IThread
	{
		public bool IsStartCalled;
		public bool IsJoinCalled;
		
		public void Start()
		{
			IsStartCalled = true;
		}
		
		public int TimeoutPassedToJoin;
		public bool JoinReturnValue = true;
		
		public bool Join(int milliescondsTimeout)
		{
			IsJoinCalled = true;
			TimeoutPassedToJoin = milliescondsTimeout;
			return JoinReturnValue;
		}
	}
}
