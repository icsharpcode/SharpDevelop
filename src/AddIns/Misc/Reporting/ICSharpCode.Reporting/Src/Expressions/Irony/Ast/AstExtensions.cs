// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using ICSharpCode.Reporting.BaseClasses;
using ICSharpCode.Reporting.Interfaces.Export;
using ICSharpCode.Reporting.Items;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;
using Irony.Interpreter;

namespace ICSharpCode.Reporting.Expressions.Irony.Ast
{
	/// <summary>
	/// Description of AstExtensions.
	/// </summary>
	public static class AstExtensions
	{
		#region Globals
		public static void AddPageInfo (this ReportingExpressionEvaluator app,IPageInfo pageInfo) {
			if (pageInfo == null)
				throw new ArgumentNullException("pageInfo");
			
			app.Globals.Add("PageInfo",pageInfo);
		}
		
		
		public static IPageInfo GetPageInfo (this ScriptThread thread){
			var pi = (IPageInfo)thread.App.Globals["PageInfo"];
			return pi;
		}
		#endregion
		
		
		#region Parameters
		
		public static ParameterCollection GetParametersCollection (this ScriptThread thread){
			var rs =  (ReportSettings)thread.App.Globals["ReportSettings"];
			return rs.ParameterCollection;
		}
		
		public static void  AddReportSettings (this ReportingExpressionEvaluator app,ReportSettings reportSettings) {
			if (reportSettings == null)
				throw new ArgumentNullException("reportSettings");
			app.Globals.Add("ReportSettings",reportSettings);
		}
		
		#endregion
		
		#region current Container
		
		public static void AddCurrentContainer (this ReportingExpressionEvaluator app,ExportContainer container){
			if (container == null)
				throw new ArgumentNullException("container");
			app.Globals.Add("CurrentContainer",container);
			
		}
		
		public static ExportContainer GetCurrentContainer (this ScriptThread thread){
			return (ExportContainer)thread.App.Globals["CurrentContainer"];
		}
		#endregion
	}
}
