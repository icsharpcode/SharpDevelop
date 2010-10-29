// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.RubyBinding;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests
{
	/// <summary>
	/// Tests that the RunRubyCommand class runs the Ruby console
	/// passing the filename of the Ruby script active in SharpDevelop.
	/// </summary>
	[TestFixture]
	public class RunRubyCommandTestFixture
	{
		MockDebugger debugger;
		RunRubyCommand command;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			// Create dummy view content with the Ruby script.
			MockViewContent viewContent = new MockViewContent();
			viewContent.PrimaryFileName = @"C:\Projects\test.rb";
			MockWorkbenchWindow workbenchWindow = new MockWorkbenchWindow();
			workbenchWindow.ActiveViewContent = viewContent;
			MockWorkbench workbench = new MockWorkbench();
			workbench.ActiveWorkbenchWindow = workbenchWindow;

			// Create the Ruby binding addin options.
			Properties p = new Properties();
			RubyAddInOptions options = new RubyAddInOptions(p);
			options.RubyFileName = @"C:\IronRuby\ir.exe";
		
			debugger = new MockDebugger();
			command = new RunRubyCommand(workbench, options, debugger);
			command.Run();
		}
		
		[Test]
		public void RunRubyCommandIsAbstractCommand()
		{
			Assert.IsNotNull(command as AbstractCommand);
		}
		
		[Test]
		public void DebuggerStartWithoutDebuggingMethodCalled()
		{
			Assert.IsTrue(debugger.StartWithoutDebuggingMethodCalled);
		}
		
		[Test]
		public void ProcessInfoFileName()
		{
			Assert.AreEqual(@"C:\IronRuby\ir.exe", debugger.ProcessStartInfo.FileName);
		}
		
		/// <summary>
		/// The -1.9 parameter is used to enable Ruby 1.9 mode otherwise UTF8 files cannot be processed.
		/// </summary>
		[Test]
		public void ProcessInfoArgs()
		{
			Assert.AreEqual("--disable-gems test.rb", debugger.ProcessStartInfo.Arguments);
		}
		
		[Test]
		public void WorkingDirectory()
		{
			Assert.AreEqual(@"C:\Projects", debugger.ProcessStartInfo.WorkingDirectory);
		}
	}
}
