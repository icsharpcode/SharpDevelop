// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.Tests.TestPrograms
{
	public class FunctionArgumentVariables
	{
		public static void Main()
		{
			System.Diagnostics.Debugger.Break();
			
			StaticFunction(0, "S");
			StaticFunction(1, "S", "p1");
			StaticFunction(2, null, "p1", "p2");
			new FunctionArgumentVariables().Function(0, "S");
			new FunctionArgumentVariables().Function(1, "S", "p1");
			new FunctionArgumentVariables().Function(2, null, "p1", "p2");
		}
		
		static void StaticFunction(int i, string s, params string[] args)
		{
			System.Diagnostics.Debugger.Break();
		}
		
		void Function(int i, string s, params string[] args)
		{
			System.Diagnostics.Debugger.Break();
		}
	}
}
