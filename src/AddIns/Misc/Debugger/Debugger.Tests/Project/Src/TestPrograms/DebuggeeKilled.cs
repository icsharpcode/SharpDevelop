// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.Tests.TestPrograms
{
	public class DebuggeeKilled
	{
		public static void Main()
		{
			int id = System.Diagnostics.Process.GetCurrentProcess().Id;
			System.Diagnostics.Debug.WriteLine(id.ToString());
			System.Diagnostics.Debugger.Break();
		}
	}
}
