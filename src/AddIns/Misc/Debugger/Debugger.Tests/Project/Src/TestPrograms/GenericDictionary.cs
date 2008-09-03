// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

namespace Debugger.Tests.TestPrograms
{
	public class GenericDictionary
	{
		public static void Main()
		{
			Dictionary<string, int> dict = new Dictionary<string, int>();
			dict.Add("one",1);
			dict.Add("two",2);
			dict.Add("three",3);
			System.Diagnostics.Debugger.Break();
		}
	}
}

#if TEST_CODE
namespace Debugger.Tests {
	using Debugger.MetaData;
	
	public partial class DebuggerTests
	{
		[NUnit.Framework.Test, NUnit.Framework.Ignore]
		public void GenericDictionary()
		{
			StartTest("GenericDictionary.cs");
			
			ObjectDump("dict", process.SelectedStackFrame.GetLocalVariableValue("dict"));
			ObjectDump("dict members", process.SelectedStackFrame.GetLocalVariableValue("dict").GetMemberValues());
			
			EndTest();
		}
	}
}
#endif

#if EXPECTED_OUTPUT
#endif // EXPECTED_OUTPUT