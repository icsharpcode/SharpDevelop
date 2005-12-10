// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.Tests.TestPrograms
{
	public class Stepping
	{
		public static void Main()
		{
			System.Diagnostics.Debugger.Break();
			System.Diagnostics.Debug.WriteLine("1"); // Step over external code
			Sub(); // Step in internal code
			Sub2(); // Step over internal code
		}
		
		public static void Sub()
		{ // Step in noop
			System.Diagnostics.Debug.WriteLine("2"); // Step in external code
			System.Diagnostics.Debug.WriteLine("3"); // Step out
			System.Diagnostics.Debug.WriteLine("4");
		}
		
		public static void Sub2()
		{
			System.Diagnostics.Debug.WriteLine("5");
		}
	}
}
