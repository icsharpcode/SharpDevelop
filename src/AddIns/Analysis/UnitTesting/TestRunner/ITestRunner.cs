// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Util;

namespace ICSharpCode.UnitTesting
{
	public interface ITestRunner : IDisposable
	{
		event EventHandler<TestFinishedEventArgs> TestFinished;
		event EventHandler AllTestsFinished;
		event EventHandler<MessageReceivedEventArgs> MessageReceived;
		void Start(SelectedTests selectedTests);
		void Stop();
	}
}
