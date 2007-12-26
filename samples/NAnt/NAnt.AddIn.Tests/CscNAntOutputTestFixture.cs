using System;
using ICSharpCode.SharpDevelop;
using NUnit.Framework;

namespace ICSharpCode.NAnt.Tests
{
	/// <summary>
	/// Tests that C# compiler errors/warnings are located in the NAnt console
	/// output.
	/// </summary>
	[TestFixture]
	public class CscNAntOutputTestFixture
	{
		[Test]
		public void ParseError085()
		{
			TaskCollection tasks = NAntOutputParser.Parse(GetNAntCscErrorOutput());
			
			Assert.AreEqual(3, tasks.Count, "Should be three tasks.");

			// First task.
			Task task = tasks[0];
			Assert.AreEqual("c:\\Projects\\dotnet\\Test\\corsavytest\\Foo.cs", task.FileName, "Task filename is incorrect.");
			Assert.AreEqual(33, task.Line, "Task line is incorrect.");
			Assert.AreEqual(3, task.Column, "Task column is incorrect.");
			Assert.AreEqual(TaskType.Error, task.TaskType, "Should be error task.");
			Assert.AreEqual(@"Invalid expression term '/' (CS1525)",  
			                task.Description,
			                "Task description is wrong.");
			
			// Second task.
			task = tasks[1];
			Assert.AreEqual("c:\\Projects\\dotnet\\Test\\corsavytest\\Foo.cs", task.FileName, "Task filename is incorrect.");
			Assert.AreEqual(33, task.Line, "Task line is incorrect.");
			Assert.AreEqual(4, task.Column, "Task column is incorrect.");
			Assert.AreEqual(TaskType.Error, task.TaskType, "Should be error task.");
			Assert.AreEqual(@"; expected (CS1002)",  
			                task.Description,
			                "Task description is wrong.");		
			
			// Last task task.
			task = tasks[2];
			Assert.AreEqual(@"C:\Projects\dotnet\Test\corsavytest\corsavytest.build", task.FileName, "Task filename is incorrect.");
			Assert.AreEqual(47, task.Line, "Task line is incorrect.");
			Assert.AreEqual(5, task.Column, "Task column is incorrect.");
			Assert.AreEqual(TaskType.Error, task.TaskType, "Should be error task.");
			Assert.AreEqual(@"External Program Failed: C:\WINDOWS\Microsoft.NET\Framework\v1.1.4322\csc.exe (return code was 1)",  
			                task.Description,
			                "Task description is wrong.");
		}
		
		[Test]
		public void ParseWarning085()
		{
			TaskCollection tasks = NAntOutputParser.Parse(GetNAntCscWarningOutput());
			
			Assert.AreEqual(1, tasks.Count, "Should be three tasks.");

			// First task.
			Task task = tasks[0];
			Assert.AreEqual("c:\\Projects\\dotnet\\Test\\corsavytest\\Foo.cs", task.FileName, "Task filename is incorrect.");
			Assert.AreEqual(38, task.Line, "Task line is incorrect.");
			Assert.AreEqual(11, task.Column, "Task column is incorrect.");
			Assert.AreEqual(TaskType.Warning, task.TaskType, "Should be error task.");
			Assert.AreEqual(@"The variable 'Test' is assigned but its value is never used (CS0219)",  
			                task.Description,
			                "Task description is wrong.");
		}
			
		
		string GetNAntCscErrorOutput()
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
				"      [csc] c:\\Projects\\dotnet\\Test\\corsavytest\\Foo.cs(34,4): error CS1525: Invalid expression term '/'\r\n" +
				"      [csc] c:\\Projects\\dotnet\\Test\\corsavytest\\Foo.cs(34,5): error CS1002: ; expected\r\n" +
				"\r\n" +
				"BUILD FAILED\r\n" +
				"\r\n" +
				"C:\\Projects\\dotnet\\Test\\corsavytest\\corsavytest.build(48,6):\r\n" +
				"External Program Failed: C:\\WINDOWS\\Microsoft.NET\\Framework\\v1.1.4322\\csc.exe (return code was 1)\r\n" +
				"\r\n" +
				"Total time: 0.5 seconds.";
		}
		
		string GetNAntCscWarningOutput()
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
				"      [csc] c:\\Projects\\dotnet\\Test\\corsavytest\\Foo.cs(39,12): warning CS0219: The variable 'Test' is assigned but its value is never used\r\n" +
				"\r\n" +
				"build:\r\n" +
				"\r\n" +
				"\r\n" +
				"BUILD SUCCEEDED\r\n" +
				"\r\n" +
				"Total time: 0.4 seconds.";
		}
	}
}
