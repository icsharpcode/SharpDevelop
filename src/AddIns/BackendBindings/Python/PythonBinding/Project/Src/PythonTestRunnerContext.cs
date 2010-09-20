// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Scripting;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.PythonBinding
{
	public class PythonTestRunnerContext : TestProcessRunnerBaseContext
	{
		PythonAddInOptions options;
		PythonStandardLibraryPath pythonStandardLibraryPath;
		IScriptingFileService fileService;

		public PythonTestRunnerContext(IUnitTestProcessRunner processRunner,
			ITestResultsMonitor testResultsMonitor,
			IUnitTestMessageService messageService,
			PythonAddInOptions options,
			PythonStandardLibraryPath pythonStandardLibraryPath,
			IScriptingFileService fileService)
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
				new ScriptingFileService())
		{
		}
		
		public PythonAddInOptions Options {
			get { return options; }
		}
		
		public PythonStandardLibraryPath PythonStandardLibraryPath {
			get { return pythonStandardLibraryPath; }
		}

		public IScriptingFileService ScriptingFileService {
			get { return base.FileSystem as IScriptingFileService; }
		}
	}
}
