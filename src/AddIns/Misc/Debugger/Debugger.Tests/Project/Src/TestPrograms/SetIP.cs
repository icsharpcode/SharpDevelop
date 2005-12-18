// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.Tests.TestPrograms
{
	public class SetIP
	{
		public static void Main()
		{
			System.Diagnostics.Debug.WriteLine("1");
			System.Diagnostics.Debugger.Break();
		}
	}
}
