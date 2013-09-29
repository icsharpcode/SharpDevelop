// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using Irony;
using Irony.Interpreter;
using Irony.Interpreter.Evaluator;
using Irony.Parsing;

namespace ICSharpCode.Reporting.Expressions.Irony
{
	/// <summary>
	/// Description of ReportingLanguageGrammer.
	/// </summary>
	public class ReportingLanguageGrammer:ExpressionEvaluatorGrammar
	{
		public ReportingLanguageGrammer()
		{
		}
		
		public override LanguageRuntime CreateRuntime(LanguageData language)
		{
			return new ReportingLanguageRuntime(language);
		}
	}
}
