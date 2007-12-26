using NUnit.Framework;
using ICSharpCode.NAnt;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using System;

namespace ICSharpCode.NAnt.Tests
{
	/// <summary>
	/// Tests that fatal errors are parsed correctly.
	/// </summary>
	[TestFixture]
	public class FatalErrorNAntOutputTestFixture
	{
		[Test]
		public void ParseCscError085()
		{
			TaskCollection tasks = NAntOutputParser.Parse(GetNAntCscOutput());
			
			Assert.AreEqual(2, tasks.Count, "Should be two tasks.");
		
			Task task = tasks[0];
		
			Assert.AreEqual(String.Empty, task.FileName, "Task filename should be blank.");
			Assert.AreEqual(TaskType.Error, task.TaskType, "Should be an error task.");
			Assert.AreEqual(0, task.Line, "Should be line number 0.");
			Assert.AreEqual(0, task.Column, "Should be col number 0");
			Assert.AreEqual("fatal error CS0042: Unexpected error creating debug information file 'c:\\Projects\\dotnet\\Test\\corsavytest\\bin\\Debug\\corsavytest.PDB' -- 'c:\\Projects\\dotnet\\Test\\corsavytest\\bin\\Debug\\corsavytest.pdb: The process cannot access the file because it is being used by another process.",
			                task.Description,
			                "Task description is wrong.");

			task = tasks[1];
			Assert.AreEqual("C:\\Projects\\dotnet\\Test\\corsavytest\\corsavytest.build", task.FileName, "Task filename is incorrect.");
			Assert.AreEqual(TaskType.Error, task.TaskType, "Should be an error task.");
			Assert.AreEqual(47, task.Line, "Incorrect line number.");
			Assert.AreEqual(5, task.Column, "Incorrect col number.");
			Assert.AreEqual("External Program Failed: C:\\WINDOWS\\Microsoft.NET\\Framework\\v1.1.4322\\csc.exe (return code was 1)",
			                task.Description,
			                "Task description is wrong.");
		}
		
		[Test]
		public void ParseVBError085()
		{
			TaskCollection tasks = NAntOutputParser.Parse(GetNAntVBOutput());
			
			Assert.AreEqual(3, tasks.Count, "Should be three tasks.");
		
			Task task = tasks[0];
		
			Assert.AreEqual(String.Empty, task.FileName, "Task filename should be blank.");
			Assert.AreEqual(TaskType.Error, task.TaskType, "Should be an error task.");
			Assert.AreEqual(0, task.Line, "Should be line number 0.");
			Assert.AreEqual(0, task.Column, "Should be col number 0");
			Assert.AreEqual("Unable to write to output file 'C:\\Projects\\dotnet\\test\\corsavyvbtest\\bin\\Debug\\corsavyvbtest.exe': The process cannot access the file because it is being used by another process.  (BC31019)",
			                task.Description,
			                "Task description is wrong.");

			task = tasks[1];
			Assert.AreEqual(String.Empty, task.FileName, "Task filename should be blank.");
			Assert.AreEqual(TaskType.Error, task.TaskType, "Should be an error task.");
			Assert.AreEqual(0, task.Line, "Should be line number 0.");
			Assert.AreEqual(0, task.Column, "Should be col number 0");
			Assert.AreEqual("Unable to write to output file 'C:\\Projects\\dotnet\\test\\corsavyvbtest\\bin\\Debug\\corsavyvbtest.pdb': Access is denied.  (BC31019)",
			                task.Description,
			                "Task description is wrong.");

			task = tasks[2];
			Assert.AreEqual("C:\\Projects\\dotnet\\test\\corsavyvbtest\\corsavyvbtest.build", task.FileName, "Task filename is incorrect.");
			Assert.AreEqual(TaskType.Error, task.TaskType, "Should be an error task.");
			Assert.AreEqual(47, task.Line, "Incorrect line number.");
			Assert.AreEqual(5, task.Column, "Incorrect col number.");
			Assert.AreEqual("External Program Failed: C:\\WINDOWS\\Microsoft.NET\\Framework\\v1.1.4322\\vbc.exe (return code was 1)",
			                task.Description,
			                "Task description is wrong.");
		}
				
		string GetNAntCscOutput()
		{
			return "Buildfile: file:///C:/Projects/dotnet/Test/corsavytest/corsavytest.build\r\n" +
				"Target(s) specified: build \r\n" +
				"\r\n" +
				"\r\n" +
				"init.debug:\r\n" +
				"\r\n" +
				"\r\n" +
				"gacreferences.debug:\r\n" +
				"\r\n" +
				"\r\n" +
				"build.debug:\r\n" +
				"\r\n" +
				"      [csc] Compiling 3 files to 'C:\\Projects\\dotnet\\Test\\corsavytest\\bin\\Debug\\corsavytest.exe'.\r\n" +
				"      [csc] fatal error CS0042: Unexpected error creating debug information file 'c:\\Projects\\dotnet\\Test\\corsavytest\\bin\\Debug\\corsavytest.PDB' -- 'c:\\Projects\\dotnet\\Test\\corsavytest\\bin\\Debug\\corsavytest.pdb: The process cannot access the file because it is being used by another process.\r\n" +
				"      [csc] \r\n" +
				"      [csc]         \r\n" +
				"\r\n" +
				"BUILD FAILED\r\n" +
				"\r\n" +
				"C:\\Projects\\dotnet\\Test\\corsavytest\\corsavytest.build(48,6):\r\n" +
				"External Program Failed: C:\\WINDOWS\\Microsoft.NET\\Framework\\v1.1.4322\\csc.exe (return code was 1)\r\n" +
				"\r\n" +
				"Total time: 0.3 seconds.\r\n" +
				"";
		}
		
		string GetNAntVBOutput()
		{
			return "Buildfile: file:///C:/Projects/dotnet/test/corsavyvbtest/corsavyvbtest.build\r\n" +
				"Target(s) specified: build \r\n" +
				"\r\n" +
				"\r\n" +
				"init.debug:\r\n" +
				"\r\n" +
				"\r\n" +
				"gacreferences.debug:\r\n" +
				"\r\n" +
				"\r\n" +
				"build.debug:\r\n" +
				"\r\n" +
				"      [vbc] Compiling 2 files to 'C:\\Projects\\dotnet\\test\\corsavyvbtest\\bin\\Debug\\corsavyvbtest.exe'.\r\n" +
				"      [vbc] vbc : error BC31019: Unable to write to output file 'C:\\Projects\\dotnet\\test\\corsavyvbtest\\bin\\Debug\\corsavyvbtest.exe': The process cannot access the file because it is being used by another process. \r\n" +
				"      [vbc] vbc : error BC31019: Unable to write to output file 'C:\\Projects\\dotnet\\test\\corsavyvbtest\\bin\\Debug\\corsavyvbtest.pdb': Access is denied. \r\n" +
				"\r\n" +
				"BUILD FAILED - 0 non-fatal error(s), 1 warning(s)\r\n" +
				"\r\n" +
				"C:\\Projects\\dotnet\\test\\corsavyvbtest\\corsavyvbtest.build(48,6):\r\n" +
				"External Program Failed: C:\\WINDOWS\\Microsoft.NET\\Framework\\v1.1.4322\\vbc.exe (return code was 1)\r\n" +
				"\r\n" +
				"Total time: 0.4 seconds.\r\n" +
				"";
		}
	}
}
