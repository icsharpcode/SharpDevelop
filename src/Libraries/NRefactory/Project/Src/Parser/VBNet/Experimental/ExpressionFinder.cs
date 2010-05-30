// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com" />
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Text;

namespace ICSharpCode.NRefactory.Parser.VBNet.Experimental
{
	public partial class ExpressionFinder
	{
		Stack<Context> stack = new Stack<Context>();
		StringBuilder output = new StringBuilder();
		bool isExpressionStart = false;
		
		void PopContext()
		{
			if (stack.Count > 0) {
				string indent = new string('\t', stack.Count - 1);
				var item = stack.Pop();
				Print(indent + "exit " + item);
			} else {
				Print("empty stack");
			}
		}
		
		void PushContext(Context context)
		{
			string indent = new string('\t', stack.Count);
			stack.Push(context);
			Print(indent + "enter " + context);
		}
		
		void SetContext(Context context)
		{
			PopContext();
			PushContext(context);
		}
		
		void Print(string text)
		{
			Console.WriteLine(text);
			output.AppendLine(text);
		}
		
		public string Output {
			get { return output.ToString(); }
		}
	}
	
	public enum Context {
		Global,
		Type,
		Member,
		IdentifierExpected,
		Body,
		Xml,
		Attribute,
		Debug
	}
}
