// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using Debugger;
using Microsoft.CSharp;
using NUnit.Framework;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Threading;

namespace Debugger.Tests
{
	/// <summary>
	/// This class contains methods that test the debugger
	/// </summary>
	[TestFixture]
	public class DebuggerTests
	{
		NDebugger debugger;
		string log;
		string lastLogMessage;
		string assemblyFilename;
		string assemblyDir;
		
		public DebuggerTests()
		{
			assemblyFilename = Assembly.GetExecutingAssembly().Location;
			assemblyDir = Path.GetDirectoryName(assemblyFilename);
			
			debugger = new NDebugger();
			debugger.MTA2STA.CallMethod = CallMethod.Manual;
			debugger.LogMessage += delegate(object sender, MessageEventArgs e) {
				log += e.Message;
				lastLogMessage = e.Message;
			};
		}
		
		void StartProgram(string name)
		{
			log = "";
			lastLogMessage = null;
			debugger.Start(assemblyFilename, assemblyDir, name);
		}
		
		
		[Test]
		public void SimpleProgram()
		{
			StartProgram("SimpleProgram");
			
			debugger.WaitForPrecessExit();
		}
		
		[Test]
		public void HelloWorld()
		{
			StartProgram("HelloWorld");
			
			debugger.WaitForPrecessExit();
			
			Assert.AreEqual("Hello world!\r\n", log);
		}
		
		[Test]
		public void Break()
		{
			StartProgram("Break");
			
			debugger.WaitForPause();
			Assert.AreEqual(PausedReason.Break, debugger.PausedReason);
			debugger.Continue();
			
			debugger.WaitForPrecessExit();
		}
		
		[Test]
		public void Symbols()
		{
			StartProgram("Symbols");
			
			debugger.WaitForPause();
			Assert.AreEqual(PausedReason.Break, debugger.PausedReason);
			Assert.AreEqual("debugger.tests.exe", Path.GetFileName(assemblyFilename).ToLower());
			Assert.AreEqual(true, debugger.GetModule(Path.GetFileName(assemblyFilename)).SymbolsLoaded, "Module symbols not loaded");
			debugger.Continue();
			
			debugger.WaitForPrecessExit();
		}
		
		[Test]
		public void Breakpoint()
		{
			Breakpoint b = debugger.AddBreakpoint(@"D:\corsavy\SharpDevelop\src\AddIns\Misc\Debugger\Debugger.Tests\Project\Src\TestPrograms\Breakpoint.cs", 18);
			
			StartProgram("Breakpoint");
			
			debugger.WaitForPause();
			Assert.AreEqual(PausedReason.Break, debugger.PausedReason);
			Assert.AreEqual("", log);
			Assert.AreEqual(true, b.Enabled);
			Assert.AreEqual(true, b.HadBeenSet, "Breakpoint is not set");
			Assert.AreEqual(18, b.SourcecodeSegment.StartLine);
			debugger.Continue();
			
			debugger.WaitForPause();
			Assert.AreEqual(PausedReason.Breakpoint, debugger.PausedReason, "Breakpoint was not hit");
			Assert.AreEqual("Mark 1\r\n", log);
			debugger.Continue();
			
			debugger.WaitForPause();
			Assert.AreEqual(PausedReason.Break, debugger.PausedReason);
			Assert.AreEqual("Mark 1\r\nMark 2\r\n", log);
			debugger.Continue();
			
			debugger.WaitForPrecessExit();
			
			Assert.AreEqual("Mark 1\r\nMark 2\r\n", log);
		}
	}
}
