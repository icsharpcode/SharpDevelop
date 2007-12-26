using System;
using ICSharpCode.SharpDevelop;
using NUnit.Framework;

namespace ICSharpCode.NAnt.Tests
{
	[TestFixture]
	public class ReadOnlyPropertyNAntOutputTestFixture
	{
		[Test]
		public void Parse085()
		{
			TaskCollection tasks = NAntOutputParser.Parse(GetNAntOutput());
			
			Assert.AreEqual(1, tasks.Count, "Should be one task.");
		
			Task task = tasks[0];
			
			Assert.AreEqual(String.Empty, task.FileName, "Task filename is incorrect.");
			Assert.AreEqual(TaskType.Warning, task.TaskType, "Should be a warning task.");
			Assert.AreEqual(0, task.Line, "Incorrect line number.");
			Assert.AreEqual(0, task.Column, "Incorrect col number.");
			Assert.AreEqual("Read-only property \"debug\" cannot be overwritten.",
			                task.Description,
			                "Task description is wrong.");
		}
		
		string GetNAntOutput()
		{
			return "Buildfile: file:///C:/Projects/dotnet/Corsavy/SharpDevelop/src/SharpDevelop.build\r\n" +
				"Target(s) specified: clean \r\n" +
				"\r\n" +
				" [property] Read-only property \"debug\" cannot be overwritten.\r\n" +
				"\r\n" +
				"clean:\r\n" +
				"\r\n" +
				"\r\n" +
				"CallBuildfiles";
		}
	}
}
