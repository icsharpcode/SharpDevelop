// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.RubyBinding
{
	public class RubyTestRunnerContext : TestProcessRunnerBaseContext
	{
		RubyAddInOptions options;
		IRubyFileService fileService;
		
		public RubyTestRunnerContext()
			: this(new UnitTestProcessRunner(),
				new TestResultsMonitor(),
				new RubyAddInOptions(),
				new RubyFileService(),
				new UnitTestMessageService())
		{
		}
		
		public RubyTestRunnerContext(IUnitTestProcessRunner processRunner,
			ITestResultsMonitor testResultsMonitor,
			RubyAddInOptions options,
			IRubyFileService fileService,
			IUnitTestMessageService messageService)
			: base(processRunner, testResultsMonitor, fileService, messageService)
		{
			this.options = options;
			this.fileService = fileService;
		}
		
		public RubyAddInOptions Options {
			get { return options; }
		}
		
		public IRubyFileService RubyFileService {
			get { return fileService; }
		}
	}
}
