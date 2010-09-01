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

namespace RubyBinding.Tests.Gui
{
	[TestFixture]
	public class DebugRubyCommandTestFixture
	{
		MockDebugger debugger;
		RunDebugRubyCommand command;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			MockWorkbench workbench = MockWorkbench.CreateWorkbenchWithOneViewContent(@"C:\Projects\test.rb");

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
		public void ProcessInfoFileNameContainsPathToIronRubyConsole()
		{
			Assert.AreEqual(@"C:\IronRuby\ir.exe", debugger.ProcessStartInfo.FileName);
		}
		
		[Test]
		public void ProcessInfoArgsHasDebugArgument()
		{
			Assert.AreEqual("-D test.rb", debugger.ProcessStartInfo.Arguments);
		}
	}
}
