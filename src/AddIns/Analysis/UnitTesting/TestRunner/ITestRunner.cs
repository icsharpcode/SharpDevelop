// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.UnitTesting
{
	public interface ITestRunner : IDisposable
	{
		event EventHandler<TestFinishedEventArgs> TestFinished;
		
		/// <summary>
		/// Runs the unit tests.
		/// </summary>
		/// <param name="selectedTests">The tests that were selected for execution.</param>
		/// <param name="progress">Object for reporting progress within the run (percentage value between 0 and 1)</param>
		/// <param name="output">Output text writer </param>
		/// <param name="cancellationToken"></param>
		/// <returns>Returns a task that gets marked as completed when all tests have finished execution.</returns>
		Task RunAsync(IEnumerable<ITest> selectedTests, IProgress<double> progress, TextWriter output, CancellationToken cancellationToken);
	}
}
