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
				case "SimpleProgram": Progs.SimpleProgram.Main(); break;
				case "HelloWorld": Progs.HelloWorld.Main(); break;
				case "Breakpoint": Progs.Breakpoint.Main(); break;
			}
		}
	}
}
