// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Util;

namespace ICSharpCode.UnitTesting
{
	public interface ITestRunner : IDisposable
	{
		event EventHandler<TestFinishedEventArgs> TestFinished;
		event EventHandler<MessageReceivedEventArgs> MessageReceived;
		
		Task RunAsync(IEnumerable<ITest> selectedTests, IProgress<double> progress, CancellationToken cancellationToken);
	}
}
