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
