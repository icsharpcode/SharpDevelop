// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using ICSharpCode.Core;
using ICSharpCode.RubyBinding;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Testing
{
	[TestFixture]
	public class RubyConsoleApplicationTestFixture
	{
		RubyConsoleApplication app;
		RubyAddInOptions options;
		
		[SetUp]
		public void Init()
		{
			options = new RubyAddInOptions(new Properties());
			options.RubyFileName = @"C:\IronRuby\ir.exe";
			app = new RubyConsoleApplication(options);
		}
		
		[Test]
		public void FileNameIsRubyFileNameFromAddInOptions()
		{
			string expectedFileName = @"C:\IronRuby\ir.exe";
			Assert.AreEqual(expectedFileName, app.FileName);
		}
		
		[Test]
		public void GetArgumentsReturnsDebugOptionWhenDebugIsTrue()
		{
			app.Debug = true;
			string expectedCommandLine = "-D";
			
			Assert.AreEqual(expectedCommandLine, app.GetArguments());
		}
		
		[Test]
		public void GetArgumentsReturnsQuotedRubyScriptFileName()
		{
			app.RubyScriptFileName = @"d:\projects\my ruby\test.rb";
			string expectedCommandLine = "\"d:\\projects\\my ruby\\test.rb\"";
			
			Assert.AreEqual(expectedCommandLine, app.GetArguments());
		}
		
		[Test]
		public void GetArgumentsReturnsQuotedRubyScriptFileNameAndItsCommandLineArguments()
		{
			app.Debug = true;
			app.RubyScriptFileName = @"d:\projects\my ruby\test.rb";
			app.RubyScriptCommandLineArguments = "-- responseFile.txt";
			string expectedCommandLine =
				"-D \"d:\\projects\\my ruby\\test.rb\" -- responseFile.txt";
			
			Assert.AreEqual(expectedCommandLine, app.GetArguments());
		}
		
		[Test]
		public void GetProcessStartInfoHasFileNameThatEqualsIronRubyConsoleApplicationExeFileName()
		{
			ProcessStartInfo startInfo = app.GetProcessStartInfo();
			string expectedFileName = @"C:\IronRuby\ir.exe";
			
			Assert.AreEqual(expectedFileName, startInfo.FileName);
		}
		
		[Test]
		public void GetProcessStartInfoHasDebugFlagSetInArguments()
		{
			app.Debug = true;
			ProcessStartInfo startInfo = app.GetProcessStartInfo();
			string expectedCommandLine = "-D";
			
			Assert.AreEqual(expectedCommandLine, startInfo.Arguments);
		}
		
		[Test]
		public void GetProcessStartInfoHasWorkingDirectoryIfSet()
		{
			app.WorkingDirectory = @"d:\temp";
			ProcessStartInfo startInfo = app.GetProcessStartInfo();
			Assert.AreEqual(@"d:\temp", startInfo.WorkingDirectory);
		}
	}
}
