using System;
using ICSharpCode.SharpDevelop;
using NUnit.Framework;

namespace ICSharpCode.NAnt.Tests
{
	/// <summary>
	/// Tests that the NAnt errors and warnings are parsed correctly.
	/// </summary>
	[TestFixture]
	public class NAntErrorAndWarningOutputTestFixture
	{
		[Test]
		public void Parse085()
		{
			TaskCollection tasks = NAntOutputParser.Parse(GetNAntOutput());
			
			Assert.AreEqual(2, tasks.Count, "Should be two tasks.");
		
			Task task = tasks[0];
			Assert.AreEqual("C:\\Projects\\dotnet\\Test\\corsavytest\\corsavytest.build", task.FileName, "Task filename is incorrect.");
			Assert.AreEqual(TaskType.Warning, task.TaskType, "Should be a warning task.");
			Assert.AreEqual(4, task.Line, "Incorrect line number.");
			Assert.AreEqual(3, task.Column, "Incorrect col number.");
			Assert.AreEqual("Attribute 'propertyexists' for <ifnot ... /> is deprecated.  Use <if test=\"${property::exists('propertyname')}\"> instead.",
			                task.Description,
			                "Task description is wrong.");
			
			task = tasks[1];
			Assert.AreEqual("C:\\Projects\\dotnet\\Test\\corsavytest\\corsavytest.build", task.FileName, "Task filename is incorrect.");
			Assert.AreEqual(TaskType.Error, task.TaskType, "Should be an error task.");
			Assert.AreEqual(47, task.Line, "Incorrect line number.");
			Assert.AreEqual(5, task.Column, "Incorrect col number.");
			Assert.AreEqual("An empty string is not a valid value for attribute 'win32icon' of <csc ... />.",
			                task.Description,
			                "Task description is wrong.");
		
		}
		
		string GetNAntOutput()
		{
			return "Buildfile: file:///C:/Projects/dotnet/Test/corsavytest/corsavytest.build\r\n" +
				"Target(s) specified: build \r\n" +
				"\r\n" +
				"    [ifnot] C:\\Projects\\dotnet\\Test\\corsavytest\\corsavytest.build(5,4): Attribute 'propertyexists' for <ifnot ... /> is deprecated.  Use <if test=\"${property::exists('propertyname')}\"> instead.\r\n" +
				"\r\n" +
				"init.debug:\r\n" +
				"\r\n" +
				"\r\n" +
				"gacreferences.debug:\r\n" +
				"\r\n" +
				"\r\n" +
				"build.debug:\r\n" +
				"\r\n" +
				"\r\n" +
				"BUILD FAILED - 0 non-fatal error(s), 1 warning(s)\r\n" +
				"\r\n" +
				"C:\\Projects\\dotnet\\Test\\corsavytest\\corsavytest.build(48,6):\r\n" +
				"An empty string is not a valid value for attribute 'win32icon' of <csc ... />.\r\n" +
				"\r\n" +
				"Total time: 0.1 seconds.";
		}
	}
}
