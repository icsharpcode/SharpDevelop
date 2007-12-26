using System;
using ICSharpCode.SharpDevelop;
using NUnit.Framework;

namespace ICSharpCode.NAnt.Tests
{
	/// <summary>
	/// Tests that the "Target 'foo' does not exist" error is handled.
	/// </summary>
	[TestFixture]
	public class TargetDoesNotExistNAntOutputTestFixture
	{
		[Test]
		public void Parse085()
		{
			TaskCollection tasks = NAntOutputParser.Parse(GetNAntOutput());
			
			Assert.AreEqual(1, tasks.Count, "Should be one task.");
		
			Task task = tasks[0];
			Assert.AreEqual(String.Empty, task.FileName, "Should not have any filename information.");
			Assert.AreEqual(TaskType.Error, task.TaskType, "Should be an error task.");
			Assert.AreEqual(0, task.Line, "Should be line number 0");
			Assert.AreEqual(0, task.Column, "Should be col number 0");
			Assert.AreEqual("Target 'abuild' does not exist in this project.",
			                task.Description,
			                "Task description is wrong.");
		}
		
		string GetNAntOutput()
		{
			return "Buildfile: file:///C:/Projects/dotnet/Test/corsavytest/corsavytest.build\r\n" +
				"Target(s) specified: abuild \r\n" +
				"\r\n" +
				"\r\n" +
				"BUILD FAILED\r\n" +
				"\r\n" +
				"Target 'abuild' does not exist in this project.\r\n" +
				"\r\n" +
				"Total time: 0.1 seconds.\r\n" +
				"";
		}
	}
}
