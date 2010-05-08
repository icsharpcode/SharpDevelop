// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using NUnit.Framework;

namespace ICSharpCode.CodeCoverage.Tests
{
	/// <summary>
	/// Tests the PartCoverRunner's command line argumentsests.
	/// </summary>
	[TestFixture]
	public class PartCoverCommandLineTests
	{
		[Test]
		public void PartCoverFileNameSpecified()
		{
			string partCoverFileName = @"C:\Program Files\PartCover\PartCover.exe";
			PartCoverRunner runner = new PartCoverRunner();
			runner.PartCoverFileName = partCoverFileName;
			
			Assert.AreEqual(partCoverFileName, runner.CommandLine);
		}
		
		[Test]
		public void ArgumentsStringIsEmptyWhenNothingSpecified()
		{
			PartCoverRunner runner = new PartCoverRunner();
			Assert.AreEqual(String.Empty, runner.GetArguments());
		}
		
		[Test]
		public void WorkingDirectoryNotSet()
		{
			PartCoverRunner runner = new PartCoverRunner();
			Assert.AreEqual(String.Empty, runner.WorkingDirectory);
		}
		
		[Test]
		public void WorkingDirectorySet()
		{
			PartCoverRunner runner = new PartCoverRunner();
			string folder = @"C:\Program Files\PartCover";
			runner.WorkingDirectory = folder;
			Assert.AreEqual(folder, runner.WorkingDirectory);
		}
		
		[Test]
		public void TargetFileNameSpecified()
		{
			string targetFileName = @"C:\Program Files\SharpDevelop\bin\Tools\NUnit-console.exe";
			string partCoverFileName = @"C:\Program Files\PartCover\PartCover.exe";
			PartCoverRunner runner = new PartCoverRunner();
			runner.PartCoverFileName = partCoverFileName;
			runner.Target = targetFileName;
			string expectedCommandLine = partCoverFileName + " --target \"" + targetFileName + "\"";
			
			Assert.AreEqual(expectedCommandLine, runner.CommandLine);
		}		
		
		[Test]
		public void TargetWorkingDirectorySpecified()
		{
			string targetWorkingDirectory = @"C:\Program Files\SharpDevelop\bin\Tools";
			PartCoverRunner runner = new PartCoverRunner();
			runner.TargetWorkingDirectory = targetWorkingDirectory;
			string expectedArgs = "--target-work-dir \"" + targetWorkingDirectory + "\"";
			
			Assert.AreEqual(expectedArgs, runner.GetArguments());
		}
		
		[Test]
		public void TargetArguments()
		{
			string targetArgs = @"C:\Project\Test\MyTests.dll";
			PartCoverRunner runner = new PartCoverRunner();
			runner.TargetWorkingDirectory = null;
			runner.TargetArguments = targetArgs;
			string expectedArgs = "--target-args \"" + targetArgs + "\"";
			
			Assert.AreEqual(expectedArgs, runner.GetArguments());
		}		

		/// <summary>
		/// In order for the target arguments to be successfully passed to
		/// PartCover we need to prefix any double quote with a backslash.
		/// </summary>
		[Test]
		public void TargetArgumentsIncludeDoubleQuotes()
		{
			string targetArgs = "\"C:\\Project\\My Tests\\MyTests.dll\" /output=\"C:\\Project\\My Tests\\Output.xml\"";
			PartCoverRunner runner = new PartCoverRunner();
			runner.TargetWorkingDirectory = null;
			runner.TargetArguments = targetArgs;
			string expectedArgs = "--target-args \"" + targetArgs.Replace("\"", "\\\"") + "\"";
			
			Assert.AreEqual(expectedArgs, runner.GetArguments());
		}		
		
		[Test]
		public void IncludeSpecified()
		{
			string include = @"[RootNamespace.MyTests]*";
			PartCoverRunner runner = new PartCoverRunner();
			runner.Include.Add(include);
			string expectedArgs = "--include " + include;
			
			Assert.AreEqual(expectedArgs, runner.GetArguments());
		}		

		[Test]
		public void TwoIncludeItemsSpecified()
		{
			string include1 = @"[RootNamespace.MyTests]*";
			string include2 = @"[System]*";
			PartCoverRunner runner = new PartCoverRunner();
			runner.Include.Add(include1);
			runner.Include.Add(include2);
			string expectedArgs = "--include " + include1 + " --include " + include2;
			
			Assert.AreEqual(expectedArgs, runner.GetArguments());
		}		
		
		[Test]
		public void ExcludeSpecified()
		{
			string exclude = @"[RootNamespace.MyTests]*";
			PartCoverRunner runner = new PartCoverRunner();
			runner.Exclude.Add(exclude);
			string expectedArgs = "--exclude " + exclude;
			
			Assert.AreEqual(expectedArgs, runner.GetArguments());
		}		

		[Test]
		public void TwoExcludeItemsSpecified()
		{
			string exclude1 = @"[RootNamespace.MyTests]*";
			string exclude2 = @"[System]*";
			PartCoverRunner runner = new PartCoverRunner();
			runner.Exclude.Add(exclude1);
			runner.Exclude.Add(exclude2);
			string expectedArgs = "--exclude " + exclude1 + " --exclude " + exclude2;
			
			Assert.AreEqual(expectedArgs, runner.GetArguments());
		}		

		[Test]
		public void OneIncludeAndExcludeItemSpecified()
		{
			string exclude = @"[RootNamespace.MyTests]*";
			string include = @"[System]*";
			PartCoverRunner runner = new PartCoverRunner();
			runner.Exclude.Add(exclude);
			runner.Include.Add(include);
			string expectedArgs = "--include " + include + " --exclude " + exclude;
			
			Assert.AreEqual(expectedArgs, runner.GetArguments());
		}		
		
		[Test]
		public void OutputSpecified()
		{
			string output = @"C:\Projects\MyTests\CodeCoverage.xml";
			PartCoverRunner runner = new PartCoverRunner();
			runner.Output = output;
			string expectedArgs = "--output \"" + output + "\"";
			
			Assert.AreEqual(expectedArgs, runner.GetArguments());
		}		
	}
}
