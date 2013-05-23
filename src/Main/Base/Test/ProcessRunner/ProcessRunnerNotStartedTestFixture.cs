// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NUnit.Framework;

namespace ICSharpCode.SharpDevelop.Tests
{
	/// <summary>
	/// Checks that the <see cref="ProcessRunner"/> responds
	/// correctly if it has not been started.
	/// </summary>
	[TestFixture]
	public class ProcessRunnerNotStartedTestFixture
	{
		ProcessRunner runner;
		
		[SetUp]
		public void Init()
		{
			runner = new ProcessRunner();
		}
		
		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void ExitCode()
		{
			int exit = runner.ExitCode;
		}
		
		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void WaitForExit()
		{
			runner.WaitForExit();
		}
		
		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void WaitForExitAsync()
		{
			runner.WaitForExitAsync();
		}
	}
}
