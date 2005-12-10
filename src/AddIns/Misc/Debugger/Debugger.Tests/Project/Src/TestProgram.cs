// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

using Progs = Debugger.Tests.TestPrograms;

namespace Debugger.Tests
{
	public class TestProgram
	{
		public static void Main(string[] args)
		{
			if (args.Length == 0) {
				return;
			}
			switch (args[0]) {
				case "Break": Progs.Break.Main(); break;
				case "Breakpoint": Progs.Breakpoint.Main(); break;
				case "Callstack": Progs.Callstack.Main(); break;
				case "FileRelease": Progs.FileRelease.Main(); break;
				case "FunctionArgumentVariables": Progs.FunctionArgumentVariables.Main(); break;
				case "FunctionLocalVariables": Progs.FunctionLocalVariables.Main(); break;
				case "HelloWorld": Progs.HelloWorld.Main(); break;
				case "SimpleProgram": Progs.SimpleProgram.Main(); break;
				case "Stepping": Progs.Stepping.Main(); break;
				case "Symbols": Progs.Symbols.Main(); break;
			}
		}
	}
}
