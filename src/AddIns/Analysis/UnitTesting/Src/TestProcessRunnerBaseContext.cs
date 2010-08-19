// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Util;

namespace ICSharpCode.UnitTesting
{
	public class TestProcessRunnerBaseContext
	{		
		IUnitTestProcessRunner processRunner;
		ITestResultsMonitor testResultsMonitor;
		IFileSystem fileSystem;
		IUnitTestMessageService messageService;
		
		public TestProcessRunnerBaseContext()
			: this(new UnitTestProcessRunner(),
				new TestResultsMonitor(),
				new UnitTestFileService(),
				new UnitTestMessageService())
		{
		}
				
		public TestProcessRunnerBaseContext(IUnitTestProcessRunner processRunner,
			ITestResultsMonitor testResultsMonitor,
			IFileSystem fileSystem,
			IUnitTestMessageService messageService)
		{
			this.processRunner = processRunner;
			this.testResultsMonitor = testResultsMonitor;
			this.fileSystem = fileSystem;
			this.messageService = messageService;
		}
		
		public IUnitTestProcessRunner TestProcessRunner {
			get { return processRunner; }
		}
		
		public ITestResultsMonitor TestResultsMonitor {
			get { return testResultsMonitor; }
		}
		
		public IFileSystem FileSystem {
			get { return fileSystem; }
		}
		
		public IUnitTestMessageService MessageService {
			get { return messageService; }
		}
	}
}
