// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.RubyBinding;
using RubyBinding.Tests.Utils;
using NUnit.Framework;

namespace RubyBinding.Tests
{
	[TestFixture]
	public class DebugRubyCommandTestFixture
	{
		MockDebugger debugger;
		RunDebugRubyCommand command;
		
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
			command = new RunDebugRubyCommand(workbench, options, debugger);
			command.Run();
		}
				
		[Test]
		public void DebuggerStartMethodCalled()
		{
			Assert.IsTrue(debugger.StartMethodCalled);
		}
		
		[Test]
		public void ProcessInfoFileName()
		{
			Assert.AreEqual(@"C:\IronRuby\ir.exe", debugger.ProcessStartInfo.FileName);
		}
		
		[Test]
		public void ProcessInfoArgs()
		{
			Assert.AreEqual("-D --disable-gems test.rb", debugger.ProcessStartInfo.Arguments);
		}
	}
}
