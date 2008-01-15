// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.Tests.TestPrograms
{
	public class Exception
	{
		public static void Main()
		{
			throw new System.Exception("test");
		}
	}
}

#if TESTS
namespace Debugger.Tests {
	public partial class DebuggerTests
	{
		[NUnit.Framework.Test]
		public void Exception()
		{
			StartTest("Exception");
			WaitForPause();
			process.Continue();
			process.WaitForExit();
			CheckXmlOutput();
		}
	}
}
#endif