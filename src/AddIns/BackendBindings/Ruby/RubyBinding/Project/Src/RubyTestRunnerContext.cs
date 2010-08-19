// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
