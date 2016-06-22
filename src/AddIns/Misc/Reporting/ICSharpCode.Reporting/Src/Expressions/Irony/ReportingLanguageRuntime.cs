// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Linq;
using ICSharpCode.Reporting.Expressions.Irony.Imports;
using Irony.Interpreter;
using Irony.Parsing;

namespace ICSharpCode.Reporting.Expressions.Irony
{
	/// <summary>
	/// Description of ReportingLanguageRuntime.
	/// </summary>
	public class ReportingLanguageRuntime:LanguageRuntime
	{
		public ReportingLanguageRuntime(LanguageData language):base(language)
		{
		}
		
		public override void Init()
		{
			base.Init();
			//add built-in methods, special form IIF, import Math and Environment methods
//			BuiltIns.AddMethod(BuiltInPrintMethod, "print");
//			BuiltIns.AddMethod(BuiltInFormatMethod, "format");	
			BuiltIns.AddSpecialForm(SpecialFormsLibrary.Iif, "iif", 3, 3);
			BuiltIns.ImportStaticMembers(typeof(System.Math));
			BuiltIns.ImportStaticMembers(typeof(Environment));
			BuiltIns.ImportStaticMembers(typeof(System.DateTime));
			//Aggregates
			BuiltIns.AddSpecialForm(ImportAggregates.Sum,"sum",1,1);
		}
		
		/*
		private object BuiltInPrintMethod(ScriptThread thread, object[] args) {
			string text = string.Empty;
			switch(args.Length) {
				case 1:
					text = string.Empty + args[0]; //compact and safe conversion ToString()
					break;
				case 0:
					break;
				default:
					text = string.Join(" ", args);
					break;
			}
			thread.App.WriteLine(text);
			return null;
		}
		*/
		
		/*
		private object BuiltInFormatMethod(ScriptThread thread, object[] args) {
			if (args == null || args.Length == 0) return null;
			var template = args[0] as string;
			if (template == null)
				this.ThrowScriptError("Format template must be a string.");
			if (args.Length == 1) return template;
			//create formatting args array
			var formatArgs = args.Skip(1).ToArray();
			var text = string.Format(template, formatArgs);
			return text;
			
		}
		*/
	}
}
