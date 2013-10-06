// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using ICSharpCode.Reporting.Items;
using Irony.Interpreter;

namespace ICSharpCode.Reporting.Expressions.Irony.Ast
{
	/// <summary>
	/// Description of AstExtensions.
	/// </summary>
	public static class AstExtensions
	{
		public static ParameterCollection GetParametersCollection (this ScriptThread thread){
			var rs =  (ReportSettings)thread.App.Globals["ReportSettings"];
			return rs.ParameterCollection;
		}
	}
}
