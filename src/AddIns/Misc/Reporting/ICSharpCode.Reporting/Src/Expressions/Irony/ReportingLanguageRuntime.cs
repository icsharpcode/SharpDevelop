// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Linq;
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
			BuiltIns.AddMethod(BuiltInFormatMethod, "format");
			
			BuiltIns.AddSpecialForm(SpecialFormsLibrary.Iif, "iif", 3, 3);
			BuiltIns.ImportStaticMembers(typeof(System.Math));
			BuiltIns.ImportStaticMembers(typeof(Environment));
		}
		
		
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
		
	}
}
