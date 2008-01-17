// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.Tests.TestPrograms
{
	public class BaseClass
	{
		string name = "base name";
		
		public string Name {
			get { return name; }
		}
		
		public string Value = "base value";
	}
	
	public class TestClass: BaseClass
	{
		string name = "derived name";
		
		new public string Name {
			get { return name; }
		}
		
		new public string Value = "derived value";
		
		string field = "field value";
		string[] array = {"one", "two", "three"};
		
		public static void Main()
		{
			new TestClass().Test("argValue");
		}
		
		public void Test(string arg)
		{
			int i = 0;
			string[] array = {"one", "two", "three"};
			string[,] array2 = {{"A","B"},{"C","D"}};
			
			System.Diagnostics.Debugger.Break();
		}
	}
}

#if TEST_CODE
namespace Debugger.Tests {
	public partial class DebuggerTests
	{
		[NUnit.Framework.Test, NUnit.Framework.Ignore]
		public void Expressions()
		{
			StartTest("Expressions.cs");
			WaitForPause();
			
			ObjectDump("Arguments", process.SelectedStackFrame.GetArgumentValues());
			ObjectDump("LocalVariables", process.SelectedStackFrame.GetLocalVariableValues());
			ObjectDump("this", process.SelectedStackFrame.GetThisValue().GetMemberValues());
			
			process.Continue();
			process.WaitForExit();
			CheckXmlOutput();
		}
	}
}
#endif

#if EXPECTED_OUTPUT
#endif // EXPECTED_OUTPUT