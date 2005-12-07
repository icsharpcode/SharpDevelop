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
		
		public DebuggerTests()
		{
			debugger = new NDebugger();
			debugger.LogMessage += delegate(object sender, MessageEventArgs e) {
				log += e.Message;
				lastLogMessage = e.Message;
			};
		}
		
		void StartProgram(string name)
		{
			log = "";
			lastLogMessage = null;
			string filename = Assembly.GetCallingAssembly().Location;
			debugger.Start(filename, Path.GetDirectoryName(filename), name);
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
		
		[Test, Ignore("Deadlocks")]
		public void Breakpoint()
		{
			debugger.AddBreakpoint("Breakpoint.cs", 16);
			
			StartProgram("Breakpoint");
			
			debugger.WaitForPause();
			
			Assert.AreEqual(PausedReason.Breakpoint, debugger.PausedReason);
			Assert.AreEqual("", log);
			
			debugger.Continue();
			
			debugger.WaitForPrecessExit();
			
			Assert.AreEqual("Line 16\r\n", log);
		}
	}
}
