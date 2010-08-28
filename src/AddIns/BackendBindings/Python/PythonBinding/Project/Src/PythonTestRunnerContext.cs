// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.PythonBinding
{
	public class PythonTestRunnerContext : TestProcessRunnerBaseContext
	{
		PythonAddInOptions options;
		PythonStandardLibraryPath pythonStandardLibraryPath;
		IPythonFileService fileService;

		public PythonTestRunnerContext(IUnitTestProcessRunner processRunner,
			ITestResultsMonitor testResultsMonitor,
			IUnitTestMessageService messageService,
			PythonAddInOptions options,
			PythonStandardLibraryPath pythonStandardLibraryPath,
			IPythonFileService fileService)
			: base(processRunner, testResultsMonitor, fileService, messageService)
		{
			this.options = options;
			this.pythonStandardLibraryPath = pythonStandardLibraryPath;
			this.fileService = fileService;
		}
		
		public PythonTestRunnerContext()
			: this(new UnitTestProcessRunner(), 
				new TestResultsMonitor(), 
				new UnitTestMessageService(),
				new PythonAddInOptions(),
				new PythonStandardLibraryPath(),
				new PythonFileService())
		{
		}
		
		public PythonAddInOptions Options {
			get { return options; }
		}
		
		public PythonStandardLibraryPath PythonStandardLibraryPath {
			get { return pythonStandardLibraryPath; }
		}

		public IPythonFileService PythonFileService {
			get { return base.FileSystem as IPythonFileService; }
		}
	}
}
