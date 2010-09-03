// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.IO;
using System.Text;

using ICSharpCode.SharpDevelop.Util;
using NUnit.Framework;

namespace ICSharpCode.SharpDevelop.Tests
{
	/// <summary>
	/// Runs a process that returns some text on the standard output.
	/// </summary>
	[TestFixture]
	//[Ignore("Ignoring since need to run ConsoleApp.exe")]
	public class StandardOutputFromProcessTestFixture : ConsoleAppTestFixtureBase
	{
		int preTestThreadCount;

		/// <summary>
		/// Initialises each test.
		/// </summary>
		[SetUp]
		public void Init()
		{
			GetPreTestThreadCount();
		}
		
		[TearDown]
		public void TearDown()
		{
			//If we use Output.ReadLine then ThreadPoolCompletion Port threads are
			//used rendering a threadcount test useless.
			//CheckPostTestThreadCount();
		}
		
		/// <summary>
		/// Runs a process expecting a single line of output.
		/// </summary>
		[Test]
		public void SingleLineOfOutput()
		{			
			ProcessRunner runner = new ProcessRunner();
			runner.WorkingDirectory = Path.GetDirectoryName(GetConsoleAppFileName());
			
			string echoText = "Test";
			string expectedOutput = String.Concat(echoText, "\r\n");
			runner.Start(GetConsoleAppFileName(), String.Concat("-echo:", echoText));
			runner.WaitForExit();
			
			Assert.IsFalse(runner.IsRunning, "IsRunning should be false.");
			Assert.AreEqual(0, runner.ExitCode, "Exit code is incorrect.");
			Assert.AreEqual(expectedOutput, runner.StandardOutput, "Should have some output.");
			Assert.AreEqual(String.Empty, runner.StandardError, "Should not be any error output.");
		}
		
		/// <summary>
		/// Runs a process and makes sure it does not return any output.
		/// </summary>
		[Test]
		public void NoOutput()
		{
			ProcessRunner runner = new ProcessRunner();
			runner.WorkingDirectory = Path.GetDirectoryName(GetConsoleAppFileName());
			
			runner.Start(GetConsoleAppFileName());
			runner.WaitForExit();
			
			Assert.IsFalse(runner.IsRunning, "IsRunning should be false.");
			Assert.AreEqual(0, runner.ExitCode, "Exit code is incorrect.");
			Assert.AreEqual(String.Empty, runner.StandardOutput, "Should not be any output.");
			Assert.AreEqual(String.Empty, runner.StandardError, "Should not be any error output.");			
		}
		
		/// <summary>
		/// The process that is run tries to send such a large amount of 
		/// data that it fills the standard output's buffer.
		/// </summary>
		[Test]
		public void LargeAmountOfOutput()
		{
			ProcessRunner runner = new ProcessRunner();
			runner.WorkingDirectory = Path.GetDirectoryName(GetConsoleAppFileName());
			
			string filename = "test.txt";
			string fullFilename = Path.Combine(runner.WorkingDirectory, filename);
		
			try
			{
				string outputText = GetOutputText();
				CreateTextFile(fullFilename, outputText);
				runner.Start(GetConsoleAppFileName(), String.Concat("-file:", filename));
				bool exited = runner.WaitForExit(5000);
				
				Assert.IsTrue(exited, "App did not exit.");
				Assert.IsFalse(runner.IsRunning, "IsRunning should be false.");
				Assert.AreEqual(0, runner.ExitCode, "Exit code is incorrect.");
				Assert.AreEqual(outputText, runner.StandardOutput, "Should have some output.");
				Assert.AreEqual(String.Empty, runner.StandardError, "Should not be any error output.");						
			}
			finally
			{
				if (File.Exists(fullFilename)) {
					File.Delete(fullFilename);
				}
			}
		}

		/// <summary>
		/// The process that is run tries to send such a large amount of 
		/// data that it fills the standard output's buffer.
		/// </summary>
		[Test]
		public void LargeAmountOfErrorOutput()
		{
			ProcessRunner runner = new ProcessRunner();
			runner.WorkingDirectory = Path.GetDirectoryName(GetConsoleAppFileName());
			
			string filename = "test.txt";
			string fullFilename = Path.Combine(runner.WorkingDirectory, filename);
		
			try
			{
				string outputText = GetOutputText();
				CreateTextFile(fullFilename, outputText);
				runner.Start(GetConsoleAppFileName(), String.Concat("-error.file:", filename));
				bool exited = runner.WaitForExit(5000);
				
				Assert.IsTrue(exited, "App did not exit.");
				Assert.IsFalse(runner.IsRunning, "IsRunning should be false.");
				Assert.AreEqual(0, runner.ExitCode, "Exit code is incorrect.");
				Assert.AreEqual(String.Empty, runner.StandardOutput, "Should not be any output.");
				Assert.AreEqual(outputText, runner.StandardError, "Should have some error output.");						
			}
			finally
			{
				if (File.Exists(fullFilename)) {
					File.Delete(fullFilename);
				}
			}
		}
		
		/// <summary>
		/// Runs a process expecting a single line of output written
		/// to standard error.
		/// </summary>
		[Test]
		public void SingleLineOfErrorOutput()
		{
			ProcessRunner runner = new ProcessRunner();
			runner.WorkingDirectory = Path.GetDirectoryName(GetConsoleAppFileName());
			
			string echoText = "Test";
			string expectedOutput = String.Concat(echoText, "\r\n");
			runner.Start(GetConsoleAppFileName(), String.Concat("-error.echo:", echoText));
			runner.WaitForExit();
			
			Assert.AreEqual(0, runner.ExitCode, "Exit code is incorrect.");
			Assert.IsFalse(runner.IsRunning, "IsRunning should be false.");
			Assert.AreEqual(String.Empty, runner.StandardOutput, "Should not be any output.");
			Assert.AreEqual(expectedOutput, runner.StandardError, "Should have some error output.");
		}		
		
		/// <summary>
		/// Creates a UTF8 text file.
		/// </summary>
		/// <param name="filename">The filename of the text file.</param>
		/// <param name="outputText">The text to store in the file.</param>
		void CreateTextFile(string filename, string outputText)
		{
			FileStream stream = File.Create(filename);
			
			byte[] bytes = UnicodeEncoding.UTF8.GetBytes(outputText);
			stream.Write(bytes, 0, bytes.Length);
			stream.Close();
		}
		
		/// <summary>
		/// Gets the output text that the console app will
		/// attempt to send us back.
		/// </summary>
		/// <returns></returns>
		string GetOutputText()
		{
			StringBuilder outputText = new StringBuilder();
			
			for (int i = 0; i < 300; ++i) {
				outputText.Append("i=");
				outputText.Append(i.ToString());
				outputText.Append(" A line of text.\r\n");
			}
			
			return outputText.ToString();
		}
		
		/// <summary>
		/// Reads the number of threads before the test is run.
		/// </summary>
		void GetPreTestThreadCount()
		{
			Process process = Process.GetCurrentProcess();
			preTestThreadCount = process.Threads.Count;
		}
		
		/// <summary>
		/// Reads the number of threads after the test is run.
		/// </summary>
		void CheckPostTestThreadCount()
		{
			GC.Collect();
			int postThreadCount = Process.GetCurrentProcess().Threads.Count;
			Assert.AreEqual(preTestThreadCount, postThreadCount, "Thread count mismatch.");
		}
	}
}
