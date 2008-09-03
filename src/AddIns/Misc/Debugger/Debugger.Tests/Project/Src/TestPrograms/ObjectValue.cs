// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.Tests.TestPrograms
{
	public class BaseClass2
	{
		public string basePublic = "a";
		string basePrivate = "b";
	}
	
	public class ObjectValue: BaseClass2
	{
		string privateField = "private";
		public string publicFiled = "public";
		
		public string PublicProperty {
			get {
				return privateField;
			}
		}
		
		public static void Main()
		{
			ObjectValue val = new ObjectValue();
			System.Diagnostics.Debugger.Break();
			val.privateField = "new private";
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
		public void ObjectValue()
		{
			Value val = null;
			
			StartTest("ObjectValue.cs");
			
			val = process.SelectedStackFrame.GetLocalVariableValue("val");
			ObjectDump("val", val);
			ObjectDump("val members", val.GetMemberValues());
			
			process.Continue();
			ObjectDump("val", val);
			ObjectDump("val members", val.GetMemberValues());
			
			EndTest();
		}
	}
}
#endif

#if EXPECTED_OUTPUT
#endif // EXPECTED_OUTPUT