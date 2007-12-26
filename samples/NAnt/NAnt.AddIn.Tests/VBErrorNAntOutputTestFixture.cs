using NUnit.Framework;
using ICSharpCode.NAnt;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using System;

namespace ICSharpCode.NAnt.Tests
{
	/// <summary>
	/// Tests that VB errors are parsed correctly.
	/// </summary>
	[TestFixture]
	public class VBErrorNAntOutputTestFixture
	{
		[Test]
		public void Parse085()
		{
			TaskCollection tasks = NAntOutputParser.Parse(GetNAntOutput());
			
			Assert.AreEqual(3, tasks.Count, "Should be three tasks.");
		
			Task task = tasks[0];
			
			Assert.AreEqual("C:\\Projects\\dotnet\\test\\corsavyvbtest\\corsavyvbtest.build", task.FileName, "Task filename is incorrect.");
			Assert.AreEqual(TaskType.Warning, task.TaskType, "Should be a warning task.");
			Assert.AreEqual(47, task.Line, "Incorrect line number.");
			Assert.AreEqual(5, task.Column, "Incorrect col number.");
			Assert.AreEqual("Attribute 'imports' for <vbc ... /> is deprecated.  Use the <imports> element instead.",
			                task.Description,
			                "Task description is wrong.");
			
			task = tasks[1];
			Assert.AreEqual("C:\\Projects\\dotnet\\test\\corsavyvbtest\\MainForm.vb", task.FileName, "Task filename is incorrect.");
			Assert.AreEqual(TaskType.Error, task.TaskType, "Should be an error task.");
			Assert.AreEqual(13, task.Line, "Incorrect line number.");
			Assert.AreEqual(0, task.Column, "Should be col number 0");
			Assert.AreEqual("Syntax error. (BC30035)",
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
		
		string GetNAntOutput()
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
				"      [vbc] C:\\Projects\\dotnet\\test\\corsavyvbtest\\corsavyvbtest.build(48,6): Attribute 'imports' for <vbc ... /> is deprecated.  Use the <imports> element instead.\r\n" +
				"      [vbc] Compiling 2 files to 'C:\\Projects\\dotnet\\test\\corsavyvbtest\\bin\\Debug\\corsavyvbtest.exe'.\r\n" +
				"      [vbc] C:\\Projects\\dotnet\\test\\corsavyvbtest\\MainForm.vb(14) : error BC30035: Syntax error.\r\n" +
				"      [vbc] \r\n" +
				"      [vbc]     /\r\n" +
				"      [vbc]     ~\r\n" +
				"\r\n" +
				"BUILD FAILED - 0 non-fatal error(s), 1 warning(s)\r\n" +
				"\r\n" +
				"C:\\Projects\\dotnet\\test\\corsavyvbtest\\corsavyvbtest.build(48,6):\r\n" +
				"External Program Failed: C:\\WINDOWS\\Microsoft.NET\\Framework\\v1.1.4322\\vbc.exe (return code was 1)\r\n" +
				"\r\n" +
				"Total time: 0.3 seconds.\r\n" +
				"\r\n";
		}
	}
}
