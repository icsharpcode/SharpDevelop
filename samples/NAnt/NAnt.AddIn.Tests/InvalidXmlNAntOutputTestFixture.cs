using System;
using ICSharpCode.SharpDevelop;
using NUnit.Framework;

namespace ICSharpCode.NAnt.Tests
{
	/// <summary>
	/// Tests that the NAnt text output indicating the build file xml is 
	/// invalid is parsed correctly.</summary>
	[TestFixture]
	public class InvalidXmlNAntOutputTestFixture
	{
		[Test]
		public void Parse085()
		{
			TaskCollection tasks = NAntOutputParser.Parse(GetNAntOutput());
			
			Assert.AreEqual(1, tasks.Count, "Should be one task.");
			
			Task task = tasks[0];
			Assert.AreEqual(@"C:\Projects\foo\foo.build", task.FileName, "Task filename is incorrect.");
			Assert.AreEqual(6, task.Line, "Task line is incorrect.");
			Assert.AreEqual(4, task.Column, "Task column is incorrect.");
			Assert.AreEqual(TaskType.Error, task.TaskType, "Should be error task.");
			string description = "Error loading buildfile.\r\n    The 'ifnot1' start tag on line '5' doesn't match the end tag of 'ifnot' in file 'file:///C:/Projects/foo/foo.build'. Line 7, position 5.";
			
			Assert.AreEqual(description,  
			                task.Description,
			                "Task description is wrong.");
		}
		
		/// <summary>
		/// Gets NAnt output for an invalid xml in a 0.85 build file.</summary>
		string GetNAntOutput()
		{
			return "\r\n" +
				"BUILD FAILED\r\n" +
				"\r\n" +
				"C:\\Projects\\foo\\foo.build(7,5):\r\n" +
				"Error loading buildfile.\r\n" +
				"    The 'ifnot1' start tag on line '5' doesn't match the end tag of 'ifnot' in file 'file:///C:/Projects/foo/foo.build'. Line 7, position 5.\r\n" +
				"\r\n" +
				"\r\n" +
				"For more information regarding the cause of the build failure, run the build again in debug mode.\r\n" +
				"\r\n" +
				"Try 'nant -help' for more information";
		}
	}
}
