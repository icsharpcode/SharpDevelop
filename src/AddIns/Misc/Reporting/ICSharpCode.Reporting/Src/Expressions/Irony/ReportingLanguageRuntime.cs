// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
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
			//      BuiltIns.AddMethod(BuiltInPrintMethod, "print");
			//      BuiltIns.AddMethod(BuiltInFormatMethod, "format");
			BuiltIns.AddSpecialForm(SpecialFormsLibrary.Iif, "iif", 3, 3);
			BuiltIns.ImportStaticMembers(typeof(System.Math));
			BuiltIns.ImportStaticMembers(typeof(Environment));
		}
	}
}
