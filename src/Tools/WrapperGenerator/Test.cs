// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.Interop.CorDebug
{
	[Flags]
	public enum Enumeration:byte { A = 1, B = 2, C = 4, D = 16 };
	
	public interface Test {
		// Return values
		void void__void();
		
		int int__void();
		
		string string__void();
		
		Test Test__void();
		
		// Direction of parameter - base type
		void void__inString(string str);
		
		void void__outString(out string str);
		
		void void__refString(ref string str);
		
		// Direction of parameter - wrapped type
		void void__inTest(Test str);
		
		void void__outTest(out Test t);
		
		void void__refTest(ref Test t);
		
		// Direction of many parameters + return value - base type
		void void__out_ref_in_out_ref__string(out string p1, ref string p2, string p3, out string p4, ref string p5);
		
		string string__out_ref_in_out_ref__string(out string p1, ref string p2, string p3, out string p4, ref string p5);
		
		// Direction of many parameters + return value - wrapped type
		void void__out_ref_in_out_ref__Test(out Test p1, ref Test p2, Test p3, out Test p4, ref Test p5);
		
		Test Test__out_ref_in_out_ref__Test(out Test p1, ref Test p2, Test p3, out Test p4, ref Test p5);
		
		// Arrays
		
		void void__array(Test[] arg);
	}
}
