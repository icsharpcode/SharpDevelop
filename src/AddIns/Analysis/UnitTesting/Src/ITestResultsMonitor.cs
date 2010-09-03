// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.UnitTesting
{
	public interface ITestResultsMonitor : IDisposable
	{
		event TestFinishedEventHandler TestFinished;
		
		string FileName { get; set; }
		
		void Stop();
		void Start();
		void Read();
		
		long InitialFilePosition { get; set; }
	}
}
