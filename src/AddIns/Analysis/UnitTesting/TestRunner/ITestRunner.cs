// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
