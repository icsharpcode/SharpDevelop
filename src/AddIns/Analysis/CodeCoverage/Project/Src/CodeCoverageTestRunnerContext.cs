// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.CodeCoverage
{
	public class CodeCoverageTestRunnerContext : TestProcessRunnerBaseContext
	{
		UnitTestingOptions options;
		
		public CodeCoverageTestRunnerContext()
			: this(new UnitTestProcessRunner(),
				new TestResultsMonitor(),
				new FileSystem(),
				new UnitTestMessageService(),
				new UnitTestingOptions())
		{
		}
				
		public CodeCoverageTestRunnerContext(IUnitTestProcessRunner processRunner,
			ITestResultsMonitor testResultsMonitor,
			ICSharpCode.CodeCoverage.IFileSystem fileSystem,
			IUnitTestMessageService messageService,
			UnitTestingOptions options)
			: base(processRunner, testResultsMonitor, fileSystem, messageService)
		{
			this.options = options;
		}
		
		public UnitTestingOptions Options {
			get { return options; }
		}
		
		public ICSharpCode.CodeCoverage.IFileSystem CodeCoverageFileSystem {
			get { return base.FileSystem as ICSharpCode.CodeCoverage.IFileSystem; }
		}
	}
}
