// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com" />
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

namespace ICSharpCode.NRefactory.Parser.VBNet.Experimental
{
	public partial class Parser
	{
		Stack<Context> stack = new Stack<Context>();
		
		void PopContext()
		{
			if (stack.Count > 0) {
				string indent = new string('\t', stack.Count - 1);
				var item = stack.Pop();
				Console.WriteLine(indent + "exit " + item);
			} else {
				Console.WriteLine("empty stack");
			}
		}
		
		void PushContext(Context context)
		{
			string indent = new string('\t', stack.Count);
			stack.Push(context);
			Console.WriteLine(indent + "enter " + context);
		}
		
		void SetContext(Context context)
		{
			PopContext();
			PushContext(context);
		}
	}
	
	public enum Context {
		Global,
		Type,
		Member,
		IdentifierExpected
	}
}
