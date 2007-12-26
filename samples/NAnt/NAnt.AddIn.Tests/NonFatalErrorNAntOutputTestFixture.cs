using NUnit.Framework;
using ICSharpCode.NAnt;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using System;

namespace ICSharpCode.NAnt.Tests
{
	[TestFixture]
	public class NonFatalErrorNAntOutputTestFixture
	{
		[Test]
		public void Parse085()
		{
			TaskCollection tasks = NAntOutputParser.Parse(GetNAntOutput());
			
			Assert.AreEqual(1, tasks.Count, "Should be one task.");
		
			Task task = tasks[0];
			
			Assert.AreEqual("C:\\Projects\\dotnet\\Corsavy\\SharpDevelop\\src\\StandardAddIn.include", task.FileName, "Task filename is incorrect.");
			Assert.AreEqual(TaskType.Error, task.TaskType, "Should be a warning task.");
			Assert.AreEqual(93, task.Line, "Incorrect line number.");
			Assert.AreEqual(4, task.Column, "Incorrect col number.");
			Assert.AreEqual("Cannot delete directory 'C:\\Projects\\dotnet\\Corsavy\\SharpDevelop\\AddIns\\AddIns\\Misc\\Debugger.AddIn'. The directory does not exist.",
			                task.Description,
			                "Task description is wrong.");
		}
		
		string GetNAntOutput()
		{
			return "[nant] C:\\Projects\\dotnet\\Corsavy\\SharpDevelop\\src\\AddIns\\Misc\\Debugger\\Debugger.Core\\Debugger.Core.build clean\r\n" +
				"    Buildfile: file:///C:/Projects/dotnet/Corsavy/SharpDevelop/src/AddIns/Misc/Debugger/Debugger.Core/Debugger.Core.build\r\n" +
				"    Target(s) specified: clean \r\n" +
				"    \r\n" +
				"    \r\n" +
				"    clean:\r\n" +
				"    \r\n" +
				"    \r\n" +
				"    SetProperties:\r\n" +
				"    \r\n" +
				"       [delete] C:\\Projects\\dotnet\\Corsavy\\SharpDevelop\\src\\StandardAddIn.include(94,5):\r\n" +
				"       [delete] Cannot delete directory 'C:\\Projects\\dotnet\\Corsavy\\SharpDevelop\\AddIns\\AddIns\\Misc\\Debugger.AddIn'. The directory does not exist.\r\n" +
				"    \r\n" +
				"    BUILD SUCCEEDED - 1 non-fatal error(s), 0 warning(s)\r\n" +
				"    \r\n" +
				"    Total time: 0.3 seconds.";
		}
	}
}
