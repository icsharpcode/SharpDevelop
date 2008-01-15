// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.Tests.TestPrograms
{
	class MyException: System.Exception
	{
		public MyException(string msg) : base (msg)
		{
			
		}
	}
	
	public class ExceptionCustom
	{
		public static void Main()
		{
			throw new MyException("test");
		}
	}
}

#if TESTS
namespace Debugger.Tests {
	public partial class DebuggerTests
	{
		[NUnit.Framework.Test]
		public void ExceptionCustom()
		{
			StartTest("ExceptionCustom");
			WaitForPause();
			process.Continue();
			process.WaitForExit();
			CheckXmlOutput();
		}
	}
}
#endif