// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.Tests.TestPrograms
{
	public class HelloWorld
	{
		public static void Main()
		{
			System.Diagnostics.Debug.WriteLine("Hello world!");
		}
	}
}

#if TESTS
namespace Debugger.Tests {
	public partial class DebuggerTests
	{
		[NUnit.Framework.Test]
		public void HelloWorld()
		{
			StartTest("HelloWorld");
			process.WaitForExit();
			CheckXmlOutput();
		}
	}
}
#endif