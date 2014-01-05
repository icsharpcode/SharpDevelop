// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Collections.Generic;
using ICSharpCode.Reporting.DataManager.Listhandling;
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
			
			if (!app.Globals.ContainsKey("PageInfo")) {
				app.Globals.Add("PageInfo",pageInfo);
			} else {
				app.Globals["PageInfo"] = pageInfo;
			}
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
			if (!app.Globals.ContainsKey("CurrentContainer")) {
				app.Globals.Add("CurrentContainer",container);
			} else {
				app.Globals["CurrentContainer"] = container;
			}
		}
		
		public static ExportContainer GetCurrentContainer (this ScriptThread thread){
			return (ExportContainer)thread.App.Globals["CurrentContainer"];
		}
		
		#endregion
		
		#region DataSource
		
		public static void AddDataSource (this ReportingExpressionEvaluator app,IEnumerable<object> dataSource){
			if (dataSource == null)
				throw new ArgumentNullException("dataSource");
			if (!app.Globals.ContainsKey("DataSource")) {
			    	app.Globals.Add("DataSource",dataSource);
			    } else {
			    	app.Globals["DataSource"] = dataSource;
			    }
		}
		
		
		
		#endregion
	}
}
